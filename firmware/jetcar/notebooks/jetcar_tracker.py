# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`jetcar_tracker`
===============
The tracker module contains the top level class for the jetcar processing, utilizing
the definitions and classes from jetcar_definitions, jetcar_lane and jetcar_center.
The LaneTracker class handles the processing of the segmentation class mask, handles
steering and direction changes.

Following classes are implemented here:
class LaneTracker:
    This class holds the LaneLimits instances for left and right but also 
    resulting point arrays for center and left and right neighboring lanes.
    It is the top level class to create and call its process or draw methods,
    that will execute all related methods of the owned lower objects.

The main API function of this class is:
    def process(self, mask, next_turn_direction:Direction):
        Top level method to process a new mask contents. This method calls 
        the find_limits methods for the left and right lane limits objects.
        It then checks the lane widths for each pair and corrects, if out of 
        expected bounds to clean up more. It then extracts a steering value 
        to keep the car in the lane or to perform a lane switch or a turn 
        left or right depening on the condition for the  next_turn_direction.
        mask                -- segmentation mask containing the SegmClass code.
        next_turn_direction -- direction code for the next upcoming turn. 

* Author(s): Stefan Warnke

Dependencies:
jetcar_definitions.py
jetcar_lane.py
jetcar_center.py
"""

from jetcar_definitions import *
from jetcar_lane import *
from jetcar_center import *
import cv2
import math
import time
import jetcar_lane

# Set to true to enable debug print messages when finding lane limits
DEBUG_PRINT_GET_LANE_LIMITS = False
# Set to true to enable debug print messages when handling directions
DEBUG_PRINT_HANDLE_DIRECTIONS = False
# Set to true to enable debug print messages in process_classes
DEBUG_PRINT_PROCESS_CLASSES = False

# When less points are found, run corrections on the points
LIMIT_FOUND_CORRECTION_THRESHOLD = N_SEARCH_POINTS - 2
# Few y coordinates for calculating the steering value
STEERING_Y = [IMG_YMAX, (IMG_YMAX+IMG_YC)//2, IMG_YC]
# Scaling factor from x coordinate to steering value
STEERING_X_SCALE = 2.5*IMG_XC
# Number of cycles to stop at the wait line
THROTTLE_STOP_CYCLES = 10
# Throttle value when approaching an intersection
THROTTLE_VALUE_APPROACHING_INTERSECTION = 0.75
# Throttle value when entering the inner intersection area
THROTTLE_VALUE_ENTERING_INTERSECTION = 0.6
# Throttle value to be used in a turn of an intersection
THROTTLE_VALUE_IN_TURN = 0.8
# Throttle value to be used in a curve outside intersections
THROTTLE_VALUE_IN_CURVE = 0.9
# Steering value threshold for reducing the speed to THROTTLE_VALUE_IN_CURVE
THROTTLE_CURVE_STEERING_THRESHOLD = 0.3
# Filter coefficient to low pass new throttle value with old one
THROTTLE_VALUE_FILTER_COEFF = 0.25
# Number of cycles to keep the current steering value when turning
STEERING_FIXED_TURN_CYCLES = 16
# Cycle count in fixed turn to switch to diagonal search
STEERING_FIXED_TURN_DIAG_CYCLES = (STEERING_FIXED_TURN_CYCLES//2) + 2
# Fixed steering value used for left turn, left turn can be wider
STEERING_LEFT_TURN_VALUE = -0.8
# Fixed steering value used for right turn, right turn must be tight
STEERING_RIGHT_TURN_VALUE = 1.0
# Minimum distance of left turn code to ORIGIN to start turning left
MIN_TURN_LEFT_DISTANCE = 100
# Minimum distance of right turn code to ORIGIN to start turning right
MIN_TURN_RIGHT_DISTANCE = 60
# Minimum limit found point count to check for switching back to straight after turn
MINIMUM_LIMIT_FOUND_POINTS = (2*N_SEARCH_POINTS)//3
# A threshold to smooth out small steering changes with a low pass
STEERING_FILTER_THRESHOLD = 0.05
# An average slope threshold for intersection detection to see if there is "Nothing" ahead
AVG_SLOPE_THRESHOLD_INTERSECTION = 5
# Switch back to straight direction when new average slope value goes above after turn
AVG_SLOPE_THRESHOLD_STRAIGHT = 3
# Switch to diagonal direction when new verage slope falls below
AVG_SLOPE_THRESHOLD_DIAG = 0.9

# ============================================================================================


class LaneTracker:
    """ This class holds the LaneLimits instances for left and right but also 
    resulting point arrays for center and left and right neighboring lanes.
    It is the top level class to create and call its process or draw methods,
    that will execute all related methods of the owned lower objects """

    def __init__(self):
        """ Initializes the LaneTracker object and creates its owned objects
        for left and right lane limits, center and neighbor arrays"""
        self.lane_limits_left = LaneLimits(Side.Left)
        self.lane_limits_right = LaneLimits(Side.Right)
        self.lane_left = LaneCenter(Side.Left, self.lane_limits_left)
        self.lane_center = LaneCenter(Side.Center, self.lane_limits_left)
        self.lane_right = LaneCenter(Side.Right, self.lane_limits_left)
        self.direction = Direction.Straight
        self.steering_value = 0
        self.new_steering_value = 0
        self.steering_fixed_count = 0
        self.turn_left_allowed = True
        self.turn_right_allowed = True
        self.go_straight_allowed = True
        self.throttle_value = 0
        self.throttle_new_value = 0
        self.stop_count = 0
        self.signal_left_enable = False
        self.signal_right_enable = False
        self.brake_light_enable = False
        self.steering_control = False
        self.intersection = False
        self.stop_detected = False
        self.slope_avg = 0

        if jetcar_lane.DEBUG_PRINT_FIND_LIMITS == True:
            print(self.lane_limits_left)
            print(self.lane_limits_right)

        return

    def get_lane_limits(self, mask):
        """ First find the lane limits. Since some of the limit searches might fail,
        correct them via trying to align those with the valid points.        
        mask        -- segmentation mask containing the SegmClass code."""

        if jetcar_lane.DEBUG_PRINT_FIND_LIMITS == True:
            print("find limits left:")
        self.lane_limits_left.find_limits(mask, self.direction)
        if jetcar_lane.DEBUG_PRINT_FIND_LIMITS == True:
            print("find limits right:")
        self.lane_limits_right.find_limits(mask, self.direction)

        if jetcar_lane.DEBUG_PRINT_FIND_LIMITS == True or DEBUG_PRINT_GET_LANE_LIMITS == True:
            print("stddev left/right: %.3f / %.3f  limits_found: %d / %d" % \
                  (self.lane_limits_left.stddev,
                   self.lane_limits_right.stddev,
                   self.lane_limits_left.limit_found_count,
                   self.lane_limits_right.limit_found_count))

            print("+++++++++++++++++++++++++++++++++++++")
            print(f'{self.lane_limits_left}')
            print(f'{self.lane_limits_right}')
            print("")

        # The lane limit points are corrected to better fit a line, but the
        # width between them can still be out of expected bounds, so it will
        # be corrected here. The limit line with more noise will be corrected.
        dx, dy = DEFINITIONS_LEFT[self.direction.value][2:4]
        corrected = False
        correct_left = False
        correct_center = False
        correct_right = (self.lane_limits_right.limit_found_count <= self.lane_limits_left.limit_found_count) and \
                        (self.lane_limits_right.stddev > 2*self.lane_limits_left.stddev) and \
                        (self.lane_limits_left.stddev < 10)

        if correct_right == True:
            if DEBUG_PRINT_GET_LANE_LIMITS == True:
                print("correct right:")
        else:
            correct_left = (self.lane_limits_left.limit_found_count <= self.lane_limits_right.limit_found_count) and \
                           (self.lane_limits_left.stddev > 2*self.lane_limits_right.stddev) and \
                           (self.lane_limits_right.stddev < 10)
            if correct_left == True:
                if DEBUG_PRINT_GET_LANE_LIMITS == True:
                    print("correct left:")
            else:
                correct_center = (self.lane_limits_left.stddev > 3) and \
                                 (self.lane_limits_left.stddev > 3)

                if DEBUG_PRINT_GET_LANE_LIMITS == True and correct_center == True:
                    print("correct center:")

        if DEBUG_PRINT_GET_LANE_LIMITS == True:
            sn = "min. widths: "
            sm = "nom. widths: "
            sx = "max. widths: "
            s1 = "orig widths: "
            s2 = "corr widths: "

        # Now go through the pairs, calculate the width and correct left,
        # right or center if out of estimated bounds
        for i in range(N_LANE_POINTS):
            point_left = self.lane_limits_left.points[i]
            point_right = self.lane_limits_right.points[i]
            lane_width = point_left.get_distance(point_right)
            point_left.update_lane_widths(self.direction)
            nom_lane_width_dx = point_left.nom_lane_width*dx
            nom_lane_width_dy = point_left.nom_lane_width*dy
            if self.direction in [Direction.LeftDiag, Direction.RightDiag]:
                nom_lane_width_dx = round(nom_lane_width_dx*DIAG_FACTOR)
                nom_lane_width_dy = round(nom_lane_width_dy*DIAG_FACTOR)

            if DEBUG_PRINT_GET_LANE_LIMITS == True:
                s1 += "  %3d " % (lane_width)
                sn += "  %3d " % (point_left.min_lane_width)
                sm += "  %3d " % (point_left.nom_lane_width)
                sx += "  %3d " % (point_left.max_lane_width)

            if (lane_width < point_left.min_lane_width) or (lane_width > point_left.max_lane_width):
                if (lane_width > point_left.max_lane_width):
                    corr_lane_width_dx = point_left.max_lane_width*dx
                    corr_lane_width_dy = point_left.max_lane_width*dy
                    if self.direction in [Direction.LeftDiag, Direction.RightDiag]:
                        corr_lane_width_dx = round(corr_lane_width_dx*DIAG_FACTOR)
                        corr_lane_width_dy = round(corr_lane_width_dy*DIAG_FACTOR)
                else:
                    corr_lane_width_dx = nom_lane_width_dx
                    corr_lane_width_dy = nom_lane_width_dy

                if correct_right == True:
                    point_right.x = min(max(point_left.x - corr_lane_width_dx,0),IMG_XMAX)
                    point_right.y = min(max(point_left.y - corr_lane_width_dy,0),IMG_YMAX)
                    corrected = True

                    if DEBUG_PRINT_GET_LANE_LIMITS == True:
                        s2 += "R_"

                elif correct_left == True:
                    point_left.x = min(max(point_right.x + corr_lane_width_dx,0),IMG_XMAX)
                    point_left.y = min(max(point_right.y + corr_lane_width_dy,0),IMG_YMAX)
                    corrected = True

                    if DEBUG_PRINT_GET_LANE_LIMITS == True:
                        s2 += "L_"

                elif correct_center == True:
                    center_x = round((point_left.x + point_right.x)/2)
                    center_y = round((point_left.y + point_right.y)/2)
                    half_lane_width_dx = corr_lane_width_dx//2
                    half_lane_width_dy = corr_lane_width_dy//2
                    point_left.x = min(max(center_x + half_lane_width_dx,0),IMG_XMAX)
                    point_left.y = min(max(center_y + half_lane_width_dy,0),IMG_YMAX)
                    point_right.x = min(max(center_x - half_lane_width_dx,0),IMG_XMAX)
                    point_right.y = min(max(center_y - half_lane_width_dy,0),IMG_YMAX)
                    corrected = True

                    if DEBUG_PRINT_GET_LANE_LIMITS == True:
                        s2 += "C_"

                else:
                    if DEBUG_PRINT_GET_LANE_LIMITS == True:
                        s2 += "  "
            else:
                if DEBUG_PRINT_GET_LANE_LIMITS == True:
                    s2 += "  "

            if corrected == True:
                self.lane_limits_left.points[i].x = point_left.x
                self.lane_limits_left.points[i].y = point_left.y
                self.lane_limits_right.points[i].x = point_right.x
                self.lane_limits_right.points[i].y = point_right.y

            center_x = round((point_left.x + point_right.x)/2)
            center_y = round((point_left.y + point_right.y)/2)
                
            self.lane_center.points[i].x = center_x
            self.lane_center.points[i].y = center_y
            self.lane_left.points[i].x = min(max(point_left.x + nom_lane_width_dx//2,0),IMG_XMAX)
            self.lane_left.points[i].y = min(max(point_left.y + nom_lane_width_dy//2,0),IMG_YMAX)
            self.lane_right.points[i].x = min(max(point_right.x - nom_lane_width_dx//2,0),IMG_XMAX)
            self.lane_right.points[i].y = min(max(point_right.y - nom_lane_width_dy//2,0),IMG_YMAX)

#            self.lane_left.points[i].x = min(max(center_x + nom_lane_width_dx,0),IMG_XMAX)
#            self.lane_left.points[i].y = min(max(center_y + nom_lane_width_dy,0),IMG_YMAX)
#            self.lane_right.points[i].x = min(max(center_x - nom_lane_width_dx,0),IMG_XMAX)
#            self.lane_right.points[i].y = min(max(center_y - nom_lane_width_dy,0),IMG_YMAX)

            if DEBUG_PRINT_GET_LANE_LIMITS == True:
                s2 += "%3d " % (point_left.get_distance(point_right))

        if DEBUG_PRINT_GET_LANE_LIMITS == True:
            print(sn)
            print(sm)
            print(sx)
            print(s1)
            print(s2)
            print("")
            print(f'{self.lane_limits_left}')
            print(f'{self.lane_limits_right}')
            print("+++++++++++++++++++++++++++++++++++++")

        # Now that the lane limits are width-corrected, recalculate line approximation
        if corrected == True:
            if correct_right == True:
                self.lane_limits_right.update_line_approx()
            elif correct_left == True:
                self.lane_limits_left.update_line_approx()
            elif correct_center == True:
                self.lane_limits_left.update_line_approx()
                self.lane_limits_right.update_line_approx()
            
        # Calculate the average between both lane limit slopes avoiding 0 transition straight ahead
        if (self.lane_limits_left.slope * self.lane_limits_right.slope) < 0:
            self.slope_avg = 1e3
        else:
            self.slope_avg = (self.lane_limits_left.slope + self.lane_limits_right.slope)/2

        if DEBUG_PRINT_GET_LANE_LIMITS == True:
            print("get_lane_limits done: left=%.1f right==%.1f slope_avg=%.1f" % (self.lane_limits_left.slope, self.lane_limits_right.slope, self.slope_avg))
        return

    def update_lane_center_classes(self, mask):
        """ Update the segmentation class codes ond object lists in the center 
        of the lanes left and right and center lane. 
        mask       -- segmentation mask containing the SegmClass code."""
        self.lane_left.get_center_classes(mask)
        self.lane_center.get_center_classes(mask)
        self.lane_right.get_center_classes(mask)

        self.lane_left.update_objects()
        self.lane_center.update_objects()
        self.lane_right.update_objects()

        if jetcar_lane.DEBUG_MASK_IMG == True and jetcar_lane.DEBUG_MASK_IMG_CENTER == True:
            mask_img = jetcar_lane.DEBUG_MASK_IMG_REFERENCE.copy()
            mask_img = self.lane_left.draw_code_points(mask_img)
            mask_img = self.lane_center.draw_code_points(mask_img)
            mask_img = self.lane_right.draw_code_points(mask_img)
            cv2.imwrite(f'{jetcar_lane.DEBUG_MASK_IMG_FILE_NAME}_class_pts.jpg',mask_img)

        return

    def set_direction_enables(self, code):
        """ Look up the allowed directions specified for the given SegmClass code. 
        code        -- SegmClass code of any arrow street marking to be checked. """
        for i in range(len(DEFINITIONS_DIRECTIONS)):
            if code == DEFINITIONS_DIRECTIONS[i][0]:
                self.turn_left_allowed = DEFINITIONS_DIRECTIONS[i][1]
                self.go_straight_allowed = DEFINITIONS_DIRECTIONS[i][2]
                self.turn_right_allowed = DEFINITIONS_DIRECTIONS[i][3]
                if DEBUG_PRINT_PROCESS_CLASSES == True:
                    print("i:%d code: %d=%s in DEFINITIONS_DIRECTIONS, allowed: left=%s straight=%s right=%s" % \
                          (i, code, SegmClass(code).name, self.turn_left_allowed, self.go_straight_allowed, self.turn_right_allowed))
                return
        return

    def init_for_next_intersection(self):
        """Remove intersection history and enable all turns for next intersection"""
        self.turn_left_allowed = True
        self.go_straight_allowed = True
        self.turn_right_allowed = True
        self.lane_left.remove_intersection_objects()
        self.lane_center.remove_intersection_objects()
        self.lane_right.remove_intersection_objects()
        self.intersection = False
        self.signal_left_enable = False
        self.signal_right_enable = False
        self.stop_detected = False
        return

    def init_object_lists(self):
        """Init all object lists with empty list objects"""
        self.lane_left.init_object_list()
        self.lane_center.init_object_list()
        self.lane_right.init_object_list()
        return

    def update_throttle(self):
        self.brake_light_enable = self.throttle_new_value < self.throttle_value
        self.throttle_value = self.throttle_value * (1-THROTTLE_VALUE_FILTER_COEFF) + \
            self.throttle_new_value * THROTTLE_VALUE_FILTER_COEFF
        if DEBUG_PRINT_PROCESS_CLASSES == True:
            print("update_throttle  brake_light_enable=%s, throttle_new_value=%.3f, throttle_value=%.3f" % (self.brake_light_enable,self.throttle_new_value,self.throttle_value))
        return

    def process_classes(self, next_turn_direction: Direction):
        """ Processes the objects in the center of the current lane and handles 
        direction arrow street markings to determine the allowed directions of 
        any intersection, the slow down and brake lights when entering an intersection
        or acceleration when the intersection is passed.
        next_turn_direction     -- direction code for the next upcoming turn.  """

        if DEBUG_PRINT_PROCESS_CLASSES == True:
            print("process_classes direction:%s  next_turn_direction:%s  stop_count=%d " % (Direction(self.direction).name, Direction(next_turn_direction).name, self.stop_count))

        if self.stop_count > 0:
            self.stop_count -= 1
            if self.stop_count > 0:
                self.throttle_value = 0
            else:
                self.stop_detected = False
                self.lane_left.remove_intersection_objects()
                self.lane_center.remove_intersection_objects()
                self.lane_right.remove_intersection_objects()
                self.throttle_value = THROTTLE_VALUE_ENTERING_INTERSECTION
            return
 
        self.intersection = False

        code = self.lane_center.get_dominant_arrow_code()
        if code != NOTHING_CODE:
            self.set_direction_enables(code)
            self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
            self.intersection = True
            if DEBUG_PRINT_PROCESS_CLASSES == True:
                print("dominant arrow code: %d=%s" % (code, SegmClass(code).name))

        for i in range(len(self.lane_center.objects)):
            if self.lane_center.objects[i].alive > CLASS_CODE_OBJ_TTL:
                code = self.lane_center.objects[i].code
                # Check codes in front
                if self.stop_detected == False and code == STOP_TEXT_CODE and self.lane_center.objects[i].distance < 5*CLASS_CODE_OBJ_ORIGIN_DIST and \
                    self.lane_center.objects[i].size >= CLASS_CODE_OBJ_MIN_SIZE and self.lane_center.objects[i].alive >= CLASS_CODE_OBJ_MIN_ALIVE:
                    if DEBUG_PRINT_PROCESS_CLASSES == True:
                        print("i:%d code: %d=%s == STOP_TEXT_CODE at dist=%d" % (i, code, SegmClass(code).name, self.lane_center.objects[i].distance))
                    self.stop_detected = True
                    self.throttle_new_value = THROTTLE_VALUE_ENTERING_INTERSECTION
                    self.intersection = True
                    self.brake_light_enable = True

                elif code in INTERSECTION_OBJECT_CODES and self.lane_center.objects[i].distance > CLASS_CODE_OBJ_ORIGIN_DIST:
                    self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
                    self.intersection = True
                    if DEBUG_PRINT_PROCESS_CLASSES == True:
                        print("i:%d code: %d=%s in INTERSECTION_OBJECT_CODES" % (i, code, SegmClass(code).name))

                elif self.stop_detected == True:
                    if (code == STOP_LINE_CODE and self.lane_center.objects[i].distance <= CLASS_CODE_OBJ_ORIGIN_DIST) or \
                       (code == STOP_TEXT_CODE and self.lane_center.objects[i].distance <= 0):
                        if DEBUG_PRINT_PROCESS_CLASSES == True:
                            print("i:%d code: %d=%s == STOP_LINE_CODE and stop_detected==True" % (i, code, SegmClass(code).name))
                        self.throttle_new_value = 0
                        self.throttle_value = 0
                        self.stop_count = THROTTLE_STOP_CYCLES
                        self.brake_light_enable = True
                        return



        # If there were no arrows or other signs indicating an intersection and the car is not in a tight curve, check for left and right turns
        if self.intersection == False and abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_INTERSECTION:
            # Check left lane for a turn lane
            for i in range(len(self.lane_left.objects)):
                if self.lane_left.objects[i].code == LANE_LEFT_TURN_CODE and self.lane_left.objects[i].size >= CLASS_CODE_OBJ_MIN_SIZE and \
                   self.lane_left.objects[i].alive >= CLASS_CODE_OBJ_MIN_ALIVE:
                    self.intersection = True
                    self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
                    self.turn_left_allowed = True
                    if DEBUG_PRINT_PROCESS_CLASSES == True:
                        print("i:%d code: %d=%s == LANE_LEFT_TURN_CODE" % (i,self.lane_left.objects[i].code,\
                                                                        SegmClass(self.lane_left.objects[i].code).name))
                    break

            # Check right lane for a turn lane
            for i in range(len(self.lane_right.objects)):
                if self.lane_right.objects[i].code == LANE_RIGHT_TURN_CODE and self.lane_right.objects[i].size >= 30 and \
                   self.lane_right.objects[i].alive >= CLASS_CODE_OBJ_MIN_ALIVE and self.lane_right.objects[i].distance < 90:
                    self.intersection = True
                    self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
                    self.turn_right_allowed = True
                    if DEBUG_PRINT_PROCESS_CLASSES == True:
                        print("i:%d code: %d=%s == LANE_RIGHT_TURN_CODE" % (i,self.lane_right.objects[i].code,\
                                                                            SegmClass(self.lane_right.objects[i].code).name))
                    break

            # If there were no arrows, but there is an intersection, check if straight forward is allowed
            if self.intersection == True and len(self.lane_center.objects) > 0:
                for i in range(len(self.lane_center.objects)-1, 0):
                    if self.lane_center.objects[i].code in [NOTHING_CODE, SegmClass.lane_wrong_dir.value] and \
                       self.lane_center.objects[i].size >= CLASS_CODE_OBJ_MIN_SIZE and self.lane_center.objects[i].alive >= CLASS_CODE_OBJ_MIN_ALIVE:
                        # Somethimes, the condition above becomes true when an intersection comes up after a curve, so check the neighbors too
                        if self.lane_left.objects[len(self.lane_left.objects)-1].code in [NOTHING_CODE, SegmClass.lane_wrong_dir.value] and \
                           self.lane_right.objects[len(self.lane_right.objects)-1].code in [NOTHING_CODE, SegmClass.lane_wrong_dir.value]:
                            self.go_straight_allowed = False
                            if DEBUG_PRINT_PROCESS_CLASSES == True:
                                print("Straight not allowed! code: %d=%s in Nothing" % (self.lane_center.codes[N_CENTER_CLASS_POINTS-1].code,\
                                                                                        SegmClass(self.lane_center.codes[N_CENTER_CLASS_POINTS-1].code).name))

        # Found intersection conditions
        if self.intersection == True:
            # In the special case that straight forward is requested but it is not allowed, just stop and wait
            if next_turn_direction == Direction.Straight and self.go_straight_allowed == False:
                self.signal_left_enable = True
                self.signal_right_enable = True
                self.new_steering_value = 0
                self.steering_value = 0
                self.throttle_new_value = 0
                self.throttle_value = 0
                self.brake_light_enable = True
                if DEBUG_PRINT_PROCESS_CLASSES == True:
                    print("process_classes done, full stop: straight not allowed")
                return

            # Set turn signal enables
            self.signal_left_enable = self.turn_left_allowed and next_turn_direction == Direction.Left
            self.signal_right_enable = self.turn_right_allowed and next_turn_direction == Direction.Right

        else:
            # no intersection, full speed ahead and re-initialize for next intersection
            self.init_for_next_intersection()
            if abs(self.steering_value) > THROTTLE_CURVE_STEERING_THRESHOLD:
                self.throttle_new_value = THROTTLE_VALUE_IN_TURN
            else:
                self.throttle_new_value = 1.0

        # Now calculate the throttle value and handle brake light
        self.update_throttle()

        if DEBUG_PRINT_PROCESS_CLASSES == True:
            print("process_classes done, intersection=%s, allowed:left=%s, straight=%s, right=%s slope_avg=%.1f" % \
                  (self.intersection, self.turn_right_allowed, self.go_straight_allowed, self.turn_right_allowed, self.slope_avg))
        return

    def get_new_steering_value(self):
        """Calculate a new steering value by averaging the point offsets from the center"""
        lsum = 0
        rsum = 0
        # sum up left and right deltas to center
        for i in range(len(STEERING_Y)):
            lsum += self.lane_limits_left.get_x(STEERING_Y[i])-IMG_XC
            rsum += self.lane_limits_right.get_x(STEERING_Y[i])-IMG_XC

        # Scale the average between left and right deltas to new steering value
        self.new_steering_value = min(max((lsum + rsum)/STEERING_X_SCALE,-1),1) 
        return

    def handle_directions(self, mask, next_turn_direction: Direction):
        """ Check for direction change request and look for left or right lane codes 
        and initiate the turn when recognized. In case of no turn, a steering value is 
        calculated to keep the car in the center of the current lane.
        mask                -- segmentation mask containing the SegmClass code.
        next_turn_direction -- direction code for the next upcoming turn. """

        # First check, if fixed turning mode is active
        if self.steering_fixed_count > 0:
            if self.throttle_value > 0:
                self.steering_fixed_count -= 1

                # While turning, switch to diagonal search at half way point
                if self.steering_fixed_count == STEERING_FIXED_TURN_DIAG_CYCLES:
                    if self.direction == Direction.Left:
                        self.direction = Direction.LeftDiag
                    elif self.direction == Direction.Right:
                        self.direction = Direction.RightDiag
                    self.throttle_new_value = THROTTLE_VALUE_IN_TURN

                # If 0 is reached, switch back to straight directly
                elif self.steering_fixed_count == 0:
                    self.init_for_next_intersection()
                    self.direction = Direction.Straight

                # To avoid overshooting, start checking for early switching to straight
                elif self.steering_fixed_count < STEERING_FIXED_TURN_DIAG_CYCLES and \
                        self.lane_limits_left.limit_found_count > MINIMUM_LIMIT_FOUND_POINTS and \
                        self.lane_limits_left.limit_found_count > MINIMUM_LIMIT_FOUND_POINTS:
                    # There are enough valid points to base on, so check current steering
                    self.get_new_steering_value()
                    if abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_STRAIGHT:
                        self.init_for_next_intersection()
                        self.direction = Direction.Straight
                        self.steering_fixed_count = 0
                        self.steering_value = self.new_steering_value

                self.update_throttle()

            if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                print("keep turning: count=%d  new_steer=%.3f steer=%.3f  dir:%s" % \
                    (self.steering_fixed_count, self.new_steering_value, self.steering_value, self.direction.name))

            return

        # If a turn is requested, check for the conditions to initiate the turn
        if self.direction != next_turn_direction and self.intersection == True and self.stop_detected == False:
            if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                print("current direction(%s) != next_turn_direction(%s)" % (self.direction.name, next_turn_direction.name))

            # Check for left turn request
            if next_turn_direction == Direction.Left and self.turn_left_allowed == True:
                self.steering_control = False
                self.signal_left_enable = True
                for i in range(len(self.lane_left.objects)):
                    if self.lane_left.objects[i].code == LANE_LEFT_TURN_CODE and \
                       self.lane_left.objects[i].distance < MIN_TURN_LEFT_DISTANCE:
                        # initiate turn, if enough points indicate a left turn lane
                        self.direction = next_turn_direction
                        self.steering_value = STEERING_LEFT_TURN_VALUE
                        self.steering_fixed_count = STEERING_FIXED_TURN_CYCLES
                        self.init_object_lists()
                        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                            print("initiate turn left")
                        break

            # Check for right turn
            elif next_turn_direction == Direction.Right and self.turn_right_allowed == True:
                self.steering_control = False
                self.signal_right_enable = True
                for i in range(len(self.lane_right.objects)):
                    if self.lane_right.objects[i].code == LANE_RIGHT_TURN_CODE and \
                       self.lane_right.objects[i].distance < MIN_TURN_RIGHT_DISTANCE:
                        # initiate turn, if enough points indicate a right turn lane
                        self.direction = next_turn_direction
                        self.steering_value = STEERING_RIGHT_TURN_VALUE
                        self.steering_fixed_count = STEERING_FIXED_TURN_CYCLES
                        self.init_object_lists()
                        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                            print("initiate turn right")
                        break

        # No turn is requested or currently possible, calculate steering value to keep car
        # in the center of the current lane
        if self.steering_fixed_count == 0 and self.throttle_value > 0:
            self.steering_control = True
            self.get_new_steering_value()

            # If the difference between current and new value is small, average both to make it smoother
            if abs(self.new_steering_value-self.steering_value) < STEERING_FILTER_THRESHOLD:
                self.steering_value = (self.steering_value + self.new_steering_value)/2
            else:
                self.steering_value = self.new_steering_value

            if self.direction == Direction.Straight:
                if abs(self.slope_avg) < AVG_SLOPE_THRESHOLD_DIAG:
                    if self.slope_avg < 0:
                        self.direction = Direction.LeftDiag
                    else:
                        self.direction = Direction.RightDiag
            elif abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_STRAIGHT:
                self.direction = Direction.Straight
        
        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
            print("steering value:%.3f  dir:%s" % (self.steering_value, self.direction.name))

        return

    def process(self, mask, next_turn_direction: Direction):
        """ Top level method to process a new mask contents. This method calls 
        the find_limits methods for the left and right lane limits objects.
        It then checks the lane widths for each pair and corrects, if out of 
        expected bounds to clean up more. It then extracts a steering value 
        to keep the car in the lane or to perform a lane switch or a turn 
        left or right depening on the condition for the  next_turn_direction.
        mask                -- segmentation mask containing the SegmClass code.
        next_turn_direction -- direction code for the next upcoming turn. """

        # In a turn it makes no sense to try finding limits and objects
        if self.steering_fixed_count <= STEERING_FIXED_TURN_DIAG_CYCLES:
            self.get_lane_limits(mask)
            self.update_lane_center_classes(mask)
            self.process_classes(next_turn_direction)

        if self.stop_count == 0:
            self.handle_directions(mask, next_turn_direction)

        if jetcar_lane.DEBUG_MASK_IMG == True:
            mask_img = self.draw(jetcar_lane.DEBUG_MASK_IMG_REFERENCE.copy())
            cv2.imwrite(f'{jetcar_lane.DEBUG_MASK_IMG_FILE_NAME}_final.jpg',mask_img)

        return

    def draw(self, img):
        """ Draws its lane limit points, the approximation lines as well as
        center points and left lane and right lane points onto the 
        passed BGR image. Returns the resulting image """
        if self.steering_fixed_count < STEERING_FIXED_TURN_DIAG_CYCLES:
            img = self.lane_limits_left.draw(img)
            img = self.lane_limits_right.draw(img)

            for i in range(N_LANE_POINTS):
                img = self.lane_center.points[i].draw(img, (128, 128, 128))
                img = self.lane_left.points[i].draw(img, (64, 64, 128))
                img = self.lane_right.points[i].draw(img, (64, 128, 64))
        return img
