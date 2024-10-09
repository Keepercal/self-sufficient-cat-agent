# Cat Simulator (WIP): Nurturing a Self-Sustaining Agent Using Reinforcement Learning

This project is in memory of Wilbur, a friend of Ralph Road and a good boy.

<p align="center">
  <img src="AdditionalFiles/Images/wilbur.jpeg" alt="Wilbur basking in the sunlight." height="450" />
</p>

# About this project

Cat Simulator was born during my final year of university as the project that I developed as part of my dissertation. The simulation is comprised of a cat, "Wilbur", and three objects, a scratching post, a water bowl, and food. Wilbur is a machine-learning agent whose behaviour derives from a reinforcement learning script that I developed using the Unity ML-Agents package within the Unity Engine.

The premise of the simulation is that Wilbur is an agent with three needs; Hunger, Thirst, and Happiness. At every step during training, these needs decrease. Wilbur's objective is to keep his needs well-maintained by navigating his environment and using an object to replenish its respective needs. The more efficiently he does this, the more rewards he will receive. If Wilbur does not successfully maintain these needs, he will receive a punishment.

In its current form, Wilbur can only be trained within Unity. Upon pressing play you can observe Wilbur begin knowing nothing about his environment to navigate between the objects and maintain his needs.

# Setting Up

## Cloning this project
To tinker with this project, you must clone this repository and import it into Unity. This guide assumes that you are familiar with the Unity engine. If not, follow [this guide](https://learn.unity.com/tutorial/install-the-unity-hub-and-editor#662942dfedbc2a0315217028) to install the Unity Hub and Editor

In a directory of your choosing, run the following command:
<br/><br/>
`git clone https://github.com/sunnyCallum/self-sufficient-cat-agent`
<br/><br/>
It does not matter where you clone it, just make sure you can locate it within your file explorer.

## Adding the project to Unity Hub
Open Unity Hub, click "Add" then click "Add project from disk". Navigate to where you cloned this project and add the folder. The project will then appear in your project list.

## Setting up Unity ML-Agents

While the ML-Agents Toolkit installation guide provides comprehensive instructions on how to install the toolkit, I found that this did not work on my Windows machine. I recommend you follow the instructions below to get it working on your machine.

Thank you to [@DDoubleMaster](https://github.com/DDoubleMaster) for providing a solution to the errors encountered. 

### Install **Unity 2023.2** or Later

[Download](https://unity3d.com/get-unity/download) and install Unity.
It is recommended to install Unity through the Unity Hub as it will
enable you to manage multiple Unity versions.

### Install **Python 3.9.13**

I recommend [installing](https://www.python.org/downloads/) Python 3.9.13.
If you are using Windows, please install the x86-64 version instead of x86.
If your Python environment doesn't include `pip3`, see these
[instructions](https://packaging.python.org/guides/installing-using-linux-tools/#installing-pip-setuptools-wheel-with-linux-package-managers)
on installing it. It is also recommended to use [conda](https://docs.conda.io/en/latest/) or [mamba](https://github.com/mamba-org/mamba) to manage your python virtual environments.

### Conda Python setup

Conda is the virtual environment I used when developing this project. This guide will walk you through installation with a Conda environment. You are free to use
any virtual environment you wish, however, this guide will assume you are using Conda.
<br/><br/>
There are two options for installing Anaconda. `Miniconda` is a minimal CMD based package manager while `Anaconda Distributor` features more packages as well as a GUI called `Anaconda Navigator`.
I used `Anaconda Distributor` when developing this project, however `Miniconda` will also suffice for installing.
<br/><br/>
You can install `Anaconda Districtor` [here](https://docs.anaconda.com/anaconda/install/) and `Miniconda` [here](https://docs.anaconda.com/miniconda/miniconda-install/)
<br/><br/>
Once conda has been installed in your system, open a terminal and execute the following commands to set-up a python 3.9.13 virtual environment
and activate it.

```shell
conda create -n mlagents python=3.10.12 && conda activate mlagents
```

### Upgrade Pip

I recommend upgrading Pip to the latest version to make sure everything runs smoothly.

```shell
python -m pip install --upgrade pip
```

### Install the `com.unity.ml-agents` Unity package

The Unity ML-Agents C# SDK is a Unity Package. You can install the
`com.unity.ml-agents` package
[directly from the Package Manager registry](https://docs.unity3d.com/Manual/upm-ui-install.html).
Please make sure you enable 'Preview Packages' in the 'Advanced' dropdown in
order to find the latest Preview release of the package.

**NOTE:** If you do not see the ML-Agents package listed in the Package Manager
please follow the [advanced installation instructions](#advanced-local-installation-for-development) below.

### Install the `mlagents` Python package

Installing the `mlagents` Python package involves installing other Python
packages that `mlagents` depends on. So you may run into installation issues if
your machine has older versions of any of those dependencies already installed.

#### (Windows) Installing PyTorch

On Windows, you'll have to install the PyTorch package separately prior to
installing ML-Agents in order to make sure the cuda-enabled version is used,
rather than the CPU-only version. Activate your virtual environment and run from
the command line:

```sh
pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
```

Note that on Windows, you may also need Microsoft's
[Visual C++ Redistributable](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads)
if you don't have it already. See the [PyTorch installation guide](https://pytorch.org/get-started/locally/)
for more installation options and versions.

#### Install **Protobuf 3.20.3**

Note: You need to manually install Protobuf version 3.20.3 as it is a specific dependency required for compatibility with the Unity ML-Agents toolkit.
Using this version ensures optimal performance and functionality within the environment.

```shell
pip install protobuf==3.20.3
```
#### Install ML-Agents

To install the Unity ML-Agents toolkit, you will need to run the following command.
This command downloads and installs the necessary package to enable integration with Unity,
allowing you to create and train machine learning agents in your Unity environments.

```shell
pip install mlagents
```

To verify if your ML-Agents installation was successful, you can run the following command:

```shell
mlagents-learn -h
```

If you see a list of commands, you have done everything correctly.

# Training the environment
Once ML-Agents has been installed onto your machine, you can begin training Wilbur's agent.
<br/>
1. Open a command or terminal window.
2. Navigate (CD) to the folder where you cloned the project repository.
3. Run `mlagents-learn config/cat_config.yaml --run-id=firstWilburRun`.
 - `config/cat_config.yaml` is the path to Wilbur's config file.
 - `run-id` is a unique name for this training session.
4. When the message "Start training by pressing the Play button in the Unity Editor" is displayed on the screen, you can press the Play button in Unity to start training in the Editor.
<br/>
Once a training session has been completed, the results will be saved to the `results` directory.
