#!/bin/sh

set -e

password=$1

cd $HOME
git clone https://github.com/NVIDIA-AI-IOT/jetcard
cd jetcard

echo "============================= start jetcard ================================================"
# fix NTP server
FILE="/etc/systemd/timesyncd.conf"
echo $password | sudo -S bash -c "echo 'NTP=0.arch.pool.ntp.org 1.arch.pool.ntp.org 2.arch.pool.ntp.org 3.arch.pool.ntp.org' >> $FILE"
echo $password | sudo -S bash -c "echo 'FallbackNTP=0.pool.ntp.org 1.pool.ntp.org 0.us.pool.ntp.org' >> $FILE"
cat $FILE
echo $password | sudo -S systemctl restart systemd-timesyncd.service

# enable i2c permissions
echo $password | sudo -S usermod -aG i2c $USER

# install pip and some apt dependencies
echo "============================= apt & pip ==================================================="
echo $password | sudo -S apt-get update
echo $password | sudo -S apt install -y python3-pip python3-pil python3-smbus python3-matplotlib cmake curl
echo $password | sudo -H pip3 install -U pip
echo $password | sudo -H pip3 install -U pip testresources==2.0.1 setuptools==49.6.0
echo $password | sudo -H pip3 install flask==1.1.2
echo $password | sudo -H pip3 install -U --upgrade numpy

# install tensorflow
echo "============================= tensorflow ==================================================="
echo $password | sudo -S apt-get install -y libhdf5-serial-dev hdf5-tools libhdf5-dev zlib1g-dev zip libjpeg8-dev liblapack-dev libblas-dev gfortran
echo $password | sudo -H pip3 install -U numpy==1.16.1 future==0.18.2 mock==3.0.5 h5py==2.10.0 keras_preprocessing==1.1.1 keras_applications==1.0.8 gast==0.2.2 futures==3.1.1 protobuf==3.14.0 pybind11==2.6.1
echo $password | sudo -H pip3 install -U grpcio==1.34.0 absl-py==0.10.0 py-cpuinfo==7.0.0 psutil==5.7.3 portpicker==1.3.1 six==1.15.0 requests==2.25.0
echo $password | sudo -H pip3 install Cython==0.29.21
echo $password | sudo -H pip3 install h5py==2.10.0
echo $password | sudo -H pip3 install -U astor==0.8.1 termcolor==1.1.0 keras-applications==1.0.8 keras-preprocessing==1.1.2 wrapt==1.12.1 google-pasta==0.2.0
echo $password | sudo -H pip3 install --pre --extra-index-url https://developer.download.nvidia.com/compute/redist/jp/v44 tensorflow==2.3.1+nv20.11

# install pytorch
echo "============================= pytorch ==================================================="
wget https://nvidia.box.com/shared/static/wa34qwrwtk9njtyarwt5nvo6imenfy26.whl -O torch-1.7.0-cp36-cp36m-linux_aarch64.whl
echo $password | sudo -S apt-get install -y python3-pip libopenblas-base libopenmpi-dev
echo $password | sudo -H pip3 install Cython==0.29.21
echo $password | sudo -H pip3 install numpy==1.16.1 torch-1.7.0-cp36-cp36m-linux_aarch64.whl
# install torchvision
echo $password | sudo -S apt-get -y install libjpeg-dev zlib1g-dev libpython3-dev libavcodec-dev libavformat-dev libswscale-dev
echo "============================= torchvision ==============================================="
git clone --branch v0.8.1 https://github.com/pytorch/vision torchvision
cd torchvision
export BUILD_VERSION=0.8.1
echo $password | sudo -H python3 setup.py install
cd ../

# install traitlets (master)
echo "============================= traitlets master ============================================"
echo $password | sudo -H pip3 install traitlets

# install updated nodejs
echo "============================= nodejs ======================================================"
wget https://nodejs.org/dist/v12.13.0/node-v12.13.0-linux-arm64.tar.xz
tar -xJf node-v12.13.0-linux-arm64.tar.xz
cd node-v12.13.0-linux-arm64
echo $password | sudo -S cp -R * /usr/local/
node -v
npm -v

# install jupyter lab
echo "============================= jupyter lab ================================================="
echo $password | sudo -H pip3 install -U jupyter jupyterlab
echo $password | sudo -S jupyter labextension install @jupyter-widgets/jupyterlab-manager
#echo $password | sudo -S jupyter labextension install @jupyterlab/statusbar
jupyter lab --generate-config -y

# set jupyter password
python3 -c "from notebook.auth.security import set_password; set_password('$password', '$HOME/.jupyter/jupyter_notebook_config.json')"
cd ../

# install jetcard
echo "============================= jetcard ====================================================="
echo $password | sudo -H python3 setup.py install

# install jetcard display service
python3 -m jetcard.create_display_service
echo $password | sudo -S mv jetcard_display.service /etc/systemd/system/jetcard_display.service
echo $password | sudo -S systemctl enable jetcard_display
echo $password | sudo -S systemctl start jetcard_display

# install jetcard jupyter service
echo "============================= jupyter service ============================================"
python3 -m jetcard.create_jupyter_service
echo $password | sudo -S mv jetcard_jupyter.service /etc/systemd/system/jetcard_jupyter.service
echo $password | sudo -S systemctl enable jetcard_jupyter
echo $password | sudo -S systemctl start jetcard_jupyter

# make swapfile
echo "============================= swapfile =================================================="
echo $password | sudo -S fallocate -l 4G /var/swapfile
echo $password | sudo -S chmod 600 /var/swapfile
echo $password | sudo -S mkswap /var/swapfile
echo $password | sudo -S swapon /var/swapfile
echo $password | sudo -S bash -c 'echo "/var/swapfile swap swap defaults 0 0" >> /etc/fstab'

# install TensorFlow models repository
echo "============================= tensorflow models ========================================="
git clone https://github.com/tensorflow/models
cd models/research
git checkout 5f4d34fc
wget -O protobuf.zip https://github.com/protocolbuffers/protobuf/releases/download/v3.7.1/protoc-3.7.1-linux-aarch_64.zip
unzip protobuf.zip
./bin/protoc object_detection/protos/*.proto --python_out=.
echo $password | sudo -H python3 setup.py install
cd slim
echo $password | sudo -H python3 setup.py install
cd ../../../

# disable syslog to prevent large log files from collecting
echo $password | sudo -S service rsyslog stop
echo $password | sudo -S systemctl disable rsyslog

# install jupyter_clickable_image_widget
echo "============================= jupyter_clickable_image_widget ============================"
echo $password | sudo -S npm install -g typescript
git clone https://github.com/jaybdub/jupyter_clickable_image_widget
cd jupyter_clickable_image_widget
echo $password | sudo -H pip3 install -e .
echo $password | sudo -S jupyter labextension install js

jupyter labextension list
echo "============================= end jetcard ==============================================="

echo "============================= jetcam ===================================================="
cd $HOME
git clone https://github.com/NVIDIA-AI-IOT/jetcam
cd jetcam
echo $password | sudo -H python3 setup.py install

echo "============================= torch2trt ===================================================="
cd $HOME
git clone https://github.com/NVIDIA-AI-IOT/torch2trt
cd torch2trt
echo $password | sudo -H python3 setup.py install

echo "============================= jetracer ===================================================="
cd $HOME
git clone https://github.com/NVIDIA-AI-IOT/jetracer
cd jetracer
echo $password | sudo -H python3 setup.py install

echo "============================= jetson-fan-ctl ===================================================="
cd $HOME
git clone https://github.com/Pyrestone/jetson-fan-ctl
cd jetson-fan-ctl
echo $password | sudo -S ./install.sh
echo $password | sudo -S service automagic-fan restart

echo $password | sudo -H pip3 install adafruit-circuitpython-servokit==1.3.0
echo $password | sudo -H pip3 install segmentation-models-pytorch==0.1.2

cd $HOME
echo $password | sudo -S nvpmodel -m0

echo $password | sudo -S jupyter lab build
echo "======================== Installation finished! ==========================================="


