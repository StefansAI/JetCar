# A few helper functions for the offline debugging.
# The function names are sufficient as explananation.
from jetcar_definitions import *

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


import pandas as pd

def load_log_file(file_name):
    log_file = None
    if os.path.exists(file_name): 
        log_file = pd.read_csv(file_name)
        print('Log file loaded!')
    return log_file