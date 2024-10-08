# Cat Simulator (WIP): Nurturing a Self-Sustaining Agent Using Reinforcement Learning

This project is dedicated to Wilbur, forever a friend to those of Ralph Road and a good boy.

<p align="center">
  <img src="AdditionalFiles/Images/wilbur.jpeg" alt="Wilbur basking in the sunlight." height="450" />
</p>

# About this project

Cat Simulator was born during my final year of university as the project that I developed as part of my dissertation. The simulation is comprised of a cat, "Wilbur", and three objects, a scratching post, a water bowl, and food. Wilbur is a machine-learning agent whose behaviour derives from a reinforcement learning script that I developed using the Unity ML-Agents package within the Unity Engine.

The premise of the simulation is that Wilbur is an agent with three needs; Hunger, Thirst, and Happiness. At every step during training, these needs decrease. Wilbur's objective is to keep his needs well-maintained by navigating his environment and using an object to replenish its respective needs. The more efficiently he does this, the more rewards he will receive. If Wilbur does not successfully maintain these needs, he will receive a punishment.

In its current form, Wilbur can only be trained within Unity. Upon pressing play you can observe Wilbur begin knowing nothing about his environment to navigate between the objects and maintain his needs.

# Setting Up

## Cloning the project
To tinker with this project, you must clone this repository and import it into Unity. This guide assumes that you are familiar with the Unity engine. If not, follow [this guide](https://learn.unity.com/tutorial/install-the-unity-hub-and-editor#662942dfedbc2a0315217028) to install the Unity Hub and Editor

In a directory of your choosing, run the following command:
<br/><br/>
`git clone https://github.com/sunnyCallum/self-sufficient-cat-agent`
<br/><br/>
It does not matter where you clone it, just make sure you can locate it within your file explorer.
<br/><br/>

# Adding the project to Unity Hub
Open Unity Hub, click "Add" then click "Add project from disk". Navigate to where you cloned this project and add the folder. The project will then appear in your project list.

# Setting up Unity ML-Agents

Please follow [this guide](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md) to set up the Unity ML-Agents toolkit on your machine.

# Training the environment
Once ML-Agents has been installed onto your machine, you can begin training Wilbur's agent.
<br/>
1. Open a command or terminal window.
2. Navigate to the folder where you cloned the ml-agents repository. Note: If you followed the default installation, then you should be able to run mlagents-learn from any directory.
3. Copy the `cat_config.yaml` file from the `AdditionalFiles/ConfigFiles` directory in the Git clone and paste it into `ml-agents/config`
4. Run `mlagents-learn config/cat_config.yaml --run-id=firstWilburRun`.
 - `config/cat_config.yaml` is the path to Wilbur's config file.
 - `run-id` is a unique name for this training session.
5. When the message "Start training by pressing the Play button in the Unity Editor" is displayed on the screen, you can press the Play button in Unity to start training in the Editor.
