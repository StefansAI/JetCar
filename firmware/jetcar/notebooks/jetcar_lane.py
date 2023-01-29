# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`jetcar_lane`
===============
The lane module deals with finding the lane limits in front of the car and all 
supporting functionality

Following classes are implemented here:

class Point:
    Simple Point class for storing x, y coordinates and providing basic functionality

class LaneLimitPoint(Point):
    Extension of the Point class to provide more functionality in the 
    context of finding lane limit points. The LaneLimitPoint is part of a 
    vector. The index in the vector is related to an expected position range 
    in the image and an expected width range of the lane given by the optics 
    and camera possition.

class LaneLimits:
    LaneLimits holds an array of LaneLimitPoint objects, updated 
    through their find_limit method. This class also provides methods
    for calculating line approximation on the array and perform some
    sanity check on the results.

* Author(s): Stefan Warnke

Dependencies:
jetcar_definitions.py
"""

from jetcar_definitions import *
import cv2
import math
import time

# Number of points in the lane limit vectors
N_LANE_POINTS = 11
# Number of points in the lane limit vectors to be used for find limit searches
N_SEARCH_POINTS = 8

# Set to true to enable debug print messages when finding lane limit points
DEBUG_PRINT_FIND_LIMITS_POINTS = False
# Set to true to enable debug print messages from the line approximation check function
DEBUG_PRINT_LINE_APPROX_CHECK = False
# Set to true to enable debug print messages when finding limits on the lane level
DEBUG_PRINT_FIND_LIMITS_LANE = False
# Set to true to enable debug print messages while searching for the limits. Attention: Creates tons of logs !
DEBUG_PRINT_FIND_LIMITS_SEARCH = False
# Set to true to enable debug print messages from the line approximation calculation
DEBUG_PRINT_LINE_APPROX = False

# True when finding lane limits on the point level or on the lane level is True
# Don't change this definition, since it combines other definitions
DEBUG_PRINT_FIND_LIMITS = DEBUG_PRINT_FIND_LIMITS_POINTS or DEBUG_PRINT_FIND_LIMITS_LANE

# Set to true to enable writing mask images with lane limits to files for debugging
# This definition has to be used in connection with valid DEBUG_MASK_IMG_FILE_NAME and
# DEBUG_MASK_IMG_REFERENCE assignments
DEBUG_MASK_IMG = False
# When DEBUG_MASK_IMG is true, assign a file name for each mask image
DEBUG_MASK_IMG_FILE_NAME = None
# When DEBUG_MASK_IMG is true, assign the reference for each mask image
DEBUG_MASK_IMG_REFERENCE = None
# Set to true for all image sequences left and right, when DEBUG_MASK_IMG is true
DEBUG_MASK_IMG_LEFT_RIGHT = False

# Typical minimal width of a lane at the ORIGIN right in front of the car bumper
ORIGIN_LANE_WIDTH = 75
# A tolerance adder of the lane width at the ORIGIN right in front of the car bumper
ORIGIN_LANE_WIDTH_TOL = (2*ORIGIN_LANE_WIDTH)//3 # 30

# Minimum step size for spacing the grid points along the direction
GRID_MIN_STEP_SIZE = 10
# Offset from the ORIGIN to start with grid points along the direction
GRID_ORIGIN_OFFS = 10

# Definition of the factor for diagonal steps
DIAG_FACTOR = 1/math.sqrt(2)

# The initial width for searching for the lane limit is set to half the image width
INITIAL_SEARCH_WIDTH = IMG_XC
# Search width for tracking already identified lane limit changes 
UPDATE_SEARCH_WIDTH = 2*ORIGIN_LANE_WIDTH

# Distance threshold for line_approx_check() to replace a point with the line approximation result
APPROX_CHECK_THRESHOLD_FACT = 1/3
# Standard deviation threshold to re-initializes the lane limit search
INIT_XY_STDDEV_THRESHOLD = 6
# Minimum limit found threshold to re-initializes the lane limit search
INIT_XY_LIMIT_FOUND_THRESHOLD = N_SEARCH_POINTS//2

# Offset of the optical axis of the camera from the ORIGIN
OPTICAL_CENTER_Y_OFFS = 10
# Y-coordinate of the optical axis point in the image
OPTICAL_CENTER_Y = IMG_YMAX-OPTICAL_CENTER_Y_OFFS

#============================================================================================

class Point:
    """ Simple Point class for storing x, y coordinates and providing basic functionality"""
    def __init__(self, x:int, y:int):
        """ Initializes the Point object with the passed x,y coordinates. 
        x -- x coordinate of the point
        y -- y coordinate of the point"""
        self.x = x
        self.y = y
        return


    def __str__(self):
        """ Returns a string representation of the Point class """
        return f'P({self.x},{self.y})'


    def copy(self):
        """ Returns a new Point object instance with the same x/y coordinates"""
        return Point(self.x,self.y) 
   

    def draw(self, img, bgr):
        """ Draw method draws a small circle of color bgr onto the image img.
        img     -- image to draw on.
        bgr     -- reverse RGB color code for drawing
        return  -- image reference after drawing"""
        img = cv2.circle(img, (self.x, self.y), 2, bgr, 2)
        return img  


    def value(self, mask):
        """ Return the value of the mask at the point coordinates.
        mask        -- segmentation mask containing the SegmClass code.
        return      -- class code from mask at the point coordinates"""
        return mask[self.y,self.x]


    def get_distance_xy(self, x:int, y:int):
        """ Returns the distance between this point object coordinates and the 
        passed x,y coordinates. 
        x           -- x coordinate of the point to calculate distance to.
        y           -- y coordinate of the point to calculate distance to.
        return      -- distance between x/y coordinates and this object coordinates."""
        dx = x - self.x
        dy = y - self.y
        if dx == 0:
            return abs(dy)
        if dy == 0:
            return abs(dx)
        return int(math.sqrt(dx*dx + dy*dy))


    def get_distance(self, p:"Point"):
        """ Returns the distance between this point object coordinates and the 
        passed point object coordinates. 
        p           -- point coordinates to calculate distance to.
        return      -- distance between point coordinates and this object coordinates."""
        return self.get_distance_xy(p.x, p.y)
    

    def get_slope_xy(self, x, y):
        """ Returns the slope between this point object coordinates and the 
        passed x,y coordinates. 
        x           -- x coordinate of the point to calculate slope with.
        y           -- y coordinate of the point to calculate slope with.
        return      -- slope between x/y coordinates and this object coordinates."""
        dx = x - self.x
        dy = y - self.y
        if dx == 0:
            return 1
        if dy == 0:
            return 0
        return dy/dx


    def get_slope(self, p:"Point"):
        """ Returns the slope between this point object coordinates and the 
        passed point object coordinates. 
        p           -- point coordinates to calculate slope with.
        return      -- slope between point coordinates and this object coordinates."""
        return self.get_slope_xy(p.x, p.y)
 

    def set_mid(self, p1:"Point", p2:"Point"):
        """ Set the coordinates of this point to the mid point between p1 and p2.
        p1          -- first point to use for mid point calculation.
        p2          -- second point to use for mid point calculation."""
        self.x = (p1.x + p2.x) // 2
        self.y = (p1.y + p2.y) // 2
        return


#============================================================================================

# Point of origin definition to reference the point directly in the 
# center of the front of the car.
ORIGIN = Point(IMG_XC, IMG_YMAX) 

#============================================================================================

class LaneLimitPoint(Point):
    """ Extension of the Point class to provide more functionality in the 
    context of finding lane limit points. The LaneLimitPoint is part of a 
    vector. The index in the vector is related to an expected position range 
    in the image and an expected width range of the lane given by the optics 
    and camera possition. """

    def __init__(self, idx:int):
        """ Initializes the LaneLimitPoint object depending on the index in 
        the vector. The passed index is used to calculate default values for 
        x,y coordinates, the distance to the origin and the expected lane 
        width at that position. 
        idx         -- index of this object in the array of the owning LaneLimits object"""
        self.idx = idx
        self.default_origin_distance = int(GRID_ORIGIN_OFFS + idx*(GRID_MIN_STEP_SIZE+(idx/2)) + 0.5)
        self.init_xy(Direction.Straight)
        self.update_lane_widths(Direction.Straight)
        self.usable = False
        return

         
    def init_xy(self, direction:Direction):
        """ Initializes default x,y coordinates for this point depending on
        the passed direction value. 
        direction   -- Direction enum code to ininitialize for."""
        if direction == Direction.Straight:
            self.x = IMG_XC
            self.y = IMG_YMAX - self.default_origin_distance
        elif direction == Direction.LeftDiag:
            d = int(self.default_origin_distance*DIAG_FACTOR)
            self.x = max(IMG_XC - d,0)
            self.y = max(IMG_YMAX - d,0)
        elif direction == Direction.RightDiag:
            d = int(self.default_origin_distance*DIAG_FACTOR)
            self.x = min(IMG_XC + d, IMG_XMAX)
            self.y = max(IMG_YMAX - d,0)
        elif direction == Direction.Left:
            self.x = max(IMG_XC - self.default_origin_distance,0)
            self.y = IMG_YMAX - ORIGIN_LANE_WIDTH
        elif direction == Direction.Right:
            self.x = min(IMG_XC + self.default_origin_distance,IMG_XMAX)
            self.y = IMG_YMAX - ORIGIN_LANE_WIDTH
        else:
            self.x = IMG_XC
            self.y = IMG_YMAX - self.default_origin_distance
        return


    def update_lane_widths(self, direction:Direction):
        """ Recalculates the expected lane widths values in relation to the 
        passed direction argument. Because of the optical distortions, the 
        lane width in the diagonal for instance is much smaller than in 
        the center straight ahead. The calculation is an approximation from
        real images of a 145 degree HOV camera.
        direction   -- Direction enum code to ininitialize for."""
        dist_corr = (abs(self.default_origin_distance - OPTICAL_CENTER_Y_OFFS) * 3) //15
        dist_corr += (max(self.default_origin_distance - IMG_YC//2, 0)) //10
        if direction != Direction.Left and direction != Direction.Right:
            dist_corr += abs(self.x - IMG_XC) //3
        self.nom_lane_width = max(ORIGIN_LANE_WIDTH - dist_corr,10)
        self.max_lane_width = self.nom_lane_width + ORIGIN_LANE_WIDTH_TOL 
        self.min_lane_width = self.nom_lane_width//2 
        self.approx_check_threshold = int(self.nom_lane_width*APPROX_CHECK_THRESHOLD_FACT)
        return
        

    def find_limit(self, mask, initial_search_width, initial_codes, limit_search_width, limit_codes, dx, dy, include_back):
        """ This method tries to find the lane limits in the mask.
        mask                    -- segmentation mask containing the SegmClass 
                                    codes
        initial_search_width    -- maximum number of steps to go left and right 
                                    to find a code present in initial_codes,
                                    can be 0 to skip the initial search
        initial_codes           -- list of SegmClass codes to find in initial 
                                    search, normally used to find my_lane
        limit_search_width      -- maximum number of steps to go left and right
                                    to keep tracking the lane limits by 
                                    searching for limit_codes
        limit_codes             -- list of SegmClass codes that represent a 
                                    lane limit, like white_solid_line etc.
        dx                      -- x-step, for instance -1 to search left
                                    and +1 to search right straight ahead
        dy                      -- y-step, for instance 0 straight ahead
                                    but +1 to search left and -1 to search 
                                    right when looking left and dx=0
        include_back            -- when True, the search starts going into the 
                                    opposite direction first to find a code 
                                    outside of limit_codes before going 
                                    forward until the first valid code is 
                                    found                   
        """
        x = self.x
        y = self.y
        self.usable = False

        if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
            print("find_limit: idx:%d  x:%d  y:%d  isw:%d  lsw:%d  dx:%d  dy:%d  back:%s" \
                % (self.idx, x, y, initial_search_width, limit_search_width, dx, dy, include_back))

        # The initial search is necessary to find the area with my_lane 
        # code first before it can be tracked later on. The search goes
        # into both directions (left,right) simultanously and stops
        # when the first initial code is found
        if initial_search_width > 0 and mask[y, x] not in initial_codes:
            if DEBUG_PRINT_FIND_LIMITS_SEARCH == True:
                print("initial search: code=%d=%s" % (mask[y, x],SegmClass(mask[y, x]).name))
            xx0 = xx1 = x
            yy0 = yy1 = y
            for i in range(initial_search_width):
                # checking to one side and stop, if we found our lane 
                xx0 = min(max(xx0 + dx,0),IMG_XMAX)
                yy0 = min(max(yy0 + dy,0),IMG_YMAX)
                if mask[yy0, xx0] in initial_codes:
                    x = xx0
                    y = yy0
                    if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
                        print("find_limit: initial found+ i:%d  x:%d  y:%d  mask:%d=%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name))
                    break

                # checking to other side and stop, if we found our lane 
                xx1 = min(max(xx1 - dx,0),IMG_XMAX)
                yy1 = min(max(yy1 - dy,0),IMG_YMAX)
                if mask[yy1, xx1] in initial_codes:
                    x = xx1
                    y = yy1
                    if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
                        print("find_limit: initial found- i:%d  x:%d  y:%d  mask:%d=%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name))
                    include_back = False   # don't have to go back in this case
                    break

                if DEBUG_PRINT_FIND_LIMITS_SEARCH == True:
                    print("initial searching i:%d  xx0:%d  yy0:%d  mask0:%d=%s,  xx1:%d  yy1:%d  mask1:%d=%s " % \
                        (i, xx0, yy0, mask[yy0, xx0],SegmClass(mask[yy0, xx0]).name, xx1, yy1, mask[yy1, xx1],SegmClass(mask[yy1, xx1]).name))

            # if our lane is not found in the search range, give up here
            if i >= initial_search_width:
                if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
                    print("find_limit: initial aborted i:%d  xx0:%d  yy0:%d xx1:%  yy1:%d" % (i, xx0, yy0, xx1, yy1))

                self.limit_found = False
                return False

        #elif self.limit_found == False:
            # Since the previous search didn't find the limit, double the search width
            #limit_search_width *= 2

        if include_back == True:
            # since the position might be somewhere in the line for instance,
            # go back until we left the limits in order to stay at the broder
            i = 0 
            if DEBUG_PRINT_FIND_LIMITS_SEARCH == True and (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX):
                print("back searching i:%d  x:%d  y:%d  mask:%d=%s in limit_codes:%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name,mask[y, x] in limit_codes))

            while (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX) and (mask[y, x] in limit_codes):
                x -= dx
                y -= dy
                i += 1
                if i >= limit_search_width:
                    if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
                        print("find_limit: back aborted i:%d  x:%d  y:%d  mask:%d" % (i, x, y, mask[y, x]))
                    return False

                if DEBUG_PRINT_FIND_LIMITS_SEARCH == True and (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX):
                    print("back searching i:%d  x:%d  y:%d  mask:%d=%s in limit_codes:%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name,mask[y, x] in limit_codes))

                    
        # assuming, the position is just back in the lane code: now go forward 
        # to find a limit code, just at the border next to the lane code
        i = 0 
        if DEBUG_PRINT_FIND_LIMITS_SEARCH == True and (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX):
            print("forward searching i:%d  x:%d  y:%d  mask:%d=%s in limit_codes:%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name,mask[y, x] in limit_codes))
        
        while (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX) and (mask[y, x] not in limit_codes):
            x += dx
            y += dy
            i += 1
            if i >= limit_search_width:
                if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
                    print("find_limit: forward aborted i:%d  x:%d  y:%d  mask:%d" % (i, x, y, mask[y, x]))
                return False

            if DEBUG_PRINT_FIND_LIMITS_SEARCH == True and (x > 0) and (x < IMG_XMAX) and (y > 0) and (y < IMG_YMAX):
                print("forward searching i:%d  x:%d  y:%d  mask:%d=%s in limit_codes:%s" % (i, x, y, mask[y, x],SegmClass(mask[y, x]).name,mask[y, x] in limit_codes))

        # poistion is found, so update point coordinates
        self.x = x
        self.y = y
        if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
            print("find_limit: found  x:%d  y:%d  mask:%d" % (x, y, mask[y, x]))

        self.usable = mask[y, x] in limit_codes
        return self.usable

#============================================================================================

class LaneLimits:
    """ LaneLimits holds an array of LaneLimitPoint objects, updated 
    through their find_limit method. This class also provides methods
    for calculating line approximation on the array and perform some
    sanity check on the results."""
    def __init__(self, side:Side):
        """ Initializes the LaneLimits object by creating the point array, 
        line approximation parameter adn history etc.
        side        -- Defines the limit left or right of the lane """
        self.points = [LaneLimitPoint(i) for i in range(N_LANE_POINTS)]
        self.direction = Direction.Nowhere
        self.side = side
        self.offs = 0
        self.slope = 0
        self.stddev = 0
        self.code = 0
        self.score = 0
        self.limit_found_count = 0

        if DEBUG_PRINT_FIND_LIMITS_POINTS == True:
            s = "%s: " % (self.side.name)
            for i in range(N_LANE_POINTS):
                s += "  %d:%d" % (i,self.points[i].default_origin_distance)
            print("dist_origin: %s" % (s))
        return

    def __getitem__(self,idx:int):
        """ Method to access this object as a linear array.
        idx     -- index into the point array. """
        return self.points[idx]

    def __len__(self):
        """ Method to return the length of the array """
        return N_LANE_POINTS

    def __str__(self):
        """ Returns a string representation of the array """
        s = "%s: " % (self.side.name)
        for i in range(N_LANE_POINTS):
            s += "%d.%s," % (i, self.points[i])
        return s

    def get_y(self, x:int):
        """ Returns the y-coordinate for the passed x-value using the 
        previously calculated line approximation parameter in this object. 
        x           -- x-coordinate input to calculate y-coordinate
        result      -- y-coordinate calculated from x input"""
        return round(self.slope*x + self.offs)


    def get_x(self, y:int):
        """ Returns the x-coordinate for the passed x-value using the 
        previously calculated line approximation parameter in this object. 
        y           -- y-coordinate input to calculate x-coordinate
        result      -- x-coordinate calculated from y input"""
        if self.slope != 0:
            return round((y - self.offs)/self.slope)
        else:
            return round((y - self.offs)/1e-6)


    def calc_line_approximation(self, eliminate_max:bool):
        """ Calculates a line approximation algorithm over the x,y
        coordinates in the line limit vector and sets slope and offs
        for get_line_x/y methods. This allows approximating the lane limits 
        by lines.
        eliminate_max   -- if true, eliminate the maximum dx as outlier """

        # If not enough lane limit had been found, allow all points
        if self.limit_found_count < 3:
            for i in range(N_SEARCH_POINTS):
                self.points[i].usable = True

        # First, calulate the x and y sums for x,y averages
        x_sum = 0
        y_sum = 0
        n = 0
        for i in range(N_SEARCH_POINTS):
            if self.points[i].usable == True:
                x_sum += self.points[i].x
                y_sum += self.points[i].y
                n += 1

        # for now, just x average is needed
        x_avg = x_sum/n

        if eliminate_max == True:
            # Best results can be achieved by eliminating one "outlier"
            max_i = 0
            max_dx = 0
            for i in range(N_SEARCH_POINTS):
                if self.points[i].usable == True:
                    dx = self.points[i].x - x_avg
                    if dx > max_dx:
                        max_dx = dx
                        max_i = i

            # take out the point with the maximum dx and recalculate averages
            n -= 1        
            self.points[max_i].usable = False
            x_avg = (x_sum - self.points[max_i].x)/n
            y_avg = (y_sum - self.points[max_i].y)/n
            
        else:
            y_avg = y_sum/n
        
        # Now sum up all x and y deltas
        dx_sum = 0
        dy_sum = 0
        x = -1
        y = -1
        # first, get x and y averages
        for i in range(N_SEARCH_POINTS):
            if self.points[i].usable == True:
                if x >= 0:
                    dx_sum += self.points[i].x - x
                    dy_sum += self.points[i].y - y
                x = self.points[i].x
                y = self.points[i].y

        # to a large enough value instead of infinity, use sign
        if dx_sum != 0:
            self.slope = dy_sum/dx_sum
        else:
            self.slope = dy_sum * 1000

        self.offs = y_avg - self.slope*x_avg

        if DEBUG_PRINT_LINE_APPROX == True:
            print("%s: Approx: n=%d x_avg=%.0f y_avg=%.0f dx_sum=%.0f dy_sum=%.0f slope=%.3f offs=%.1f" % \
                    (self.side.name, n, x_avg,y_avg,dx_sum,dy_sum,self.slope,self.offs))

        return
              


    def line_approx_check(self, adjust:bool):
        """ Performs a sanity check on the line approximation by checking
        the differences between real points and line approximation. The
        goal is replacing outliers with the line approximation values and
        to return True, if the line approximation should be repeated after that
        replacement.
        parm        -- line approximation parameter object to be used for calculation.
        adjust      -- enables the outlier replacement when True.
        return      -- again"""
        again = False
        d = [0]*N_SEARCH_POINTS
        dsum=0
        # Check separately for straight, diagonal and horizzontal
        if self.direction == Direction.Straight:
            if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                s = "%s: xx/dx: " % (self.side.name)

            for i in range(N_SEARCH_POINTS):
                # calculate the x-coordinate from line approximation
                xx = self.get_x(self.points[i].y)
                # and get the difference to real value
                dx = xx - self.points[i].x
                # if above threshold, replace with approximation
                if adjust == True and \
                  (self.points[i].usable == False or abs(dx) >= self.points[i].approx_check_threshold):
                    self.points[i].x = min(max(xx,0),IMG_XMAX)
                    self.points[i].usable = True
                    again = True
                else:
                    d[i] = dx
                    dsum += dx
                    
                if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                    s += "%d/%d, " % (xx,dx)
                    
        elif self.direction == Direction.RightDiag or self.direction == Direction.LeftDiag:
            if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                s = "%s: xx,yy/dxy: " % (self.side.name)

            for i in range(N_SEARCH_POINTS):
                # calculate the x-coordinate from line approximation
                xx = self.get_x(self.points[i].y)
                # calculate the y-coordinate from line approximation
                yy = self.get_y(self.points[i].x)
                
                dx = (xx - self.points[i].x)/2
                dy = (yy - self.points[i].y)/2
                dxy = round(math.sqrt(dx*dx + dy*dy))

                # if above threshold, replace with approximation
                if adjust == True and \
                    (self.points[i].usable == False or abs(dxy) >= self.points[i].approx_check_threshold):
                    self.points[i].x = min(max((xx + self.points[i].x)//2,0),IMG_XMAX)
                    self.points[i].y = min(max((yy + self.points[i].y)//2,0),IMG_YMAX)
                    self.points[i].usable = True
                    again = True
                else:
                    d[i] = dxy
                    dsum += dxy
                    
                if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                    s += "%d,%d/%d, " % (xx,yy,dxy)
 
        else: #self.direction == Direction.Right or self.direction == Direction.Left:
            if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                s = "%s: yy/dy: " % (self.side.name)

            for i in range(N_SEARCH_POINTS):
                # calculate the y-coordinate from line approximation
                yy = self.get_y(self.points[i].x)
                # and get the difference to real value
                dy = yy - self.points[i].y
                # if above threshold, replace with approximation
                if adjust == True and \
                    (self.points[i].usable == False or abs(dy) >= self.points[i].approx_check_threshold):
                    self.points[i].y = min(max(yy,0),IMG_YMAX)
                    self.points[i].usable = True
                    again = True
                else:
                    d[i] = dy
                    dsum += dy

                if DEBUG_PRINT_LINE_APPROX_CHECK == True:
                    s += "%d/%d, " % (yy,dy)


        # now calculate the standard deviation excluding the replacements
        # as a noise merrit 
        davg = dsum/N_SEARCH_POINTS
        dsum = 0
        for i in range(N_SEARCH_POINTS):
            dd = d[i]-davg
            dsum += dd*dd
        self.stddev = math.sqrt(dsum/N_SEARCH_POINTS)

        if DEBUG_PRINT_LINE_APPROX_CHECK == True:
            print("%s again:%s  stddev:%.1f" % (s, again, self.stddev))   

        return again


    def extrapolate(self):
        """Extrapolates the additional points outside search points using slope and offs"""
        if self.direction == Direction.Straight:
            for i in range(N_SEARCH_POINTS,N_LANE_POINTS):
                # calculate the x-coordinate from line approximation
                xx = self.get_x(self.points[i].y)
                self.points[i].x = min(max(xx,0),IMG_XMAX)

        elif self.direction == Direction.RightDiag or self.direction == Direction.LeftDiag:
            for i in range(N_SEARCH_POINTS,N_LANE_POINTS):
                # calculate the x-coordinate from line approximation
                xx = self.get_x(self.points[i].y)
                yy = self.get_y(self.points[i].x)
                self.points[i].x = min(max((xx + self.points[i].x)//2,0),IMG_XMAX)
                self.points[i].y = min(max((yy + self.points[i].y)//2,0),IMG_YMAX)

        else: #self.direction == Direction.Right or self.direction == Direction.Left:
            for i in range(N_SEARCH_POINTS,N_LANE_POINTS):
                # calculate the y-coordinate from line approximation
                yy = self.get_y(self.points[i].x)
                self.points[i].y = min(max(yy,0),IMG_YMAX)
        return


    def update_line_approx(self):
        """Re-calculates line approximation over all points, then extrapolation."""
        # Calculate slope and offset over all points
        self.calc_line_approximation(False)
        # Calculate standard deviation without replacements
        self.line_approx_check(False)
        # Extrapolate the additional points
        self.extrapolate()
        return


    def get_dominant_class(self, mask):
        """Determines the class code with the highest occurance along the lane limits.
        Codes are counted in the vector and the maximum count and code returned.
        mask        -- segmentation mask containing the SegmClass code."""

        # get the dx and dy steps for the current direction
        if self.side == Side.Left:
            dx, dy = DEFINITIONS_LEFT[self.direction.value][2:4]
        else:
            dx, dy = DEFINITIONS_RIGHT[self.direction.value][2:4]

        code = [-1 for _ in range(N_LANE_POINTS)]
        count = [0 for _ in range(N_LANE_POINTS)]
         # Copy all codes from window into linear array
        for i in range(N_LANE_POINTS):
            code[i] = mask[min(max(self.points[i].y + dy,0),IMG_YMAX), min(max(self.points[i].x + dx,0),IMG_XMAX)]

        # Go through and count, mark already counted with -1
        for i in range(N_LANE_POINTS):
            c = code[i]
            if c>=0:
                for j in range(i,N_LANE_POINTS):
                    if c == code[j]:
                        count[i] += 1
                        if i != j:
                            code[j] = -1
        # Look for the maximum count
        max_count = 0
        max_idx = 0
        for i in range(N_LANE_POINTS):
            if code[i]>=0 and count[i] > max_count:
                max_count = count[i]
                max_idx = i

        # return the code at maximum count and the normalized score
        return code[max_idx], max_count/N_LANE_POINTS


    def find_limits(self, mask, direction:Direction):
        """ Top level method to perform the search for the lane limits, 
        calculate the line approximation for the line approximation, 
        check and replace outliers and run again if necessary.
        mask        -- segmentation mask containing the SegmClass code
        direction   -- Direction class value of the current or new 
                        direction """

        # Preload the correct parameter depending on left or right side
        if self.side == Side.Left:
            initial_codes, limit_codes, dx, dy = DEFINITIONS_LEFT[direction.value]
        else:
            initial_codes, limit_codes, dx, dy = DEFINITIONS_RIGHT[direction.value]

        # Set flag if xy need to be re-initialized because of direction change
        initxy = self.direction == Direction.Nowhere or \
                 self.direction != direction or \
                 self.stddev > INIT_XY_STDDEV_THRESHOLD or \
                 self.limit_found_count <= INIT_XY_LIMIT_FOUND_THRESHOLD
        include_back = not initxy

        #set search limit according to new initial search or continue tracking 
        if initxy == True:
            initial_search_width = IMG_XMAX
            limit_search_width = IMG_XMAX
            for i in range(N_LANE_POINTS):
                self.points[i].init_xy(direction)
        else:
            initial_search_width = 0 
            limit_search_width = ORIGIN_LANE_WIDTH

        t0 = time.perf_counter()
        self.direction = direction
        
        # call the find_limit methods for each search point
        self.limit_found_count = 0
        for i in range(N_SEARCH_POINTS):
             if self.points[i].find_limit(mask, initial_search_width, initial_codes, limit_search_width, limit_codes, dx, dy, include_back):
                self.limit_found_count += 1
            
        if DEBUG_PRINT_FIND_LIMITS_LANE == True:
            s ="%s: x/y  " % (self.side.name)
            for i in range(N_SEARCH_POINTS):
                tf = "%s"%(self.points[i].usable)
                s += "%3d,%3d:%s "%(self.points[i].x,self.points[i].y, tf[0])
            print(s)

        # now run line approximation and check and replace outliers for a limited 
        # number of repeatitions only, if necessary. The result should be a 
        # clean-up linear approximation of the lane limits
        for i in range(3):
            t1 = time.perf_counter()
            self.calc_line_approximation(i==0)
            t2 = time.perf_counter()  

            if DEBUG_MASK_IMG == True and DEBUG_MASK_IMG_LEFT_RIGHT == True:
                self.extrapolate()
                mask_img = self.draw(DEBUG_MASK_IMG_REFERENCE.copy())
                cv2.imwrite(f'{DEBUG_MASK_IMG_FILE_NAME}_{self.side.name}_{i}_0.jpg',mask_img)

            t3 = time.perf_counter()
            do_it_again = self.line_approx_check(True)
            t4 = time.perf_counter()
            
            if DEBUG_PRINT_FIND_LIMITS_LANE == True:
                s ="%d. x/y  " % (i)
                for j in range(N_SEARCH_POINTS):
                    s += "%3d,%3d  "%(self.points[j].x,self.points[j].y)
                print(s)

            if DEBUG_MASK_IMG == True and DEBUG_MASK_IMG_LEFT_RIGHT == True:
                self.extrapolate()
                mask_img = self.draw(DEBUG_MASK_IMG_REFERENCE.copy())
                cv2.imwrite(f'{DEBUG_MASK_IMG_FILE_NAME}_{self.side.name}_{i}_1.jpg',mask_img)
 
            if do_it_again == False:
                break

        self.extrapolate()
        self.code, self.score = self.get_dominant_class(mask)

        if DEBUG_PRINT_FIND_LIMITS_LANE == True:
            print("dt:  %.3fms  %.3fms  %.3fms" % ((t1-t0)*1000,(t2-t1)*1000,(t4-t3)*1000))
            print("%s  Approx: slope=%.3f offs=%.1f stdev=%.3f" %(self.side.name, self.slope, self.offs, self.stddev))
            print("%s  Dominant: %d=%s, Score=%.1f" % (self.side.name,self.code,SegmClass(self.code).name,self.score))
            
            if self.side == Side.Left:
                dx, dy = DEFINITIONS_LEFT[self.direction.value][2:4]
            else:
                dx, dy = DEFINITIONS_RIGHT[self.direction.value][2:4]

            s ="mask codes:  "
            for i in range(N_SEARCH_POINTS):
                s += "%3d  "%(mask[min(max(self.points[i].y + dy,0),IMG_YMAX), min(max(self.points[i].x + dx,0),IMG_XMAX)])
            print(s)

        return


    def draw(self, img):
        """ Method to draw the line approximation line and all limit points 
        onto the passed input image and return the resulting image.
        img     -- image bitmap to draw to."""

        # define a color depending on left or right side
        if self.side == Side.Left:
            bgr = (255,128,0)
        else:
            bgr = (255,0,128)

        # calculate the line end points accordingly
        pt1 = (self.get_x(self.points[0].y), self.points[0].y)
        pt2 = (self.get_x(self.points[-1].y), self.points[-1].y)

        # draw the line onto the image
        img = cv2.line(img, pt1, pt2, bgr, 2) 

        # draw all individual search points of the vector
        for i in range(N_SEARCH_POINTS):
            img = self.points[i].draw(img, bgr)

        if self.side == Side.Left:
            bgr = (128,64,0)
        else:
            bgr = (128,0,64)

        # draw all individual extrapolated points of the vector
        for i in range(N_SEARCH_POINTS,N_LANE_POINTS):
            img = self.points[i].draw(img, bgr)

        #return the resulting image
        return img



