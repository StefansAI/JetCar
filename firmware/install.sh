#!/bin/sh

set -e

password=$1

git clone https://github.com/NVIDIA-AI-IOT/jetcard
cd jetcard
./install.sh

cd $HOME

# enable i2c permissions
echo $password | sudo -S usermod -aG i2c $USER

echo "============================= jetson-fan-ctl ===================================================="
cd $HOME
git clone https://github.com/Pyrestone/jetson-fan-ctl
cd jetson-fan-ctl
echo $password | sudo -S ./install.sh
echo $password | sudo -S service automagic-fan restart

echo $password | sudo -SH pip3 install adafruit-circuitpython-servokit==1.3.0
#echo $password | sudo -SH pip3 install segmentation-models-pytorch==0.1.2

#cd $HOME
#echo $password | sudo -S nvpmodel -m0

#cd $HOME
#echo $password | sudo -S jupyter lab build

cd $HOME
mkdir JetCar
cd JetCar
mkdir notebooks
cd notebooks

echo "======================== Installation finished! ==========================================="


