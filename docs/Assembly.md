<h1 style="text-align: center;">JetCar</h1>
<h2 style="text-align: center;">Assembly</h2>
<br><br>
<img src="assets/images/assembly/01-printed_parts.jpg"/><br>
Get the parts from the 3D printer, remove the support layers and clean up the parts, sand them where necessary and make them smooth for assembly.
<br><br>
<br><img src="assets/images/assembly/02-steering.jpg"/><br>
Start with the undercarriage and the steering parts. Place the wheel spindles on the undercarriage and make sure, they move and don't grind. File, sand or use drill bits where necessary to make them fit snug and smooth.
<br><br>
<br><img src="assets/images/assembly/03-steering.jpg"/><br>
Place the steering rack part and the front axle part on top of the wheel spindles and make sure again, it moves smoothly.
<br><br>
<br><img src="assets/images/assembly/04-steering.jpg"/><br>
Screw the front axle to the undercarriage with the taping screws. Check afterwards that the steering parts move smoothly.
<br><br>
<br><img src="assets/images/assembly/05-servo_driver.jpg"/><br>
Now, screw the servo controller to the undercarriage. Find more info about the servo controller here:  <a href="https://learn.adafruit.com/16-channel-pwm-servo-driver/downloads">Schematics</a> 
<br><br>
<br><img src="assets/images/assembly/06-motor_driver.jpg"/><br>
Solder wires with connectors to the motors and to the H-bridge boards and slide the H-bridges in place. <a href="https://www.ti.com/lit/ds/symlink/drv8833.pdf">Datasheet</a> 
<br><br>
<br><img src="assets/images/assembly/07-motor.jpg"/><br>
Take the motors and put the original hubs and wheels on the shafts. Glue the hub caps to the outside for better looks.
<br><br>
<br><img src="assets/images/assembly/08-motor_bracket.jpg"/><br>
Place motors and bracket on top of the undercarriage and fix the brackets with the tapping screws. Make sure, the outer metal gear plates end up in the small grooves of the undercarriage.
<br><br>
<br><img src="assets/images/assembly/09-rear_wheels.jpg"/><br>
With both motors mounted it should look like this.
<br><br>
<br><img src="assets/images/assembly/10-front_wheels.jpg"/><br>
Since the steering does not work well with the original wheels, use the printed wheels and place the rubber tires on top of those. 
<br><br>
<br><img src="assets/images/assembly/11-front_wheel.jpg"/><br>
Use the M3 screws to fix the front wheels to the wheel spindles.
<br><br>
<br><img src="assets/images/assembly/12-all_wheels.jpg"/><br>
Here it is with all four wheels assembled.
<br><br>
<br><img src="assets/images/assembly/13-servo_arm.jpg"/><br>
Mount the servo arm to the servo. If you have a servo tester to center the servo first, then the screw can be tightened. Otherwise wait with tightening until the power up test later.
<br><br>
<br><img src="assets/images/assembly/14-servo.jpg"/><br>
Mounte the servo with arm to the front axle using the tapping screws.
<br><br>
<br><img src="assets/images/assembly/15-pushrod.jpg"/><br>
Mount the servo push rod by gently pressing it over the 2 balls of servo arm and steering rack until it snaps in.
<br><br>
<br><img src="assets/images/assembly/16-Wiring_Diagram.jpg"/><br>
This is the wiring diagram including optional LED lights. The pin assignment shown represents the current connections. But they can be completely re-arranged. If so, don't forget to change the channel definitions in <a href="https://github.com/StefansAI/JetCar/blob/main/firmware/jetcar/notebooks/jetcar_car.py">JetCar_Car.py</a>.
<br><br>
<br><img src="assets/images/assembly/17-power_cable.jpg"/><br>
The right angle USB cable will be used as power cable.
<br><br>
<br><img src="assets/images/assembly/18-Cut_cable.jpg"/><br>
Cut it at the end of the smaller connector.
<br><br>
<br><img src="assets/images/assembly/19-Cut_data_lines.jpg"/><br>
Cut the data lines and leave only red (+) and black (-) wires. Solder 3 pairs of extensions for the servo driver board and the 2 H-bridge boards. Use shrink tube to isolate and stabilize the wires.
<br><br>
<br><img src="assets/images/assembly/20-motor_wired.jpg"/><br>
Plug in the wire connectors from the H-bridges to the servo board, the motor connectors to the H-bridge boards and the USB power cable connectors to all. (This picture does not match the wiring diagram above. It was a first test.)
<br><br>
<br><img src="assets/images/assembly/21-wifi_card.jpg"/><br>
To prepare the Jetson Nano board, remove the nano module with the heatsink and plug the Wi-Fi-board with the snapped in antennas in. Fix the Wi-Fi card with the original screw.
<br><br>
<br><img src="assets/images/assembly/22-nano_antennas.jpg"/><br>
Snap the nano module back in place.
<br><br>
<br><img src="assets/images/assembly/23-nano_flatbed.jpg"/><br>
Mount the fan on top of the heatsink.
<br><br>
<br><img src="assets/images/assembly/24-pioled.jpg"/><br>
Solder the right angle header on the top side of the PiOLED display and plug it into the header connector of the Jetson Nano.
<br><br>
<br><img src="assets/images/assembly/25-Jetson_Nano_Header.jpg"/><br>
The I2C interface used for this OLED and the servo board is located at the last pins of this connector. Grab some wires and note the colors used at the pins.
<br><br>
<br><img src="assets/images/assembly/26-oled.jpg"/><br>
Make sure it sits on the correct pins.
<br><br>
<br><img src="assets/images/assembly/27-together.jpg"/><br>
Connect to the correct I2C pins on the servo board.
<br><br>
<br><img src="assets/images/assembly/28-power_up.jpg"/><br>
Connect the 2 USB cables to the 2 battery ports and the system should start up for first testing. Make sure to have the micro SD card prepared correctly (see firmware setup) and inserted. The wheels should not touch the ground for this test.
<br><br>
<br><img src="assets/images/assembly/28a-IP-address.jpg"/><br>
After the Jetson Nano is started up, the OLED display shows the IP address the board is on at this moment. Now open a web browser and type in the top this address plus port number 8888. In this case 192.168.1.105:8888 to connect to the board. Attention: Go through the firmware setup first.
<br><br>
<br><img src="assets/images/assembly/29-Basic_Motion.jpg"/><br>
When connected, navigate to the Jupyter notebooks, open and run <a href="https://github.com/StefansAI/JetCar/blob/main/firmware/jetcar/notebooks/JetCar_Basic_Motion.ipynb">JetCar_Basic_Motion.jpynb</a>. You can now move the sliders for steering and throttle. The sliders on top are only indicators, the ones at the bottom for adjustments. 
<br><br>
<br><img src="assets/images/assembly/30-LED_schematics.png"/><br>
Adding LEDs to the setup is not necessary, but gives a nice touch. Here is the schematics for the wiring below.<br>
In essence, all back and signal lights are wired as serial pairs without resistor, since there is one on the PWM board for each channel. These LEDs have a forward voltage of around 2V and use small currents. <br>
The white headlights have forward voltages of 3.2V at 20mA and will have to get their own serial resistors. Each pair can now be driven by their individual H-bridge.
<br><br>
<br><img src="assets/images/assembly/31-LED_wiring.jpg"/><br>
For the LED wiring use thinner and more flexible wires to connect between the LEDs and the stiffer ones with the connector just to get to the header. Fix the LEDs and wires with hot glue, which can be peeled off if needed.
<br><br>
<br><img src="assets/images/assembly/32-LED_wiring_front.jpg"/><br>
The headlight LEDs have all their serial resistors in shrink tubes with the wires coming out to connect to the other side.
<br><br>
<br><img src="assets/images/assembly/33-LED_wiring_back.jpg"/><br>
The back side is easier. The red wire is +5V connecting all serial pairs on one side. The other side goes directly to the PWM servo board outputs, which already have a serial resistor on board.
<br><br>
<br><img src="assets/images/assembly/34-front_LEDs.jpg"/><br>
Front view with LEDs.
<br><br>
<br><img src="assets/images/assembly/35-back_LEDs.jpg"/><br>
Back view with LEDs.
<br><br>
<br><img src="assets/images/assembly/36-connecting_LEDs.jpg"/><br>
Now comes the trickiest part of the whole assembly. The LED wires won't have so much room and need to be short. But this makes it really hard to plug in when putting the lower body part on the undercarriage.
<br><br>
<br><img src="assets/images/assembly/37-the_tricky_part.jpg"/><br>
Place the body on top, but don't screw it on so it can be moved. Now use tweezers to grab one by one through the top openings and place them on the correct header. Lift or move the body a bit to the sides for better access. It takes a moment to get them all in. Good luck.
<br><br>
<br><img src="assets/images/assembly/38-LED_test.jpg"/><br>
Now another test exactly the same way as above with the JetCar_Basic_Motion notebook. This time turn on one by one all LED check boxes to verify them.
<br><br>
<br><img src="assets/images/assembly/39-upper_body.jpg"/><br>
Mount the upper body on top and fix with screws.
<br><br>
<br><img src="assets/images/assembly/40-battery.jpg"/><br>
Plug in USB connectors and place battery in compartment after placing the switch piece into the opening. Sand it down if necessary. In the end it should easily turn on the battery pack with one click and turn off with a double click. The cables are running on the side to the front.
<br><br>
<br><img src="assets/images/assembly/41-flatbed.jpg"/><br>
Now place the flatbed with the Jetson Nano on top. It slides under in the front and can be fixed with 2 screws in the back. Connect the power cable and the I2C wires with tweezers.
<br><br>
<br><img src="assets/images/assembly/42-camera.jpg"/><br>
Screw the camera mount to the cabin and screw in the camera board.
<br><br>
<br><img src="assets/images/assembly/43-camera_connected.jpg"/><br>
Plug in the flex cable of the camera into the Jetson Nano connector. Lift the plastic part, place the flex board in and press the tiny plastic part back in. Make sure, it is held in and doesn't come out.
<br><br>
<br><img src="assets/images/assembly/44-Done.jpg"/><br>
Congratulation. It's done and ready to go.
<br><br><br>


- <a href="BOM.md">BOM</a><br>
- <a href="SD%20Card%20Setup.md">SD Card Setup</a><br>