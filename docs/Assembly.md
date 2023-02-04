<h1 style="text-align: center;">JetCar</h1>
<h2 style="text-align: center;">Assembly</h2>
<br>
<p style="text-align: center;">
 <img src="assets/images/assembly/01-printed%20parts.JPG"/><br>
Get the parts from the 3D printer, remove the support layers and clean up the parts, sand them where necessary and make them smooth for assembly.
<br>
<br><img src="assets/images/assembly/02-steering.JPG"/><br>
Start with the undercarraige and the steering parts. Place the wheel spindles on the undercarriage and make sure, they move and don't grind. File, sand or use drill bits where necessary to make them fit snug and smoth.
<br>
<br><img src="assets/images/assembly/03-steering.JPG"/><br>
Place the steering rack part and the front axle part on top of the wheel spindles and make sure again, it moves smoothly.
<br>
<br><img src="assets/images/assembly/04-steering.JPG"/><br>
Screw the front axle to the undercarriage with the taping screws. Check afterwards that the steering parts move smoothly.
<br>
<br><img src="assets/images/assembly/05-servo%20driver.JPG"/><br>
Now, screw the servo controller to the undercarriage. Find more info about the servo controller here:  <a href="https://learn.adafruit.com/16-channel-pwm-servo-driver/downloads">Schematics</a> 
<br>
<br><img src="assets/images/assembly/06-motor%20driver.JPG"/><br>
Solder wires with connectors to the motors and to the H-bridge boards and slide the H-bridges in place. <a href="https://www.ti.com/lit/ds/symlink/drv8833.pdf">Datasheet</a> 
<br>
<br><img src="assets/images/assembly/07-motor.JPG"/><br>
Take the motors and put the original hubs and wheels on the shafts. Glue the hub caps to the outside for better looks.
<br>
<br><img src="assets/images/assembly/08-motor%20bracket.JPG"/><br>
Place motors and bracket on top of the undercarriage and fix the brackets with the tapping screws. Make sure, the outer metal gear plates end up in the small grooves of the undercarriage.
<br>
<br><img src="assets/images/assembly/09-rear%20wheels.JPG"/><br>
With both motors mounted it should look like this.
<br>
<br><img src="assets/images/assembly/10-front%20wheels.JPG"/><br>
Since the steering does not work well with the original wheels, use the printed wheels and place the rubber tires on top of those. 
<br>
<br><img src="assets/images/assembly/11-front%20wheel.JPG"/><br>
Use the M3 screws to fix the front wheels to the wheel spindles.
<br>
<br><img src="assets/images/assembly/12-all%20wheels.JPG"/><br>
With all four wheels assembled it shoul look like this.
<br>
<br><img src="assets/images/assembly/13-servo%20arm.JPG"/><br>
Mount the servo arm to the servo. If you have a servo tester to center the servo first, then the screw can be tightened. Otherwise wait with tightening until the power up test later.
<br>
<br><img src="assets/images/assembly/14-servo.JPG"/><br>
Mounte the servo with arm to the front axle using the tapping screws.
<br>
<br><img src="assets/images/assembly/15-pushrod.JPG"/><br>
Mount the servo push rod by gently pressing it over the 2 balls of servo arm and steering rack until it snaps in.
<br>
<br><img src="assets/images/assembly/16-Wiring%20Diagram.JPG"/><br>
This is the wiring diagram including optional LED lights. The pin assignment shown represents the current connections. But they can be completely re-arranged. If so, don't forget to change the channel assignemnts in JetCar_Car.py.
<br>
<br><img src="assets/images/assembly/17-power%20cable.JPG"/><br>
The right angle USB cable will be used as power cable.
<br>
<br><img src="assets/images/assembly/18-Cut%20cable.JPG"/><br>
Cut it at the end of the smaller connector.
<br>
<br><img src="assets/images/assembly/19-Cut%20data%20lines.JPG"/><br>
Cut the data lines and leave only red (+) and black(-). Solder 3 pairs of extensions for the servo driver board and the 2 H-bridge boards. Use shrink tube to isolate and stabilize the wires.
<br>
<br><img src="assets/images/assembly/20-motor%20wired.JPG"/><br>
Plug in the wire connectors from the H-bridges to the servo board, the motor connectors to the H-bridge boards and the USB power cable connectors to all. (This picture does not match the wiring diagram above. It was a first test.)
<br>
<br><img src="assets/images/assembly/21-wifi%20card.JPG"/><br>
To prepare the Jetson Nano board, remove the nano module with the heatsink and plug the Wifi-board with the snapped in antennas in. Fix the wifi card with the original screw.
<br>
<br><img src="assets/images/assembly/22-nano%20antennas.JPG"/><br>
Snap the nano module back in place.
<br>
<br><img src="assets/images/assembly/23-nano%20flatbed.JPG"/><br>
Mount the fan on top of the heatsink.
<br>
<br><img src="assets/images/assembly/24-pioled.JPG"/><br>
Solder the right angle header on the top side of the PiOLED display and plug it into the header connector of the Jetson Nano.
<br>
<br><img src="assets/images/assembly/25-Jetson Nano Header.JPG"/><br>
The I2C interface used for this OLED and the servo board is located at thelast pins of this connector. Grab some wires and note the colors used at the pins.
<br>
<br><img src="assets/images/assembly/26-oled.jpg"/><br>
Make sure it sits on the correct pins.
<br>
<br><img src="assets/images/assembly/27-together.JPG"/><br>
Connect to the correct I2C pins on the servo board.
<br>
<br><img src="assets/images/assembly/28-power%20up.JPG"/><br>
Connect the 2 USB cables to the 2 battery ports and the system should start up for first testing. Make sure to have the micro SD card prepared correctly (see firmware setup) and inserted. The wheels should not touch the ground for this test.
<br>
<br><img src="assets/images/assembly/28a-IP-address.JPG"/><br>
After the Jetson Nano is started up, the OLED display shows the the IP address the board is on at this moment. Now open a webbrowser and type in the top this address plus port number 8888. In this case 192.168.1.105:8888 to connect to the board. Attention: Go through the firmware setup first.
<br>
<br><img src="assets/images/assembly/29-Basic%20Motion.JPG"/><br>
When connected, navigate to the Jupyter notebooks, open and run "JetCar_Basic_Motion". You can now move the sliders for steering and throttle. The sliders on top are only indicators, the ones at the bottom for adjustments. 
<br>
<br><img src="assets/images/assembly/"/><br>
<br>
<br><img src="assets/images/assembly/31-LED%20wiring.JPG"/><br>
For the LED wiring use thinner and more flexible wires to connect between the LEDs and the the more stiffer ones with the connector just to get to the header. Fix the LEDs and wires with hot glue, which can be peeled off if needed.
<br>
<br><img src="assets/images/assembly/32-LED%20wiring%20front.JPG"/><br>
The headlight LEDs have all their serial resistors in shrink tubes with the wires coming out to connect to the other side.
<br>
<br><img src="assets/images/assembly/33-LED%20wiring%20back.JPG"/><br>
The back side is easier. The red wire is +5V connecting all serial pairs on one side. The other side goes directly to the PWM servo board outputs, which already have a serial resistor on board.
<br>
<br><img src="assets/images/assembly/34-front LEDs.JPG"/><br>
Front view with LEDs.
<br>
<br><img src="assets/images/assembly/35-back LEDs.jpg"/><br>
Back view with LEDs.
<br>
<br><img src="assets/images/assembly/36-connecting LEDs.JPG"/><br>
Now comes the trickiest part of the whole assembly. The LED wires won't have so much room and need to be short. But this makes it really hard to plug in when putting the lower body part on the undercarriage.
<br>
<br><img src="assets/images/assembly/37-the%20tricky%20part.JPG"/><br>
Place the body on top, but doen't screw it on so it can be moved. Now use tweezers to grab one by one through the top openings and place them on the correct header. Lift or move the body a bit to the sides for better access. It takes a moment to get them all in. Good luck.
<br>
<br><img src="assets/images/assembly/38-LED%20test.JPG"/><br>
Now another test exactly the same way as above with the JetCar_Basic_Motion notebook. This time turn on one by one all LED check boxes to verify them.
<br>
<br><img src="assets/images/assembly/39-upper%20body.JPG"/><br>
Mount the upper body on top and fix with screws.
<br>
<br><img src="assets/images/assembly/40-battery.JPG"/><br>
Plug in USB connectors and place battery in compartment after placing the switch piece into the opening. Sand it down if necessary. In the end it should easily turn on the battery pack with one click and turn off with a double click. The cables ar running on the side to the front.
<br>
<br><img src="assets/images/assembly/41-flatbed.JPG"/><br>
Now place the flatbed with the Jetson Nano on top. It slides under in the front and can be fixed with 2 screws in the back. Connect the power cable and the I2C wires with tweezers.
<br>
<br><img src="assets/images/assembly/42-camera.JPG"/><br>
Screw the camera mount to the cabin and screw in the camera board.
<br>
<br><img src="assets/images/assembly/43-camera%20connected.JPG"/><br>
Pluig in the flex cable of the camera into the Jetson Nano connector. Lift the plastic part, place the flex board in and press the tiny plastic part back in. Make sure, it is held in and doesn't come out.
<br>
<br><img src="assets/images/assembly/44-Done.JPG"/><br>
Congratulation. It's done and ready to go.
<br>
</p>
