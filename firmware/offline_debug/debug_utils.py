from jetcar_definitions import *


#RECORDING_DIR = "DebugData/LaneChangeLeft/"
#RECORDING_DIR = "DebugData/InMiddleLane/"
#RECORDING_DIR = "DebugData/StraightStop/"
#RECORDING_DIR = "DebugData/StraightAhead/"
#RECORDING_DIR = "DebugData/StraightYield/"
#RECORDING_DIR = "DebugData/OffLaneRight/"
#RECORDING_DIR = "DebugData/LeftTurn/"
#RECORDING_DIR = "DebugData/RightCurve/"
RECORDING_DIR = "DebugData/TestDrive/"
RECORDING_MIN = 0 #48
RECORDING_MAX = 1000
START_DIRECTION = Direction.Straight.value
NEXT_DIRECTION = Direction.Left.value
NEXT_DIR_IDX = 17


import cv2
import PIL.Image
import numpy as np

def bgr8_to_jpeg(value, quality=75):
    return bytes(cv2.imencode('.jpg', value)[1])


import pandas as pd
import matplotlib.pyplot as plt
import os

def load_color_map(file_name):
    color_map = 0
    if os.path.exists(file_name): 
        color_map = np.loadtxt(open(file_name, "rb"), delimiter=",", skiprows=0).astype(np.uint8)
        print('Color map loaded!')
    else:
        np.savetxt(file_name,color_map, delimiter=",")
    return color_map


def make_or_clear_directory(directory_name):
    if os.path.exists(directory_name) == False:
        os.mkdir(directory_name)
    else:
        files = os.listdir(directory_name) 
        #files = [f for f in os.listdir(directory_name) if os.path.isfile(f)]
        for name in files:
            os.remove(os.path.join(directory_name, name))