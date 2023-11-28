<h1 style="text-align: center;">JetCar</h1>
<h2 style="text-align: center;">The mini self-driving car project</h2>
<br>
<div style="text-align: center;">
  <img src="docs/assets/images/JetCar.png" />
</div>
<br>
The JetCar project is based on ideas from <a href="https://github.com/NVIDIA-AI-IOT/jetbot">JetBot</a> and <a href="https://github.com/NVIDIA-AI-IOT/jetracer">JetRacer</a>. It utilizes the <a href="https://developer.nvidia.com/embedded/jetson-nano-developer-kit">Jetson Nano Developer Kit</a>, the OLED display and battery pack from JetBot and the steering servo control from JetRacer. But this project includes a completely new mechanical design, as compact as possible with these components. 
The goal is not speed or road following alone, but navigating on a street map autonomously, recognizing intersections and turning left or right on user request when the turn is allowed. To do this, JetCar has to recognize intersections and a few signs on the street.
<br>
<div style="text-align: center;">
  <img src="docs/assets/images/JetCar_Demo.gif" />
</div>
<br>
The project is organized into following folders:
<br><br>

1. firmware
    - jetcar: Jupyter notebooks and python files to run in the JetCar
    - offline_debug: python files for offline debugging of firmware recordings
    - SD card install script 
<br><br>

2. mechanical
    - step_files: all step files of the car design
    - stl_files: all stl files for 3D printer
<br><br>

3. tools
    - bin: Windows executables
    - source: Full C# source code projects for the executables 
    - Jupyter notebook for training
<br><br>

To download just one sub folder from this repository, <a href="https://www.gitkraken.com/learn/git/github-download#how-to-download-a-folder-from-github">read here</a> or enter the URL directly in <a href="https://download-directory.github.io/"> https://download-directory.github.io/.</a> 
<br><br>
More documentation:<br>
- <a href="docs/BOM.md">BOM</a><br>
- <a href="docs/Assembly.md">Assembly</a><br>
- <a href="docs/SD%20Card%20Setup.md">SD Card Setup</a><br>
- <a href="docs/Data%20Preparation.md">Data Preparation with ImageSegmenter</a><br>
- <a href="docs/Model%20Training.md">Model Training</a><br>
- <a href="docs/StreetMaker.md">Street Maker</a><br>
- <a href="docs/Operation.md">Operation</a><br>
- <a href="docs/Debugging.md">Debugging</a><br>
<br><br>
