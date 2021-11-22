
# Rehabilination

A project done for the Facebook XR Hackathon 2021

  

![Rehabilination](https://i.imgur.com/YDfAEfb.png)

  

Rehabilination is a mixed realtiy experience that supports young hospitalized kids throughout their entire patient journey. Our goal is to increase motivation for kids' own rehabilitation.

  

This project is build for the Oculus Quest 2 with the help of Unity (2020.3.21f1) using both the Voice SDK and Passthrough capabilities of the Oculus Integration SDK (v34). Due to restrictions/issues with the XR Plugin Management and Passthrough not working on the alternative Legacy XR input SDK or OpenXR we unfortuanately were unable to make use of the Hands SDK.

  

## How does it work?

  

### Requirements  

The project is build with **Unity 2020.3.21f1** which would be advised to be used when running the app using Unity (alternatively you can use the provided APK to sideload the app). From there the app should be ran from the **MaineScene** within the **_Scenes** folder. The project makes use of the Voice SDK and experimental Passthrough feature. It's therefore required to have the **experimental features** enabled within your Oculus Quest Device. We also use Google Text to Speech API's so WiFi (internet) is also a requirement in order to fully experience the full project.


### How to play

When starting the app a character named *Reco Very* will appear in front of you which will then start a quick communications sequence using the Voice SDK in order to establish the wishes of the user.

As the AI is not completely trained to accept all responses yet we provided you with a list of possible answers to the questions you will receive within this sequence to get the best experience:


>Reco: Good morning champ! How are you feeling

Possible answers: **I am doing fine**   / **I am not feeling so well**

>Reco: Great! Ready for a challange? / I am sorry, let me try to cheer you up, Want to play a game?

Possible answers: **Yes, let's go!** / **Yes**

>Reco: Fantaaaaaastic, let's go! What would you like to do today?

Possible answers: **I want to play the cocunut game**

>Reco: Great, let's go nuts!

The app will then launch a minigame to incentivize hospitalized children to perform simple tasks to help make their recovery more enjoyable. The minigame currently implemented can be played by reaching your controller towards the cocunut trees located above your head and grabbing a cocunut using the **grip button**. You can then throw the cocunut into Reco's bucket by making a **throwing movement** and releasing the **grip button**. Along the way Reco will help you out with motivating words.