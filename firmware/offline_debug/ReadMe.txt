Offline debugging with recorded images and masks from the JetCar can be done via Visual Studio Code or other environments.
Following files from the JetCar notebooks folder will have to be copied to the debug folder:
  1. jetcar_definitions.py
  2. jetcar_lane.py
  3. jetcar_center.py
  4. jetcar_tracker.py
  5. jetcar_colormap.csv
  
Unzip recordings into DebugData.  

Edit offline_debug.py to enable or disable debug prints.