#!/bin/sh

set -e

password=$1

sudo apt-get update

echo "\e[44m ======= jetson-fan-ctl ======== \e[0m"
cd $HOME
git clone https://github.com/Pyrestone/jetson-fan-ctl
cd jetson-fan-ctl
echo $password | sudo -S ./install.sh
echo $password | sudo -S service automagic-fan restart

echo "\e[44m ======= Workaround for JetCard ======== \e[0m"
cd $HOME
sudo apt-get install -y python3-pip pkg-config
sudo apt-get install -y libhdf5-serial-dev hdf5-tools libhdf5-dev zlib1g-dev zip libjpeg8-dev liblapack-dev libblas-dev gfortran
sudo ln -s /usr/include/locale.h /usr/include/xlocale.h
sudo -H pip3 install -U --verbose 'protobuf<4' 'Cython<3'
sudo -H pip3 install https://developer.download.nvidia.com/compute/redist/jp/v461/pytorch/torch-1.11.0a0+17540c5+nv22.01-cp36-cp36m-linux_aarch64.whl
sudo -H pip3 install --pre --extra-index-url https://developer.download.nvidia.com/compute/redist/jp/v461  'tensorflow<3'

sudo apt-get install libomp-dev

echo "\e[44m ======= JetCard & mods =========== \e[0m"
git clone https://github.com/NVIDIA-AI-IOT/jetcard
cd jetcard
sudo sed -i 's,wget -N https://nvidia.box.com/shared/static/9eptse6jyly1ggt9axbja2yrmj6pbarc.whl -O torch-1.6.0-cp36-cp36m-linux_aarch64.whl,#,g' install.sh
sudo sed -i 's,sudo -H pip3 install numpy,#,g' install.sh
sudo sed -i 's,sudo -H pip3 install -U numpy,#,g' install.sh
sudo sed -i 's/sudo -H pip3 install Cython/#/g' install.sh
sudo sed -i 's/futures protobuf/futures==3.0.5 /g' install.sh
sudo sed -i 's,sudo -H pip3 install --pre --extra-index-url https://developer.download.nvidia.com/compute/redist/jp/v45,#,g' install.sh
./install.sh $password

cd $HOME

# enable i2c permissions
echo $password | sudo -S usermod -aG i2c $USER

echo "\e[44m ======= adafriut servokit  ======== \e[0m"
echo $password | sudo -SH pip3 install adafruit-circuitpython-servokit==1.3.0

echo "\e[44m ======= jetcar notebooks  ======== \e[0m"
cd $HOME
mkdir JetCar
cd JetCar
git init
git remote add -f origin "http://github.com/StefansAI/JetCar"
git config core.sparseCheckout true
echo "firmware/jetcar/notebooks" >> .git/info/sparse-checkout
git pull origin main

cd $HOME
mv JetCar/firmware/jetcar/notebooks JetCar
rmdir JetCar/firmware/jetcar
rmdir JetCar/firmware


echo "\e[44m ======= mobile net v3  ======== \e[0m"
python3 <<HEREDOC
import segmentation_models_pytorch as smp
model = smp.UnetPlusPlus(encoder_name="timm-mobilenetv3_large_minimal_100", classes=10, activation='argmax2d')
HEREDOC


echo "\e[44m ======= Installation finished! ======= \e[0m"


