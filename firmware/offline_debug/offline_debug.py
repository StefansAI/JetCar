# This file can be used for offline-debugging of recorded data from the JetCar.
# Create a workspace for Visual Studio Code and copy the firmware files here.
#  1. jetcar_definitions.py
#  2. jetcar_lane.py
#  3. jetcar_center.py
#  4. jetcar_tracker.py
#  5. jetcar_colormap.csv
# Unzip the recording from the JetCar into the sub-folder DebugData.

import cv2
from IPython.display import display
import numpy as np
import pandas
import os
import time
from jetcar_definitions import *
from jetcar_lane import *
from jetcar_tracker import *
from debug_utils import *

# Use recording image index Min/Max for restricting the range for debugging
RECORDING_MIN = 0 
RECORDING_MAX = 1000
# Definition of input and output data folders
RECORDING_DIR = "DebugData/"
MASK_OUTPUT_DIR = "DebugOut/"

# Uncomment any debug definition here for all 
if True:
    import jetcar_lane
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_POINTS = True
    #jetcar_lane.DEBUG_PRINT_LINE_APPROX_CHECK = True
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_LANE = True
    #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_SEARCH = True
    #jetcar_lane.DEBUG_PRINT_LINE_APPROX = True
    jetcar_lane.DEBUG_MASK_IMG = True
    #jetcar_lane.DEBUG_MASK_IMG_INITXY = True
    #jetcar_lane.DEBUG_MASK_IMG_LEFT_RIGHT = True
    #jetcar_lane.DEBUG_MASK_IMG_CENTER = True

    import jetcar_center
    #jetcar_center.DEBUG_PRINT_GET_CENTER_CLASSES = True
    #jetcar_center.DEBUG_PRINT_GET_NEW_LIST = True
    #jetcar_center.DEBUG_PRINT_UPDATE_OBJECT_LIST = True

    import jetcar_tracker
    #jetcar_tracker.DEBUG_PRINT_GET_LANE_LIMITS = True
    #jetcar_tracker.DEBUG_PRINT_HANDLE_DIRECTIONS = True
    #jetcar_tracker.DEBUG_PRINT_PROCESS_CLASSES = True

# load the color map for the output mask images
color_map = load_color_map("jetcar_colormap.csv")
# load the csv file from the recording to compare
log_file = load_log_file(os.path.join(RECORDING_DIR, "log_file.csv"))

#create top level object instance
tracker = LaneTracker()

# make sure, there is an empty output folder
make_or_clear_directory(MASK_OUTPUT_DIR)
next_direction = Direction.Straight

# Go through all image/mask pairs from min to max
for img_count in range(RECORDING_MIN,RECORDING_MAX):
    print("Start processing %s" %("Img: %d" %(img_count)))
    filename = 'Img_%03d.jpg' % (img_count)
    image_path = os.path.join(RECORDING_DIR, filename)
    
    # Check, if exists, otherwise stop debugging
    if os.path.exists(image_path):
        image = cv2.imread(image_path)
     
        # get the mask as array and create an image with class colors from it
        filename = 'Mask_%03d.npy' % (img_count)
        mask_path = os.path.join(RECORDING_DIR, filename)
        mask = np.load(mask_path,mmap_mode='r') 
        mask_img = color_map[mask]

        # Get the direction change from the log file
        if img_count < len(log_file.index):
            next_direction = Direction[log_file.loc[img_count]['nextdir']]
        
        # Assign the mask image reference and the base file name
        if jetcar_lane.DEBUG_MASK_IMG == True:
            jetcar_lane.DEBUG_MASK_IMG_REFERENCE = mask_img
            jetcar_lane.DEBUG_MASK_IMG_FILE_NAME = os.path.join(MASK_OUTPUT_DIR, 'Mask_%03d' % (img_count))
            #cv2.imwrite(os.path.join(MASK_OUTPUT_DIR, 'Mask_%03d.jpg' % (img_count)),mask_img)

        # Change this block to add more debugging from a specific image number on
        # Change the number below and uncomment whatever is needed.
        # Put a break point below to run the program until this number to start stepping into the code
        if img_count >= 1000: 
            #jetcar_tracker.DEBUG_PRINT_GET_LANE_LIMITS = True
            #jetcar_lane.DEBUG_MASK_IMG_INITXY = True
            #jetcar_lane.DEBUG_MASK_IMG_LEFT_RIGHT = True
            #jetcar_lane.DEBUG_PRINT_LINE_APPROX_CHECK = True
            #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_LANE = True
            #jetcar_lane.DEBUG_PRINT_FIND_LIMITS_POINTS = True
            jetcar_center.DEBUG_PRINT_GET_CENTER_CLASSES = True
            jetcar_center.DEBUG_PRINT_UPDATE_OBJECT_LIST = True
            jetcar_center.DEBUG_PRINT_GET_NEW_LIST = True

        # process the mask, everything happens in there, suppress the very first image from the camera, because it is always garbeled
        if img_count > 0:
            tracker.process(mask, next_direction)
        
        # at the end, just add a formatted version of the log file line fro comparison
        print("Img: %d ->processed!" %(img_count))
        if img_count < len(log_file.index):
            s ="log: "
            for col in log_file.columns:
                if isinstance(log_file.loc[img_count][col],float):
                    s+= "%s=%.3f, "%(col,float(log_file.loc[img_count][col]))
                else:
                s+= "%s=%s, "%(col,log_file.loc[img_count][col])
            print(s)
        print("=====================================================================================================================")
  
    else:
        break



