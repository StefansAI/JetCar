# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`jetcar_definitions`
====================
Main definitions for the jetcar code. 
Besides image size, this file contains segmentation class definitions and
grouping lists of these segmentaion classes used in the code:

class SegmClass(enum.Enum):
    Definition of all segmentation class names and values

class Side(enum.Enum):
    Definitions of a side from the current lane

class Direction(enum.Enum):
    Definitions of simplified driving directions 
    

* Author(s): Stefan Warnke

Dependencies:
-
"""

import enum

# Image size in both dimensions x and y
IMG_SIZE = 224 

# Center x and y coordinates in the image or mask
IMG_XC = IMG_SIZE//2
IMG_YC = IMG_SIZE//2

# Maximum indices in x and y dimensions
IMG_XMAX = IMG_SIZE-1
IMG_YMAX = IMG_SIZE-1

# Scene Parsing has N classes including nothing=0
N_CLASSES = 19

# Definition of all segmentation class names and values
class SegmClass(enum.Enum):
	nothing = 0
	white_shoulder_line = 1
	yellow_solid_line = 2
	yellow_dashed_line = 3
	yellow_double_line = 4
	lane_driving_dir = 5
	lane_wrong_dir = 6
	lane_left_turn = 7
	lane_right_turn = 8
	yield_line = 9
	stop_line = 10
	stop_text = 11
	arrow_straight = 12
	arrow_straight_left = 13
	arrow_straight_right = 14
	arrow_left_only = 15
	arrow_right_only = 16
	arrow_left_right = 17
	parking_sign = 18
	#Unused:
	white_solid_line = 100
	white_dashed_line = 101
	white_double_line = 102
	yellow_solid_dashed_line = 103
	yellow_dashed_solid_line = 104
	lane_limit_line = 105
	lane_center = 106
	yield_text = 107
	crosswalk_lines = 108
	crosswalk_zebra = 109
	merge_left = 110
	merge_right = 111

# Declare some special codes used directly in the classes
NOTHING_CODE =  SegmClass.nothing.value
LANE_MY_DRIVING_DIR_CODE = SegmClass.lane_driving_dir.value
LANE_LEFT_TURN_CODE = SegmClass.lane_left_turn.value
LANE_RIGHT_TURN_CODE = SegmClass.lane_right_turn.value
STOP_LINE_CODE = SegmClass.stop_line.value
STOP_TEXT_CODE = SegmClass.stop_text.value

# Code list definition for unspecified background
NOTHING_CODES = [NOTHING_CODE]

# Code list definition for my own lane only
MY_LANE_ONLY_CODES = [  
    LANE_MY_DRIVING_DIR_CODE]
 
# Code list definition for all of my lanes 
MY_LANE_CODES = [  
    LANE_MY_DRIVING_DIR_CODE, 
    LANE_LEFT_TURN_CODE, 
    LANE_RIGHT_TURN_CODE]

# Code list definition for all possible lane limits on 
# the left side of the lane straight ahead
STRAIGHT_LANE_LIMIT_CODES_LEFT = [ 
    NOTHING_CODE, \
    SegmClass.white_shoulder_line.value, \
    SegmClass.white_solid_line.value, \
    SegmClass.white_dashed_line.value, \
    SegmClass.yellow_solid_line.value, \
    SegmClass.yellow_dashed_line.value, \
    SegmClass.lane_wrong_dir.value, \
    SegmClass.lane_left_turn.value, \
    SegmClass.lane_right_turn.value]

# Code list definition for all possible lane limits on 
# the right side of the lane straight ahead
STRAIGHT_LANE_LIMIT_CODES_RIGHT = [ 
    NOTHING_CODE, \
    SegmClass.white_shoulder_line.value, \
    SegmClass.white_solid_line.value, \
    SegmClass.white_dashed_line.value, \
    SegmClass.yellow_solid_line.value, \
    SegmClass.yellow_dashed_line.value, \
    SegmClass.lane_wrong_dir.value, \
    SegmClass.lane_right_turn.value]

# Code list definition for all possible lane limits on 
# the left side of the left turn direction
LEFT_TURN_LIMIT_CODES_LEFT = [ 
    SegmClass.white_shoulder_line.value, \
    SegmClass.white_solid_line.value, \
    SegmClass.white_dashed_line.value, \
    SegmClass.yellow_solid_line.value, \
    SegmClass.yellow_dashed_line.value, \
    SegmClass.lane_wrong_dir.value]

# Code list definition for all possible lane limits on 
# the right side of the left turn direction
LEFT_TURN_LIMIT_CODES_RIGHT = LEFT_TURN_LIMIT_CODES_LEFT

# Code list definition for all possible lane limits on 
# the left side of the right turn direction
RIGHT_TURN_LIMIT_CODES_LEFT = LEFT_TURN_LIMIT_CODES_LEFT

# Code list definition for all possible lane limits on 
# the right side of the right turn direction
RIGHT_TURN_LIMIT_CODES_RIGHT = LEFT_TURN_LIMIT_CODES_LEFT

# Code list definition of all street signs
STREET_SIGN_CODES = [ 
    STOP_TEXT_CODE]

# Code list definition of all arrows
STREET_ARROW_CODES = [ 
    SegmClass.arrow_straight.value, \
    SegmClass.arrow_straight_left.value, \
    SegmClass.arrow_straight_right.value, \
    #SegmClass.arrow_left_straight_right.value, \
    SegmClass.arrow_left_only.value, \
    SegmClass.arrow_right_only.value, \
    SegmClass.arrow_left_right.value]

# Code list definitions of intersection signs to keep track
INTERSECTION_OBJECT_CODES = [ 
    #SegmClass.wait_line.value, \
    SegmClass.yield_line.value, \
    SegmClass.stop_line.value, \
    SegmClass.stop_text.value, \
    SegmClass.arrow_straight.value, \
    SegmClass.arrow_straight_left.value, \
    SegmClass.arrow_straight_right.value, \
    #SegmClass.arrow_left_straight_right.value, \
    SegmClass.arrow_left_only.value, \
    SegmClass.arrow_right_only.value, \
    SegmClass.arrow_left_right.value]


# Combined sets of code lists and dx/dy combinations for the different 
# directions for left lane limit searches
DEFINITIONS_LEFT = [
    (NOTHING_CODES,      NOTHING_CODES,                    0, 0), # Nowhere
    (MY_LANE_CODES,      LEFT_TURN_LIMIT_CODES_LEFT,       0,+1), # Left
    (MY_LANE_ONLY_CODES, LEFT_TURN_LIMIT_CODES_LEFT,      -1,+1), # LeftDiag
    (MY_LANE_ONLY_CODES, STRAIGHT_LANE_LIMIT_CODES_LEFT,  -1, 0), # Straight
    (MY_LANE_ONLY_CODES, LEFT_TURN_LIMIT_CODES_LEFT,      -1,-1), # RightDiag
    (MY_LANE_CODES,      LEFT_TURN_LIMIT_CODES_LEFT,       0,-1)] # Right

# Combined sets of code lists and dx/dy combinations for the different 
# directions for right lane limit searches
DEFINITIONS_RIGHT = [
    (NOTHING_CODES,      NOTHING_CODES,                    0, 0), # Nowhere
    (MY_LANE_CODES,      LEFT_TURN_LIMIT_CODES_RIGHT,      0,-1), # Left
    (MY_LANE_ONLY_CODES, LEFT_TURN_LIMIT_CODES_RIGHT,     +1,-1), # LeftDiag
    (MY_LANE_ONLY_CODES, STRAIGHT_LANE_LIMIT_CODES_RIGHT, +1, 0), # Straight
    (MY_LANE_ONLY_CODES, LEFT_TURN_LIMIT_CODES_RIGHT,     +1,+1), # RightDiag
    (MY_LANE_CODES,      LEFT_TURN_LIMIT_CODES_RIGHT,      0,+1)] # Right


# Combined sets of direction marking codes and allowed turns for left, straight and right
DEFINITIONS_DIRECTIONS = [              #       Left,Straight,Right
    (SegmClass.arrow_straight.value,            False, True,  False),
    (SegmClass.arrow_straight_left.value,       True,  True,  False),
    (SegmClass.arrow_straight_right.value,      False, True,  True ),
    #(SegmClass.arrow_left_straight_right.value, True,  True,  True ),
    (SegmClass.arrow_left_only.value,           True,  False, False),
    (SegmClass.arrow_right_only.value,          False, False, True ),
    (SegmClass.arrow_left_right.value,          True,  False, True )]


# Definitions of a side from the current lane
class Side(enum.Enum):
    Left    = -1
    Center  =  0
    Right   =  1

# Definitions of simplified driving directions 
class Direction(enum.Enum):
    Nowhere     = 0
    Left        = 1
    LeftDiag    = 2
    Straight    = 3
    RightDiag   = 4
    Right       = 5
