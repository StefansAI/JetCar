# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`ServoPWM`
==========

Adding PWM channels to Servo implementations in adafruit_servokit.
The PCA9685 chip is naturally a PWM chip. adafruit_servokit reduces the
PWM range to realize servo timing. This module introduces the PWM
range mapped from 0.0 to 1.0 in parallel to servo functionality.

* Author(s): Stefan Warnke

Dependency:
Adafruit's Servo Kit library: https://github.com/adafruit/Adafruit_CircuitPython_ServoKit.git
"""
from adafruit_servokit import ServoKit

__version__ = "0.0.0-auto.0"
__repo__ = "https://github.com/..."


class PWM:
    """ PWM channel class similar to the adafruit servo classes.
    This class maps the range 0.0 to 1.0 to the PWM range of the chip.
    For more flexibility the maximum duty cycle can be reduced.

    :param pwm_out: Reference to PCA9685 channel object to be linked to.
    """
    def __init__(self, pwm_out):
        self._pwm_out = pwm_out
        self._max_duty = 0xFFFF
    
    @property
    def fraction(self):
        """Pulse width expressed as fraction between 0.0 and 1.0 (`max_duty`)."""
        if self._pwm_out.duty_cycle == 0:  
            return 0
        return self._pwm_out.duty_cycle / self._max_duty

    @fraction.setter
    def fraction(self, value):
        if value <= 0:
            self._pwm_out.duty_cycle = 0  
        elif value >= 1.0:
            self._pwm_out.duty_cycle = self._max_duty 
        else:
            self._pwm_out.duty_cycle = int(value * self._max_duty)
            
    @property
    def max_duty(self):
        """Maximum PWM duty cycle, normally 0xFFFF, but can be reduced to a lower required level"""
        return self._max_duty
    
    @max_duty.setter
    def max_duty(self, value):
        if value <= 1:
            self._max_duty = 1  
        elif value >= 0xFFFF:
            self._max_duty = 0xFFFF
        else:
            self._max_duty = int(value)

 
class _PWM:
    """This class creates the array access to the list of PWM objects."""
    def __init__(self, kit):
        self.kit = kit

    def __getitem__(self, pwm_channel):
        num_channels = self.kit._channels
        if pwm_channel >= num_channels or pwm_channel < 0:
            raise ValueError("servo must be 0-{}!".format(num_channels - 1))
        pwm = self.kit._items[pwm_channel]
        if pwm is None:
            pwm = PWM(self.kit._pca.channels[pwm_channel])
            self.kit._items[pwm_channel] = pwm
            return pwm
        if isinstance(self.kit._items[pwm_channel], PWM):
            return pwm
        raise ValueError("Channel {} is already in use.".format(pwm_channel))

    def __len__(self):
        return len(self.kit._items)

    
class ServoPWM(ServoKit):
    """ServoPWM inherits everything from Adafruit's ServoKit and adds the pwm objects in parallel to servo and continues_servo"""
    def __init__(self,*,channels=16,i2c=None,address=0x40,reference_clock_speed=25000000,frequency=50):
        super().__init__(channels=16,i2c=None,address=0x40,reference_clock_speed=25000000,frequency=50)
        self._pwm=_PWM(self)
            
    @property
    def pwm(self):
        """Accesses the list of PWM objects."""
        return self._pwm
          
