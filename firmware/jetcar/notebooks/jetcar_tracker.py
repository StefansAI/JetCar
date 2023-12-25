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
THROTTLE_VALUE_APPROACHING_INTERSECTION = 0.9
# Throttle value when entering the inner intersection area
THROTTLE_VALUE_ENTERING_INTERSECTION = 0.8
# Throttle value to be used in a turn of an intersection
THROTTLE_VALUE_IN_TURN = 0.95
# Throttle value to be used in a curve outside intersections
THROTTLE_VALUE_IN_CURVE = 1.1
# Steering value threshold for reducing the speed to THROTTLE_VALUE_IN_CURVE
THROTTLE_CURVE_STEERING_THRESHOLD = 0.5
# Throttle value after a stop to overcome friction and get going
THROTTLE_VALUE_AFTER_STOP = 1.0
# Filter coefficient to low pass new throttle value with old one
THROTTLE_VALUE_FILTER_COEFF = 0.25
# Number of cycles to keep the current steering value when turning left
STEERING_FIXED_TURN_CYCLES_LEFT = 20
# Number of cycles to keep the current steering value when turning right
STEERING_FIXED_TURN_CYCLES_RIGHT = 20
# Number of cycles to keep the current steering value when turning right on offramp
STEERING_FIXED_TURN_CYCLES_RIGHT_OFFRAMP = 15
# Cycle count in fixed turn to switch to diagonal search when turning left
STEERING_FIXED_TURN_DIAG_CYCLES_LEFT = STEERING_FIXED_TURN_CYCLES_LEFT - 8
# Cycle count in fixed turn to switch to diagonal search when turning right
STEERING_FIXED_TURN_DIAG_CYCLES_RIGHT = STEERING_FIXED_TURN_CYCLES_RIGHT - 6
# Cycle count in fixed turn to switch to diagonal search when turning right
STEERING_FIXED_TURN_DIAG_CYCLES_RIGHT_OFFRAMP = STEERING_FIXED_TURN_CYCLES_RIGHT_OFFRAMP - 6
# Threshold to separate off ramp from sharp right turn
RIGHT_OFFRAMP_SLOPE_THRESHOLD = 1.0
# Fixed steering value used for left turn, left turn can be wider
STEERING_LEFT_TURN_VALUE = -1
# Fixed steering value used for right turn, right turn must be tight
STEERING_RIGHT_TURN_VALUE = 1.0
# Fixed steering value used for right turn when going on off-ramp
STEERING_RIGHT_TURN_VALUE_OFFRAMP = 0.7
# Minimum size of left turn code object
MIN_TURN_LEFT_OBJ_SIZE = 30
# Minimum size of right turn code object
MIN_TURN_RIGHT_OBJ_SIZE = 40
# Minimum distance of left turn code to ORIGIN to start turning left
MIN_TURN_LEFT_DISTANCE = 60
# Minimum distance of right turn code to ORIGIN to start turning right
MIN_TURN_RIGHT_DISTANCE = 30
# Minimum distance of right turn code to ORIGIN to start turning right into an offramp
MIN_TURN_RIGHT_DISTANCE_OFFRAMP = 65
# Minimum distance of left turn code to ORIGIN to recognize an intersection
MIN_INTERSECTION_LEFT_TURN_DISTANCE = 120
# Minimum distance of right turn code to ORIGIN to recognize an intersection
MIN_INTERSECTION_RIGHT_TURN_DISTANCE = 80
# Minimum distance of left turn code to ORIGIN to recognize an intersection
MIN_INTERSECTION_LEFT_TURN_SIZE = 10
# Minimum distance of right turn code to ORIGIN to recognize an intersection
MIN_INTERSECTION_RIGHT_TURN_SIZE = 15
# Minimum limit found point count to check for switching back to straight after turn
MINIMUM_LIMIT_FOUND_POINTS = (2*N_SEARCH_POINTS)//3
# A threshold to smooth out small steering changes with a low pass
STEERING_FILTER_THRESHOLD = 0.05
# An average slope threshold for intersection detection to see if there is "Nothing" ahead
AVG_SLOPE_THRESHOLD_INTERSECTION = 5
# Switch back to straight direction when new average slope value goes above after turn
AVG_SLOPE_THRESHOLD_STRAIGHT = 1.5
# Switch to diagonal direction when new verage slope falls below
AVG_SLOPE_THRESHOLD_DIAG = 0.9
# Standard deviation threshold for allowing a left or right lane limit correction
MAX_STDDEV_LR_CORRECTION_VALID = 10
# Standard deviation threshold for allowing a center lane limit correction
MAX_STDDEV_CENTER_CORRECTION_VALID = 3
# Standard deviation threshold for allowing to switch from fixed steering to steering control
MAX_STDDEV_STEERING_VALID = 3
# Minimum distance to recognize stop text, yield text as valid for intersection approach
TEXT_MIN_ORIGIN_DIST_APPROACH = 10*CLASS_CODE_OBJ_ORIGIN_DIST
# Minimum distance to recognize stop text, yield text as valid for intersection enter
TEXT_MIN_ORIGIN_DIST_ENTER = 5*CLASS_CODE_OBJ_ORIGIN_DIST
# Minimum size to recognize stop text, yield text as valid
TEXT_MIN_ORIGIN_SIZE = 2*CLASS_CODE_OBJ_MIN_SIZE

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
        self.steering_fixed_count_diag_cycles = STEERING_FIXED_TURN_DIAG_CYCLES_RIGHT
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
        self.force_initxy = False

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
        self.lane_limits_left.find_limits(mask, self.direction, self.force_initxy)
        if jetcar_lane.DEBUG_PRINT_FIND_LIMITS == True:
            print("find limits right:")
        self.lane_limits_right.find_limits(mask, self.direction, self.force_initxy)

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
                        (self.lane_limits_left.stddev < MAX_STDDEV_LR_CORRECTION_VALID)

        if correct_right == True:
            if DEBUG_PRINT_GET_LANE_LIMITS == True:
                print("correct right:")
        else:
            correct_left = (self.lane_limits_left.limit_found_count <= self.lane_limits_right.limit_found_count) and \
                           (self.lane_limits_left.stddev > 2*self.lane_limits_right.stddev) and \
                           (self.lane_limits_right.stddev < MAX_STDDEV_LR_CORRECTION_VALID)
            if correct_left == True:
                if DEBUG_PRINT_GET_LANE_LIMITS == True:
                    print("correct left:")
            else:
                correct_center = (self.lane_limits_left.stddev > MAX_STDDEV_CENTER_CORRECTION_VALID) and \
                                 (self.lane_limits_left.stddev > MAX_STDDEV_CENTER_CORRECTION_VALID)

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

        group_move_avg = (self.lane_left.move_avg + self.lane_center.move_avg + self.lane_right.move_avg)//3

        self.lane_left.update_objects(group_move_avg)
        self.lane_center.update_objects(group_move_avg)
        self.lane_right.update_objects(group_move_avg)

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
                    print("i=%d code: %d:%s in DEFINITIONS_DIRECTIONS, allowed: left=%s straight=%s right=%s" % \
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
        self.lane_left.clear_expired_turns()
        self.lane_center.clear_expired_turns()
        self.lane_right.clear_expired_turns()       
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
            print("process_classes direction=%s  next_turn_direction=%s  stop_count=%d " % (Direction(self.direction).name, Direction(next_turn_direction).name, self.stop_count))

        # First, handle stop time by counting down the stop_count timer and returning without doing anything else
        if self.stop_count > 0:
            self.stop_count -= 1
            if self.stop_count > 0:
                self.throttle_value = 0
            else:
                self.stop_detected = False
                self.lane_left.remove_intersection_objects()
                self.lane_center.remove_intersection_objects()
                self.lane_right.remove_intersection_objects()
                self.throttle_value = THROTTLE_VALUE_AFTER_STOP
            return
 
        # In the case of going straight through an intersection, check if any turns had been passed and are expired so we can init for next intersection
        if self.intersection == True and self.direction == Direction.Straight and \
           self.lane_left.next_turn_lane_obj == None and self.lane_right.next_turn_lane_obj == None and \
           (len(self.lane_left.expired_turns) > 0 or len(self.lane_right.expired_turns) > 0):
            self.init_for_next_intersection()

        # Reset intersection to re-assess in this cycle
        self.intersection = False
        
        # Check for any arrow markings in our lane and enable possible directions accordingly, slow down
        if self.lane_center.dominant_arrow_code != None:
            self.set_direction_enables(self.lane_center.dominant_arrow_code)
            self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
            self.intersection = True
            if DEBUG_PRINT_PROCESS_CLASSES == True:
                print("dominant arrow code: %d:%s" % (self.lane_center.dominant_arrow_code, SegmClass(self.lane_center.dominant_arrow_code).name))

        # If there is any stop text coming up in range, slow down and prepare to stop
        text_obj = self.lane_center.stop_yield_text_obj
        if text_obj != None and text_obj.code == SegmClass.stop_text.value and text_obj.distance < TEXT_MIN_ORIGIN_DIST_APPROACH and \
           text_obj.size >= TEXT_MIN_ORIGIN_SIZE:
            self.stop_detected = True
            self.intersection = True

            if DEBUG_PRINT_PROCESS_CLASSES == True:
                print("lane_center.stop_yield_text_obj: %d:%s size=%d alive=%d dist=%d" % (text_obj.code, SegmClass(text_obj.code).name, text_obj.size, text_obj.alive, text_obj.distance))

            if text_obj.distance > TEXT_MIN_ORIGIN_DIST_ENTER:
                self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
            elif (text_obj.distance + text_obj.size) >= -10:
                self.throttle_new_value = THROTTLE_VALUE_ENTERING_INTERSECTION
            else:
                self.throttle_new_value = 0
                self.throttle_value = 0
                self.stop_count = THROTTLE_STOP_CYCLES
                self.brake_light_enable = True
                if DEBUG_PRINT_PROCESS_CLASSES == True:
                    print("STOP!")
                return

        # If the stop line is close enough, stop now, set the counter and return
        line_obj = self.lane_center.stop_yield_line_obj
        if line_obj != None and line_obj.code == SegmClass.stop_line.value and line_obj.distance <= CLASS_CODE_OBJ_ORIGIN_DIST and \
            self.stop_detected == True and self.stop_detected == True:
            self.throttle_new_value = 0
            self.throttle_value = 0
            self.stop_count = THROTTLE_STOP_CYCLES
            self.brake_light_enable = True
            if DEBUG_PRINT_PROCESS_CLASSES == True:
                print("lane_center.stop_yield_line_obj: %d:%s size=%d alive=%d dist=%d" % (line_obj.code, SegmClass(line_obj.code).name, line_obj.size, line_obj.alive, line_obj.distance))
                print("STOP!")
            return


        # If there were no arrows or other signs indicating an intersection and the car is not in a tight curve, check for left and right turns
        if self.intersection == False and abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_INTERSECTION:
            # Check left lane for a turn lane
            lturn_obj = self.lane_left.next_turn_lane_obj
            if lturn_obj != None and lturn_obj.distance < MIN_INTERSECTION_LEFT_TURN_DISTANCE and lturn_obj.size > MIN_INTERSECTION_LEFT_TURN_SIZE:
                self.intersection = True
                self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
                if DEBUG_PRINT_PROCESS_CLASSES == True:
                    print("lane_left.next_turn_lane_obj size=%d alive=%d dist=%d" % (lturn_obj.size,lturn_obj.alive,lturn_obj.distance))

            # Check right lane for a turn lane
            rturn_obj = self.lane_right.next_turn_lane_obj
            if rturn_obj != None and rturn_obj.distance < MIN_INTERSECTION_RIGHT_TURN_DISTANCE and rturn_obj.size > MIN_INTERSECTION_RIGHT_TURN_SIZE:
                self.intersection = True
                self.throttle_new_value = THROTTLE_VALUE_APPROACHING_INTERSECTION
                if DEBUG_PRINT_PROCESS_CLASSES == True:
                    print("lane_right.next_turn_lane_obj size=%d alive=%d dist=%d" % (rturn_obj.size,rturn_obj.alive,rturn_obj.distance))

            # If there were no arrows, but there is an intersection, check if straight forward is allowed
            # To avoid false positives, check the last 2 codes in each lane
            if self.intersection == True and self.go_straight_allowed == True and \
               self.lane_center.codes[N_CENTER_CLASS_POINTS-1] in STRAIGHT_BLOCK_CODES and \
               self.lane_center.codes[N_CENTER_CLASS_POINTS-2] in STRAIGHT_BLOCK_CODES and \
               self.lane_left.codes[N_CENTER_CLASS_POINTS-1] in STRAIGHT_BLOCK_CODES and \
               self.lane_left.codes[N_CENTER_CLASS_POINTS-2] in STRAIGHT_BLOCK_CODES and \
               self.lane_right.codes[N_CENTER_CLASS_POINTS-1] in STRAIGHT_BLOCK_CODES and \
               self.lane_right.codes[N_CENTER_CLASS_POINTS-2] in STRAIGHT_BLOCK_CODES:
                
                self.go_straight_allowed = False

                if DEBUG_PRINT_PROCESS_CLASSES == True and self.go_straight_allowed == False:
                    print("Straight not allowed! codes: left=%d:%s center=%d:%s right=%d:%s" % (
                          self.lane_left.objects[-1].code, SegmClass(self.lane_left.objects[-1].code).name,
                          self.lane_center.objects[-1].code, SegmClass(self.lane_center.objects[-1].code).name,
                          self.lane_right.objects[-1].code, SegmClass(self.lane_right.objects[-1].code).name))

        # Found intersection conditions
        if self.intersection == True:
            # In the special case that straight forward is requested but it is not allowed, just stop and wait
            if next_turn_direction == Direction.Straight and self.go_straight_allowed == False:
                if (self.lane_left.next_turn_lane_obj != None and self.lane_left.next_turn_lane_obj.distance <= MIN_TURN_LEFT_DISTANCE and self.lane_left.next_turn_lane_obj.size >= MIN_TURN_LEFT_OBJ_SIZE) or \
                   (self.lane_right.next_turn_lane_obj != None and self.lane_right.next_turn_lane_obj.distance <= MIN_TURN_RIGHT_DISTANCE and self.lane_right.next_turn_lane_obj.size >= MIN_TURN_RIGHT_OBJ_SIZE):
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

            # Set turn signal enables as in emergancy with all 4 turn signals to get the operators attention: "I cannot go where you want me to go"
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

    def get_new_steering_value(self, left=True, right=True):
        """Calculate a new steering value by averaging the point offsets from the center"""
        if left == False and right == False:
            return
        
        lsum = 0
        rsum = 0
        # sum up left and right deltas to center
        for i in range(len(STEERING_Y)):
            lsum += self.lane_limits_left.get_x(STEERING_Y[i])-IMG_XC
            rsum += self.lane_limits_right.get_x(STEERING_Y[i])-IMG_XC

        if left == False:
            lsum = rsum - len(STEERING_Y)*ORIGIN_LANE_WIDTH
        elif right == False:
            rsum = lsum + len(STEERING_Y)*ORIGIN_LANE_WIDTH

        # Scale the average between left and right deltas to new steering value
        self.new_steering_value = min(max((lsum + rsum)/STEERING_X_SCALE,-1),1) 
        return

    def update_steering(self):
        """Filter and update steering_value from new_steering_value"""
        # If the difference between current and new value is small, average both to make it smoother
        if abs(self.new_steering_value-self.steering_value) < STEERING_FILTER_THRESHOLD:
            self.steering_value = (self.steering_value + self.new_steering_value)/2
        else:
            self.steering_value = self.new_steering_value
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
                
                # accelerate slowly in the turn to pull through
                if self.throttle_new_value < THROTTLE_VALUE_IN_CURVE:
                    self.throttle_new_value += 0.025

                # While turning, switch to diagonal search at half way point
                if self.steering_fixed_count == self.steering_fixed_count_diag_cycles:
                    if self.direction == Direction.Left:
                        self.direction = Direction.LeftDiag
                    elif self.direction == Direction.Right:
                        self.direction = Direction.RightDiag
                    self.throttle_new_value = THROTTLE_VALUE_IN_TURN
                    self.force_initxy = True

                # If 0 is reached, switch back to straight directly
                elif self.steering_fixed_count == 0:
                    self.init_for_next_intersection()
                    self.direction = Direction.Straight

                # To avoid overshooting, start checking for early switching to straight
                elif self.steering_fixed_count < self.steering_fixed_count_diag_cycles:
                    left_limit_ok = self.lane_limits_left.limit_found_count > MINIMUM_LIMIT_FOUND_POINTS and self.lane_limits_left.stddev < MAX_STDDEV_STEERING_VALID
                    right_limit_ok = self.lane_limits_right.limit_found_count > MINIMUM_LIMIT_FOUND_POINTS and self.lane_limits_right.stddev < MAX_STDDEV_STEERING_VALID
                    if left_limit_ok == True or right_limit_ok == True:
                        # There are enough valid points to base a decision on, so check current steering
                        self.get_new_steering_value(left_limit_ok, right_limit_ok)
                        self.update_steering()

                        if left_limit_ok == False:
                            self.slope_avg = self.lane_limits_right.slope
                        elif right_limit_ok == False:
                            self.slope_avg = self.lane_limits_left.slope
                        else:
                            self.force_initxy = False

                        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                            print("now steering: count=%d  left_limit_ok=%d  left_limit_found_count=%d left_stddev=%.3f  right_limit_ok=%d  right_limit_found_count=%s  right_stddev=%.3f  left_slope=%.3f right_slope=%.3f slope_avg=%.3f" % \
                                (self.steering_fixed_count, left_limit_ok, self.lane_limits_left.limit_found_count, self.lane_limits_left.stddev, right_limit_ok, self.lane_limits_right.limit_found_count, self.lane_limits_right.stddev, self.lane_limits_left.slope, self.lane_limits_right.slope, self.slope_avg))

                        if abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_STRAIGHT:
                            self.init_for_next_intersection()
                            self.direction = Direction.Straight
                            self.steering_fixed_count = 0
                            self.force_initxy = False

                    elif DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                        print("still fixed: count=%d  left_limit_found_count=%d left_stddev=%.3f  right_limit_found_count=%s  right_stddev=%.3f " % \
                            (self.steering_fixed_count, self.lane_limits_left.limit_found_count, self.lane_limits_left.stddev, self.lane_limits_right.limit_found_count, self.lane_limits_right.stddev))

                self.update_throttle()

            if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                print("keep turning: count=%d  new_steer=%.3f steer=%.3f  dir=%s  new_throttle=%.3f throttle_value=%.3f " % \
                    (self.steering_fixed_count, self.new_steering_value, self.steering_value, self.direction.name, self.throttle_new_value, self.throttle_value))

            return

        # If a turn is requested, check for the conditions to initiate the turn
        if self.direction != next_turn_direction and self.intersection == True and self.stop_detected == False:
            if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                print("current direction(%s) != next_turn_direction(%s)" % (self.direction.name, next_turn_direction.name))

            # Check for left turn request
            if next_turn_direction == Direction.Left and self.turn_left_allowed == True:
                self.steering_control = False
                self.signal_left_enable = True
                if self.lane_left.next_turn_lane_obj != None and self.lane_left.next_turn_lane_obj.distance <= MIN_TURN_LEFT_DISTANCE and \
                   self.lane_left.next_turn_lane_obj.size >= MIN_TURN_LEFT_OBJ_SIZE:
                    # initiate turn, if enough points indicate a left turn lane
                    self.direction = next_turn_direction
                    self.steering_value = STEERING_LEFT_TURN_VALUE
                    self.steering_fixed_count = STEERING_FIXED_TURN_CYCLES_LEFT
                    self.steering_fixed_count_diag_cycles = STEERING_FIXED_TURN_DIAG_CYCLES_LEFT
                    self.init_object_lists()
                    if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                        print("initiate turn left")

            # Check for right turn
            elif next_turn_direction == Direction.Right and self.turn_right_allowed == True:
                self.steering_control = False
                self.signal_right_enable = True
                if self.lane_right.next_turn_lane_obj != None and self.lane_right.next_turn_lane_obj.distance < MIN_TURN_RIGHT_DISTANCE_OFFRAMP  and \
                   self.lane_right.next_turn_lane_obj.size >= MIN_TURN_RIGHT_OBJ_SIZE:
                    # initiate turn, if enough points indicate a right turn lane
                    self.direction = next_turn_direction
                    if jetcar_lane.DEBUG_MASK_IMG == True:
                        dbg_img = jetcar_lane.DEBUG_MASK_IMG_REFERENCE.copy()

                    # there are 2 possibilities: a sharp right turn at intersection and a shallow turn at an off-ramp
                    offramp = False
                    p0 = Point(0,0)
                    p1 = Point(0,0)
                    n0 = 0
                    n1 = 0
                    scp = SegmClassPoint(NEIGHBOR_WINDOW_WIDTH, NEIGHBOR_WINDOW_HEIGHT, NEIGHBOR_STEP_SIZE_X, NEIGHBOR_STEP_SIZE_Y)
                    # calculate the x and y sums for the lane right points and something in between lane limits and lane right points
                    for i in range(len(self.lane_right.codes)):                       
                        if self.lane_right.codes[i].location.y > self.lane_right.next_turn_lane_obj.endpoint.y:
                            scp.location.x = (self.lane_center.codes[i].location.x + 2*self.lane_right.codes[i].location.x)//3
                            scp.location.y = (self.lane_center.codes[i].location.y + 2*self.lane_right.codes[i].location.y)//3
                            scp_code,scp_score = scp.get_dominant_class(mask)

                            if scp_code == LANE_RIGHT_TURN_CODE:
                                p0.x += scp.location.x
                                p0.y += scp.location.y
                                n0 += 1
                                if jetcar_lane.DEBUG_MASK_IMG == True:
                                    dbg_img = scp.location.draw(dbg_img, (255,64,64))

                            if self.lane_right.codes[i].code == LANE_RIGHT_TURN_CODE:
                                p1.x += self.lane_right.codes[i].location.x
                                p1.y += self.lane_right.codes[i].location.y
                                n1 += 1
                                if jetcar_lane.DEBUG_MASK_IMG == True:
                                    dbg_img = self.lane_right.codes[i].location.draw(dbg_img, (64,64,255))

                    # now calculate the average coordinates from these sums and the slope of the averages
                    if n0 > 0 and n1 > 0:
                        p0.x /= n0
                        p0.y /= n0
                        p1.x /= n1
                        p1.y /= n1
                        slope = p0.get_slope(p1)
                        offramp = slope >= RIGHT_OFFRAMP_SLOPE_THRESHOLD    # a higher slope indicates an off-ramp
                        if jetcar_lane.DEBUG_MASK_IMG == True:
                            dbg_img = cv2.line(dbg_img, (int(p0.x), int(p0.y)), (int(p1.x), int(p1.y)), (128,128,128), 2) 
                            cv2.imwrite(f'{jetcar_lane.DEBUG_MASK_IMG_FILE_NAME}_4_r_turn.jpg',dbg_img)
                    else:
                        slope = 0
                 
                    # at the end set turn radius and foxed counts for off-ramp or sharp right turn
                    if offramp == True:
                        self.steering_value = STEERING_RIGHT_TURN_VALUE_OFFRAMP
                        self.steering_fixed_count = STEERING_FIXED_TURN_CYCLES_RIGHT_OFFRAMP
                        self.steering_fixed_count_diag_cycles = STEERING_FIXED_TURN_DIAG_CYCLES_RIGHT_OFFRAMP
                        self.init_object_lists()
                        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                            print("initiate turn right into off-ramp (slope=%.3f)" % (slope))

                    elif self.lane_right.next_turn_lane_obj.distance < MIN_TURN_RIGHT_DISTANCE:
                        self.steering_value = STEERING_RIGHT_TURN_VALUE
                        self.steering_fixed_count = STEERING_FIXED_TURN_CYCLES_RIGHT
                        self.steering_fixed_count_diag_cycles = STEERING_FIXED_TURN_DIAG_CYCLES_RIGHT
                        self.init_object_lists()
                        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                            print("initiate sharp turn right (dist:%d  slope=%.3f)" % (self.lane_right.next_turn_lane_obj.distance,slope))
                    elif DEBUG_PRINT_HANDLE_DIRECTIONS == True:
                        print("not ready to turn right (dist:%d  slope=%.3f)" % (self.lane_right.next_turn_lane_obj.distance,slope))

        # No turn is requested or currently possible, calculate steering value to keep car
        # in the center of the current lane
        if self.steering_fixed_count == 0 and self.throttle_value > 0:
            self.steering_control = True
            self.get_new_steering_value()
            self.update_steering()

            if self.direction == Direction.Straight:
                if abs(self.slope_avg) < AVG_SLOPE_THRESHOLD_DIAG:
                    if self.slope_avg < 0:
                        self.direction = Direction.LeftDiag
                    else:
                        self.direction = Direction.RightDiag
            elif abs(self.slope_avg) > AVG_SLOPE_THRESHOLD_STRAIGHT:
                self.direction = Direction.Straight
        
        if DEBUG_PRINT_HANDLE_DIRECTIONS == True:
            print("steering value=%.3f  dir=%s" % (self.steering_value, self.direction.name))

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
        if self.steering_fixed_count <= self.steering_fixed_count_diag_cycles:
            self.get_lane_limits(mask)
            self.update_lane_center_classes(mask)
            self.process_classes(next_turn_direction)

        if self.stop_count == 0:
            self.handle_directions(mask, next_turn_direction)

        if jetcar_lane.DEBUG_MASK_IMG == True:
            mask_img = self.draw(jetcar_lane.DEBUG_MASK_IMG_REFERENCE.copy())
            cv2.imwrite(f'{jetcar_lane.DEBUG_MASK_IMG_FILE_NAME}_5_final.jpg',mask_img)

        return

    def draw(self, img):
        """ Draws its lane limit points, the approximation lines as well as
        center points and left lane and right lane points onto the 
        passed BGR image. Returns the resulting image """
        if self.steering_fixed_count < self.steering_fixed_count_diag_cycles:
            img = self.lane_limits_left.draw(img)
            img = self.lane_limits_right.draw(img)

            for i in range(N_LANE_POINTS):
                img = self.lane_center.points[i].draw(img, (128, 128, 128))
                img = self.lane_left.points[i].draw(img, (64, 64, 128))
                img = self.lane_right.points[i].draw(img, (64, 128, 64))
        return img
