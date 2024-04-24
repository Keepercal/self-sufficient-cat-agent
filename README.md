Consequently, to install and use the ML-Agents Toolkit you will need to:

- Install Unity (2023.2 or later)
- Install Python (3.10.12 or higher)
- Install Anaconda Virtual Environment
- Clone the ML-Agents repository
- Install the mlagents-envs
- Install the mlagents Python package

# Prerequsites

Download Unity from here: https://unity.com/download
Install Python 3.10.12 or higher from here: https://www.python.org/downloads/
Download Anaconda3 VE: https://docs.anaconda.com/free/anaconda/install/windows/
Install Microsft Visual C++ 14.0 build tools: https://visualstudio.microsoft.com/visual-cpp-build-tools/

## Install Unity 2023.2 or Later

You will need the Unity editor in order to train the agent. Please install Unity Hub and editor version 2023.3.19f

## Clone the project repository

Please clone the project repository to a desired location. You will then be able to add it to Unity Hub and open the project.

`git clone https://github.com/sunnyCallum/cat-game.git`

## Add the cloned project to Unity Hub

Open Unity Hub and press "Add" or "Add project from disk". Locate the project clone and add it to Unity Hub.

## Open the cloned project

Once the project is within Unity Hub, click to open the project. If no editor is installed, install the latest version or verison 2023.3.19f.

Upon opening, it will more than likely tell you that the project is on an older editor version. Opening it with a newer version should be fine.

## Install Python 3.10.12 or Later

You will require Python in order to train the agent. Please install Python onto your system.

## Create a virtual env using Anaconda

I recommend installing Anaconda3 to seperate the packages needed for this project. Once installed, open Anaconda Powershell Prompt (Note: You will not be able to start the virtual env through Windows Command Prompt or MacOS Terminal) and run the following commands:

`conda create -n mlagents python=3.10.12`

&

`conda activate mlagents`

## Clone the ML-Agents repository

Please clone the ML-Agents repository to a desired location. This repo will be required to run the commands for training an agent. 

`git clone --branch release_21 https://github.com/Unity-Technologies/ml-agents.git`

## Install the mlagents Python package

Installing the mlagents Python package involves installing other Python packages that mlagents depends on.

### (Windows) Installing PyTorch

On Windows, you'll have to install the PyTorch package separately prior to installing ML-Agents in order to make sure the cuda-enabled version is used, rather than the CPU-only version. Activate your virtual environment and run from the command line:

pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121

### (OS X) Installing GRPC libraries#

On OS X, you may need to explicitly install the GRPC runtime libraries to avoid hitting errors when training like dlopen(/Users/alex.mccarthy/miniconda3/envs/mlagents/lib/python3.10/site-packages/grpc/_cython/cygrpc.cpython-310-darwin.so, 0x0002): symbol not found in flat namespace '_CFRelease'.

`pip3 install grpcio`

## Installing mlagents
To install the mlagents Python package, activate the `mlagents` virtual environment and run from the command line:

`cd /path/to/ml-agents` <-- cd to the place you cloned the `mlagents` repo <br>
`python -m pip install ./ml-agents-` <br>
`python -m pip install ./ml-agents` <br>

## Project config file

There is a YAML file called `dsp_config.yaml` within the `AdditionalFiles` folder of the project clone. Copy this file and place it in `/mlagents/config/`
