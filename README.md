
# Rehabilination

A project done for the Facebook XR Hackathon 2021

  

![Rehabilination](https://i.imgur.com/YDfAEfb.png)

  

Rehabilination is a mixed reality experience that supports young hospitalized kids throughout their entire patient journey. The goal is to increase a young patient’s motivation for his or her own rehabilitation. An interactive digital character offers kids distraction, motivation, information and stress-relief before, during and after their hospitalization by using gamification and storytelling.

Video: https://vimeo.com/648713873/cf480480ed  

This project is build for the Oculus Quest 2 with the help of Unity (2020.3.21f1) using both the Voice SDK and Passthrough capabilities from the Presence Platform within the Oculus Integration SDK (v34). Due to restrictions/issues with the XR Plugin Management and Passthrough not working on the alternative Legacy XR input SDK or OpenXR we unfortuanately were unable to make use of the Hands SDK.

  

## How does it work?

  

### Requirements  

The project is build with **Unity 2020.3.21f1** which would be advised to be used when running the app using Unity (alternatively you can use the provided APK to sideload the app). From there the app should be ran from the **MaineScene** within the **_Scenes** folder. The project makes use of the Voice SDK and experimental Passthrough feature. It's therefore required to have the **experimental features** enabled within your Oculus Quest Device. Since we use the Voice SDK and the Google Text to Speech API's WiFi (internet) is also a requirement in order to fully experience the full project. During our development and filming moments we have disabled the guardian feature which we would recommend doing to get the best experience.


### How to play
The app scene is setup to mimic being in a hospital bed. It is therefore recommend to start this game while sitting in a chair with table infront of you.
When starting the app a character named *Rico Very* will appear in front of you which will then start a quick communications sequence using the Voice SDK in order to establish the wishes of the user.

As the AI is not completely trained to accept all responses yet we provided you with a list of possible answers to the questions you will receive within this sequence to get the best experience:


>Rico: Good morning champ! How are you feeling

Possible answers: **I am doing fine** / **I am not feeling so well**

>Rico: Great! Ready for a challange? / I am sorry, let me try to cheer you up, Want to play a game?

Possible answers: **Yes, let's go!** / **Yes**

>Rico: Fantaaaaaastic, let's go! What would you like to do today?

Possible answers: **I want to play the coconut game**

>Rico: Great, let's go nuts!

The app will then launch a minigame to incentivize and motivate hospitalized children to perform simple tasks to help make their entire patient journey more enjoyable. The minigame currently implemented can be played by reaching your controller towards the coconut trees located above your head and grabbing a coconut using the **grip button**. You can then throw the coconut into Rico's bucket by making a **throwing movement** and releasing the **grip button**. Get as much coconuts into the bucket before the timer runs out! Along the way Rico will help you out with motivating words.