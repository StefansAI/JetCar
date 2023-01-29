"""
`utils`
==========

Some basic functions for processing the image, load color map and directory handling.
"""

import torch
import torchvision.transforms as transforms
import torch.nn.functional as F
import cv2
import PIL.Image
import numpy as np

# Reverse order for BGR instead of RGB processing
mean = torch.Tensor([0.485, 0.456, 0.406][::-1]).cuda().half()
std = torch.Tensor([0.229, 0.224, 0.225][::-1]).cuda().half()

# Preprocess a camera image for inference
def preprocess(image):
    device = torch.device('cuda')
    image = PIL.Image.fromarray(image)
    image = transforms.functional.to_tensor(image).to(device).half()
    image.sub_(mean[:, None, None]).div_(std[:, None, None]).half()
    return image[None, ...].half()


import pandas as pd
import matplotlib.pyplot as plt
import os

# Loads the reversed RGB color look up table from a csv file or writes the current the empty one out if doesn't exist
def load_color_map(file_name):
    color_map = (plt.cm.jet(np.arange(256)) * 255).astype(np.uint8)
    if os.path.exists(file_name): 
        color_map = np.loadtxt(open(file_name, "rb"), delimiter=",", skiprows=0).astype(np.uint8)
        print('Color map loaded!')
    else:
        np.savetxt(file_name,color_map,delimiter=",")
    return color_map


# Make the directory if it doesn't exist or delete all files in it to clear
def make_or_clear_directory(directory_name):
    if os.path.exists(directory_name) == False:
        os.mkdir(directory_name)
    else:
        for name in os.listdir(directory_name):
            try:
                os.remove(os.path.join(directory_name, name))
            except:
                print("Could not delete %s !" % (name))
