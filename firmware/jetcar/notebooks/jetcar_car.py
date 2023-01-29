# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`JetCar`
==========

JetCar contains all control elements for moving and steering the car through 
"Adafruit 16-Kanal/16-Channel 12-Bit Pwm board". Steering is done via a standard
servo, while motor control via PWM directly. More PWM channels can be used for
lights or other functions.
JetCar uses traitlets to process traitlets events.

* Author(s): Stefan Warnke

Dependency:
traitlets
Adafruit's Servo Kit library: https://github.com/adafruit/Adafruit_CircuitPython_ServoKit.git
ServoPWM module based on the servo kit library.
"""

import traitlets
from ServoPWM import ServoPWM

__version__ = "0.0.0-auto.0"
__repo__ = "https://github.com/..."
        
class JetCar(traitlets.HasTraits):
    """JetCar uses "Adafruit 16-Kanal/16-Channel 12-Bit Pwm board" to move around.
    The steering servo is connected to this board and h-bridges to control the motors.
    Additional channels can be used for lights."""
    
    # This is the standard I2C bus address of the adafruit board
    I2C_ADDRESS = 0x40
    
    # Index of the channel where the servo is plugged in
    STEERING_CHANNEL = 0

    # Indices of the motor control channels  
    THROTTLE_LEFT_FORWARD_CHANNEL = 1
    THROTTLE_LEFT_REVERSE_CHANNEL= 2
    THROTTLE_RIGHT_FORWARD_CHANNEL = 3
    THROTTLE_RIGHT_REVERSE_CHANNEL = 4
    
    HEAD_LIGHT_HIGH_CHANNEL_P=13
    HEAD_LIGHT_HIGH_CHANNEL_N=12
    HEAD_LIGHT_LOW_CHANNEL_P=15
    HEAD_LIGHT_LOW_CHANNEL_N=14
    
    TAIL_LIGHT_CHANNEL=8
    BRAKE_LIGHT_CHANNEL=9
  
    SIGNAL_LEFT_CHANNEL=11
    SIGNAL_RIGHT_CHANNEL=5

    #traitlets
    steering = traitlets.Float()
    throttle = traitlets.Float()
    head_light_high = traitlets.Float()
    head_light_low = traitlets.Float()
    tail_light = traitlets.Float()
    brake_light = traitlets.Float()
    signal_left = traitlets.Float()
    signal_right = traitlets.Float()
   
    def __init__(self, *args, **kwargs):
        """Initializes the JetCar object and prepares all used channels.
        Some defaults are assigned and should be changed if necessary"""
        self.kit = ServoPWM(channels=16, address=self.I2C_ADDRESS)
        self.steering_servo = self.kit.continuous_servo[self.STEERING_CHANNEL]
        self.throttle_left_forward = self.kit.pwm[self.THROTTLE_LEFT_FORWARD_CHANNEL]
        self.throttle_left_reverse = self.kit.pwm[self.THROTTLE_LEFT_REVERSE_CHANNEL]
        self.throttle_right_forward = self.kit.pwm[self.THROTTLE_RIGHT_FORWARD_CHANNEL]
        self.throttle_right_reverse = self.kit.pwm[self.THROTTLE_RIGHT_REVERSE_CHANNEL]
        self.steering_offset = -0.08
        self.steering_gain = -0.69
        self.steering_throttle_threshold = 0.25
        self.steering_throttle_gain = 0.25
        self.throttle_min_threshold = 0.05
        self.throttle_raw_value = 0;
        self.throttle_ratio = 1
        self.head_light_high_pwm_p = self.kit.pwm[self.HEAD_LIGHT_HIGH_CHANNEL_P]
        self.head_light_high_pwm_n = self.kit.pwm[self.HEAD_LIGHT_HIGH_CHANNEL_N]
        self.head_light_high_pwm_p.fraction = 0
        self.head_light_high_pwm_n.fraction = 1
        self.head_light_low_pwm_p = self.kit.pwm[self.HEAD_LIGHT_LOW_CHANNEL_P]
        self.head_light_low_pwm_n = self.kit.pwm[self.HEAD_LIGHT_LOW_CHANNEL_N]
        self.head_light_low_pwm_p.fraction = 0
        self.head_light_low_pwm_n.fraction = 1
        self.tail_light_pwm = self.kit.pwm[self.TAIL_LIGHT_CHANNEL]
        self.tail_light_pwm.fraction = 1
        self.brake_light_pwm = self.kit.pwm[self.BRAKE_LIGHT_CHANNEL]
        self.brake_light_pwm.fraction = 1
        self.signal_left_pwm = self.kit.pwm[self.SIGNAL_LEFT_CHANNEL]
        self.signal_left_pwm.fraction = 1
        self.signal_right_pwm = self.kit.pwm[self.SIGNAL_RIGHT_CHANNEL]
        self.signal_right_pwm.fraction = 1
      
    
    def _update_throttle(self):
        """Updates all motor channels depending forward or reverse using the p
        recalculated throttle_ratio to balance between the 2 motors"""
        if abs(self.throttle_raw_value) < self.throttle_min_threshold:
            self.throttle_left_forward.fraction = 0 
            self.throttle_left_reverse.fraction = 0
            self.throttle_right_forward.fraction = 0  
            self.throttle_right_reverse.fraction = 0
        else:
            self.throttle_left = self.throttle_raw_value*self.throttle_ratio;
            if self.throttle_left >= 0:
                self.throttle_left_forward.fraction = min(self.throttle_left,1)   
                self.throttle_left_reverse.fraction = 0
            else:
                self.throttle_left_reverse.fraction = min(-self.throttle_left,1)  
                self.throttle_left_forward.fraction = 0

            self.throttle_right = self.throttle_raw_value/self.throttle_ratio;
            if self.throttle_right >= 0:
                self.throttle_right_forward.fraction = min(self.throttle_right,1)  
                self.throttle_right_reverse.fraction = 0
            else:
                self.throttle_right_reverse.fraction = min(-self.throttle_right,1)   
                self.throttle_right_forward.fraction = 0
                
    @traitlets.validate('steering')
    def _clip_steering(self, proposal):
        """Clips the steering value to the range -1 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < -1.0:
            return -1.0
        else:
            return proposal['value']
        
    @traitlets.observe('steering')
    def _on_steering(self, change):
        """Steering change event handler to apply offset and gain and calculate a new throttle_ratio 
        depending on steering value. Calls _update_throttle to update motor control."""
        self.steering_servo.throttle = min(max(change['new'] * self.steering_gain + self.steering_offset,-1),+1)
        
        if self.steering_servo.throttle > self.steering_throttle_threshold:
            self.throttle_ratio = 1  - (self.steering_servo.throttle - self.steering_throttle_threshold) * self.steering_throttle_gain
        elif self.steering_servo.throttle < - self.steering_throttle_threshold:
            self.throttle_ratio = 1  - (self.steering_servo.throttle + self.steering_throttle_threshold) * self.steering_throttle_gain
        else:
            self.throttle_ratio = 1
            
        self._update_throttle()
            
            
    @traitlets.validate('throttle')
    def _clip_throttle(self, proposal):
        """Clips the throttle value to the range -1 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < -1.0:
            return -1.0
        else:
            return proposal['value']          
   
    @traitlets.observe('throttle')
    def _on_throttle(self, change):
        """Throttle change event handler to update the output for both motors."""
        self.throttle_raw_value = change['new']
        self._update_throttle()

        
    @traitlets.validate('head_light_high')
    def _clip_head_light_high(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('head_light_high')
    def _on_head_light_high(self, change):
        """Turns on or off high beam head lights."""
        self.head_light_high_pwm_p.fraction = change['new']
        self.head_light_high_pwm_n.fraction = 1-change['new']

        
    @traitlets.validate('head_light_low')
    def _clip_head_light_low(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('head_light_low')
    def _on_head_light_low(self, change):
        """Turns on or off low beam head lights."""
        self.head_light_low_pwm_p.fraction = change['new']
        self.head_light_low_pwm_n.fraction = 1-change['new']

        
    @traitlets.validate('tail_light')
    def _clip_tail_light(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('tail_light')
    def _on_tail_light(self, change):
        """Turns on or off tail lights."""
        self.tail_light_pwm.fraction = 1-change['new']*0.25

    @traitlets.validate('brake_light')
    def _clip_tail_light(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('brake_light')
    def _on_brake_light(self, change):
        """Turns on or off brake lights."""
        self.brake_light_pwm.fraction = 1-change['new']

        
    @traitlets.validate('signal_left')
    def _clip_signal_left(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('signal_left')
    def _on_signal_left(self, change):
        """Turns on or off signal left lights."""
        self.signal_left_pwm.fraction = 1-change['new']

        
    @traitlets.validate('signal_right')
    def _clip_signal_right(self, proposal):
        """Clips the throttle value to the range 0 to 1"""
        if proposal['value'] > 1.0:
            return 1.0
        elif proposal['value'] < 0:
            return 0
        else:
            return proposal['value']          
   
    @traitlets.observe('signal_right')
    def _on_signal_right(self, change):
        """Turns on or off signal right lights."""
        self.signal_right_pwm.fraction = 1-change['new']

