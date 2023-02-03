import ipywidgets

import cv2
from IPython.display import display
import numpy as np
import os
import time
from jetcar_definitions import *
from jetcar_lane import *
from jetcar_tracker import *
from debug_utils import *

if True:
    import jetcar_lane
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_POINTS = True
    jetcar_lane.DEBUG_PRINT_LINE_APPROX_CHECK = True
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_LANE = True
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_SEARCH = True
    jetcar_lane.DEBUG_PRINT_LINE_APPROX = True
    jetcar_lane.DEBUG_MASK_IMG = True
    #jetcar_lane.DEBUG_MASK_IMG_LEFT_RIGHT = True

    import jetcar_center
    #jetcar_center.DEBUG_PRINT_GET_CENTER_CLASSES = True
    #jetcar_center.DEBUG_PRINT_GET_NEW_LIST = True
    jetcar_center.DEBUG_PRINT_UPDATE_OBJECTS_OBJECT_LIST = True

    import jetcar_tracker
    jetcar_tracker.DEBUG_PRINT_GET_LANE_LIMITS = True
    jetcar_tracker.DEBUG_PRINT_HANDLE_DIRECTIONS = True
    jetcar_tracker.DEBUG_PRINT_PROCESS_CLASSES = True


color_map = load_color_map("jetcar_colormap.csv")

tracker = LaneTracker()

make_or_clear_directory("DebugOut")
next_direction = START_DIRECTION

for img_count in range(RECORDING_MIN,RECORDING_MAX):
    summary_text = "Img: %d" %(img_count)
    print("Start processing %s" %(summary_text))
    filename = 'Img_%03d.jpg' % (img_count)
    image_path = os.path.join(RECORDING_DIR, filename)
    
    if os.path.exists(image_path):
        image = cv2.imread(image_path)
     
        filename = 'Mask_%03d.npy' % (img_count)
        mask_path = os.path.join(RECORDING_DIR, filename)
        mask = np.load(mask_path,mmap_mode='r') #.astype(np.uint8)
        mask_img = color_map[mask]
         
        if jetcar_lane.DEBUG_MASK_IMG == True:
            jetcar_lane.DEBUG_MASK_IMG_REFERENCE = mask_img
            jetcar_lane.DEBUG_MASK_IMG_FILE_NAME = os.path.join('DebugOut', 'Mask_%03d' % (img_count))

        if img_count == 0:
            next_direction = Direction.Straight
        if img_count == 2:
            next_direction = Direction.Right
        #if img_count == 40:
        #    next_direction = Direction.Left
        elif img_count == 22:
            jetcar_tracker.DEBUG_PRINT_GET_LANE_LIMITS = True
            jetcar_lane.DEBUG_MASK_IMG_LEFT_RIGHT = True
            jetcar_lane.DEBUG_PRINT_LINE_APPROX_CHECK = True
            jetcar_lane.DEBUG_PRINT_FIND_LIMITS_LANE = True
            jetcar_lane.DEBUG_PRINT_FIND_LIMITS_POINTS = True
            jetcar_center.DEBUG_PRINT_GET_CENTER_CLASSES = True
        elif img_count == 2300:
            break
      

        if img_count > 0:
            tracker.process(mask, next_direction)
        
        mask_img = tracker.draw(mask_img)

        summary_text += " ->processed!"

        print(summary_text)
        print("=====================================================================================================================")
  
    else:
        break



