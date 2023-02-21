# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`jetcar_center`
===============
The center module handles a simple form of object tracking in the center of a lane 
based on the segmentation class codes sampled along the center line. The lane could 
be the one in the center in front of the car or a neighbor lane left or right or 
the space outside on either side, if there is no lane.

Following classes are implemented here:

class SegmClassPoint:
    Class for determining and storing the class code around a location. This class is 
    used to track codes along the center line as a first part of object tracking.

class SegmClassObj:
    Class to keep track of a street marking object.

class LaneCenter:
    Class to handle sampling the class codes in the center of a lane and tracking objects 
    along the center. It can be the lane the car is driving on or one of the neighbor lanes. 


* Author(s): Stefan Warnke

Dependencies:
jetcar_definitions.py
jetcar_lane.py
"""

from pickle import FALSE
from jetcar_definitions import *
from jetcar_lane import *
import cv2
import math
import time

# Set to true to enable debug print messages when determining the dominant center class codes
DEBUG_PRINT_GET_CENTER_CLASSES = False
# Set to true to enable debug print messages with the new object list from the codes
DEBUG_PRINT_GET_NEW_LIST = False
# Set to true to enable debug print messages with the final objects list contents
DEBUG_PRINT_UPDATE_OBJECTS_OBJECT_LIST = False

# Width of the window to determine the dominant class code for the center lane
CENTER_WINDOW_WIDTH = 7
# Height of the window to determine the dominant class code for the center lane
CENTER_WINDOW_HEIGHT = 5
# X-step size in the window to determine the dominant class code for the center lane
CENTER_STEP_SIZE_X = 1
# Y-step size in the window to determine the dominant class code for the center lane
CENTER_STEP_SIZE_Y = 1


# Width of the window to determine the dominant class code for the left or right neighbor lane
NEIGHBOR_WINDOW_WIDTH = 5
# Height of the window to determine the dominant class code for the left or right neighbor lane
NEIGHBOR_WINDOW_HEIGHT = 3
# X-step size in the window to determine the dominant class code for the left or right neighbor lane
NEIGHBOR_STEP_SIZE_X = 1
# Y-step size in the window to determine the dominant class code for the left or right neighbor lane
NEIGHBOR_STEP_SIZE_Y = 1

# Time-To-Live definition for keeping object in the list, when not updated
CLASS_CODE_OBJ_TTL = 3
# Time-To-Live definition for keeping intersection object in the list, when not updated
INTERSECTION_OBJ_TTL = 20
# Minimum number of cycles for an object class code to be alive before it will be recognized as valid
CLASS_CODE_OBJ_MIN_ALIVE = 4
# Minimum size for an object class code to be recognized as valid
CLASS_CODE_OBJ_MIN_SIZE = 5
# Object distance from ORIGIN to switch from update length to moving the object location
CLASS_CODE_OBJ_ORIGIN_DIST = 10

#============================================================================================

class SegmClassPoint:
    """ Class for determining and storing the class code around a location.
        This class is used to track codes along the center line as a first part
        of object tracking."""

    def __init__(self, window_width, window_height, step_size_x, step_size_y):
        """Initializes the ClassCodePoint object with default values.
        window_width    -- Width of the window to determine the dominant class code.
        window_height   -- Height of the window to determine the dominant class code.  """
        self.step_size_x = step_size_x
        self.step_size_y = step_size_y
        self.dx = step_size_x * (window_width//2)
        self.dy = step_size_y * (window_height//2)
        self.N = window_width * window_height
        self.location = Point(IMG_XC,IMG_YC)
        self.code = 0
        self.score = 0
        return


    def get_dominant_class(self, mask):
        """Determines the class code with the highest occurance around the location.
        Codes are counted in the 3x3 windows and the maximum count and code returned.
        mask        -- segmentation mask containing the SegmClass code."""
        code = [-1 for _ in range(self.N)]
        count = [0 for _ in range(self.N)]
        last_idx = 0
        # Copy all codes from window into linear array
        for yy in range(self.location.y - self.dy, self.location.y +  self.dy +1, self.step_size_y):
            if yy <= IMG_YMAX and yy >= 0:
                for xx in range(self.location.x - self.dx, self.location.x + self.dx +1, self.step_size_x):
                    if xx <= IMG_XMAX and xx >= 0:
                        code[last_idx] = mask[yy, xx]
                        last_idx += 1         
        # Go through and count, mark already counted with -1
        for i in range(last_idx):
            c = code[i]
            if c>=0:
                for j in range(i,last_idx):
                    if c == code[j]:
                        count[i] += 1
                        if i != j:
                            code[j] = -1
        # Look for the maximum count
        max_count = 0
        max_idx = 0
        for i in range(last_idx):
            if code[i]>=0 and count[i] > max_count:
                max_count = count[i]
                max_idx = i

        # return the code at maximum count and the normalized score
        return code[max_idx], max_count/last_idx


    def update_location(self, mask, point):
        """Updates location and passes the mask to determine the most occurring class code around it.
        mask        -- segmentation mask containing the SegmClass code.
        point       -- new location point to store """
        self.location = point
        self.code, self.score = self.get_dominant_class(mask)
        return


    def update_midpoint(self, mask, point1, point2):
        """Updates location as midpoint between the 2 points and passes a mask to determine the most occurring code around it.
        mask        -- segmentation mask containing the SegmClass code.
        point1      -- first point for the midpoint calculation
        point2      -- second point for the midpoint calculation """
        self.location.set_mid(point1,point2)
        self.code, self.score = self.get_dominant_class(mask)
        return


    def __str__(self):
        """ Returns a string representation of the ClassCodePoint object """
        return f'{self.code}:{SegmClass(self.code).name}-{self.score:.1f}'

#============================================================================================

class SegmClassObj:
    """ Class to keep track of a street marking object."""
    def __init__(self, class_point:SegmClassPoint, origin:Point):
        """Initializes the object with the parameters.
        code        -- class code to track.
        point       -- initial location."""
        self.code = class_point.code
        self.location = class_point.location.copy()
        self.endpoint = class_point.location.copy()
        self.origin = origin.copy()
        self.size = 0
        self.distance = origin.get_distance(self.location)
        self.ttl = CLASS_CODE_OBJ_TTL
        self.alive = 0
        return

    def update_location(self, point:Point):
        """ Updates the location of the object and its distance to ORIGIN
        point       -- new location point of this object"""
        self.location = point.copy()
        self.distance = self.origin.get_distance(self.location)
        self.ttl = CLASS_CODE_OBJ_TTL
        return

    def update_endpoint(self, point:Point, move_enabled):
        """ Update the endpoint and size of the tracked object unless it will be moved. 
        Normally, an object in the list just has to be updated and can be deleted when out of sight.
        However, direction arrows and stop marking in the current lane needs to be kept until 
        intersection is passed. These objects will be just moved behind the image border and kept there.
        point           -- new end point location
        move_enabled    -- if True and object location is at ORIGIN, 
        the size will be kept constant and the location will be moved together with the endpoint."""
        self.endpoint = point.copy()
        if move_enabled == False or self.distance > CLASS_CODE_OBJ_ORIGIN_DIST:
            self.size = self.location.get_distance(self.endpoint)        
        else:
            self.location = Point(self.endpoint.x,self.endpoint.y+self.size)
            self.distance = self.origin.get_distance(self.location)
            if self.location.y > IMG_YMAX:
                self.distance = -self.distance
        return

    def update(self, new_obj:"SegmClassObj"):
        """ General update method to update location or end point. It is assumed, 
        that the car motion will make an object appear closer each time and the location 
        will have to be updated. Otherwise, it will have to be moved 
        new_obj         -- object reference to update from. """
        if new_obj.distance < self.distance:
            self.location = new_obj.location.copy()
            self.distance = new_obj.distance 
                      
        self.update_endpoint(new_obj.endpoint, True)  
        self.ttl = CLASS_CODE_OBJ_TTL 
        return

    def __str__(self):
        """ Returns a string representation of the ClassCodeObj object """
        #s = f'{SegmClass(self.code).name}'
        return f'{self.distance}:{self.size}:{SegmClass(self.code).name}:{self.alive}:{self.ttl}'

#============================================================================================
N_CENTER_CLASS_POINTS = 2*N_LANE_POINTS+1
#N_CENTER_CLASS_POINTS = N_LANE_POINTS+2

class LaneCenter:
    """ Class to handle sampling the class codes in the center of a lane and tracking objects 
    along the center. It can be the lane the car is driving on or one of the neighbor lanes. """
    def __init__(self, side:Side, init_obj:LaneLimits):
        """ Initializes the object with the parameters
        side            -- Defines if it is the center lane or left/right neighbor
        init_obj        -- Array of points in the lane center to use y-coordinates from"""
        self.side = side
        self.origin = Point(IMG_XC + side.value*(ORIGIN_LANE_WIDTH-2),IMG_YMAX)
        self.points = [Point(IMG_XC, init_obj.points[i].y) for i in range(len(init_obj.points))]
        if side == Side.Center:
            w = CENTER_WINDOW_WIDTH
            h = CENTER_WINDOW_HEIGHT
            sx = CENTER_STEP_SIZE_X
            sy = CENTER_STEP_SIZE_Y
        else:
            w = NEIGHBOR_WINDOW_WIDTH
            h = NEIGHBOR_WINDOW_HEIGHT
            sx = NEIGHBOR_STEP_SIZE_X
            sy = NEIGHBOR_STEP_SIZE_Y
        self.codes = [SegmClassPoint(w, h, sx, sy) for i in range(N_CENTER_CLASS_POINTS)]
        self.init_object_list()
        return

    def init_object_list(self):
        """Init object list with an empty list object"""
        self.objects = []
        return

    def get_center_classes(self, mask):
        """Determine the class codes along the center points.
        mask        -- segmentation mask containing the SegmClass code."""
        self.codes[0].update_location(mask, self.origin)
        self.codes[1].update_midpoint(mask, self.origin, self.points[0])
        # Go through the points to update the codes objects, interpolate in between
        if N_CENTER_CLASS_POINTS > 2*N_LANE_POINTS:
            for i in range(N_LANE_POINTS):
                ii = 2*i+2
                self.codes[ii].update_location(mask, self.points[i])
                if i < N_LANE_POINTS-1:
                    ii += 1
                    self.codes[ii].update_midpoint(mask, self.points[i], self.points[i+1])
        else:
           for i in range(N_LANE_POINTS):
                ii = i+2
                self.codes[ii].update_location(mask, self.points[i])
 
        if DEBUG_PRINT_GET_CENTER_CLASSES == True:
            sc = f'{self.side}  codes:  '
            for i in range(len(self.codes)):
                sc += f'{self.codes[i]}  '
            print(sc)       

        return

    def check_ttls(self):
        """ Decrement the TTL counters of all current objects and remove expired ones.
        All objects in front of the car will be removed when their time-to-live is expired.
        Intersection objects in the current center lane not in front anymore will be kept 
        until the intersection is entered or passed."""
        for i in reversed(range(len(self.objects))):
            self.objects[i].ttl -= 1
            if self.objects[i].ttl <= 0:
                if self.side == Side.Center:
                    if self.objects[i].distance > CLASS_CODE_OBJ_ORIGIN_DIST:  
                        self.objects.pop(i)
                    elif self.objects[i].code not in INTERSECTION_OBJECT_CODES or \
                         self.objects[i].size == 0 or self.objects[i].ttl < -INTERSECTION_OBJ_TTL:
                        self.objects.pop(i)
                else:
                    self.objects.pop(i)
            else:
                self.objects[i].alive += 1
        return


    def get_new_list(self):
        """ Create a new list of objects from the class codes sampled in 
        get_center_classes(). Objects are created when a class code other 
        than lane_my_dir is found. It is then tracked to the end to get the 
        size information. 
        returns     -- The new object list created from the current class codes."""
        current_obj = None
        new_list = []
        # Track any object along the center line and compress into new list entry
        for i in range(len(self.codes)):
            if current_obj == None:
                if self.codes[i].code != LANE_MY_DRIVING_DIR_CODE:
                    current_obj = SegmClassObj(self.codes[i], self.origin)
                    new_list.append(current_obj)
            else:
                if current_obj.code != self.codes[i].code:
                    current_obj.update_endpoint(self.codes[i].location, False)
                    # The new code might be a new object starting
                    if self.codes[i].code != LANE_MY_DRIVING_DIR_CODE:
                        current_obj = SegmClassObj(self.codes[i], self.origin)
                        new_list.append(current_obj)
                    else:
                        current_obj = None

        # make sure the end point is updated, if the code goes to the last point
        if current_obj != None:
            current_obj.update_endpoint(self.codes[len(self.codes)-1].location, False)

        if DEBUG_PRINT_GET_NEW_LIST == True:
            s = f'{self.side}   new: '
            for i in range(len(new_list)):
                s += f'{new_list[i]}  '
            print(s)
        return new_list


    def update_objects(self):
        """Top level method to call check_ttls() to remove expired objects from the list.
        It then calls get_new_list() to convert the array of code points to a new list of 
        objects. This new list is then used to update matching existing objects in 
        self.objects list or insert newly discovered objects into the correct position."""
        
        self.check_ttls()
        new_list = self.get_new_list()

        idx=0
        # 
        for i in range(len(new_list)):
            handled = False
            while handled == False:
                if idx < len(self.objects):
                    if new_list[i].code == self.objects[idx].code:
                        self.objects[idx].update(new_list[i])
                        handled = True
                    elif new_list[i].distance < self.objects[idx].distance:
                        self.objects.insert(idx, new_list[i])
                        handled = True
                    idx += 1
                else:
                    self.objects.append(new_list[i])
                    handled = True
 
        if DEBUG_PRINT_UPDATE_OBJECTS_OBJECT_LIST == True:
            s = f'{self.side.name} objects: '
            for i in range(len(self.objects)):
                s += f'{self.objects[i]}  '
            print(s)
        return


    def remove_intersection_objects(self):
        """ Remove intersection related objects from the objects list """
        for i in reversed(range(len(self.objects))):
            if self.objects[i].ttl <= 0 and self.objects[i].distance < CLASS_CODE_OBJ_ORIGIN_DIST and \
               self.objects[i].code in INTERSECTION_OBJECT_CODES:
                self.objects.pop(i)
        return


    def get_dominant_arrow_code(self):
        """ Since there can be multiple false arrow codes in the current mask, 
        identify the one with the most occurance and longest live time to return as dominant"""
        sizes =  [0 for i in range(len(STREET_ARROW_CODES))]
        alives =  [0 for i in range(len(STREET_ARROW_CODES))]
        n = 0
        # Go through the object list to check for arrow codes
        for i in range(len(self.objects)): 
            if self.objects[i].code in STREET_ARROW_CODES:
                # Accumulate size and alive values individually
                idx = STREET_ARROW_CODES.index(self.objects[i].code)
                sizes[idx] += self.objects[i].size
                alives[idx] += self.objects[i].alive
                n += 1

        if n == 0:
            # Nothing found, return nothing
            return NOTHING_CODE
        elif n == 1:
            #only one found, nothing further to do 
            return STREET_ARROW_CODES[idx]

        max_prod = 0
        # When multiple arrow codes are present, identify the dominant one
        for i in range(len(STREET_ARROW_CODES)):
            prod = sizes[i]*alives[i]
            if prod > max_prod:
                max_prod = prod
                idx = i
        # return dominant code
        return STREET_ARROW_CODES[idx]
        

    def draw_code_points(self, img):
        """ Method to draw all code location points 
        onto the passed input image and return the resulting image.
        img     -- image bitmap to draw to."""

        # define a color depending on left, center or right side
        if self.side == Side.Center:
            bgr = (128,128,128)
        elif self.side == Side.Left:
            bgr = (64, 64, 128)
        else:
            bgr = (64, 128, 64)

        # draw all individual search points of the vector
        for i in range(len(self.codes)):
            img = self.codes[i].location.draw(img, bgr)

        #return the resulting image
        return img

#============================================================================================

