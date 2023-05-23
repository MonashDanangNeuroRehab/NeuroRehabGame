# Developers' notes - Game Components

## Feedback Game Manager
Feedback Game Manager initiates the games at each obstacle and "move" the player (for the details of how the player is moved, check "") and keep tracks of the total score. 
Potentially this script can be used to configure and monitor difficulty levels of the implemented games

## Goal Logic
Goal Logic controls the behavior when an obstacle is met by the player. It detects when the player reaches an obstacle to start the minigame and initiate the finishing sequence when the player finishes the minigame

## RehabMiniGame..
RehabMiniGame is where the logic of each minigame is implemented
Each game is extended upon an abstract class provided by ***LeapMotionSDK*** called ***PostProcessProvider***
***PostProcessProvider*** allows us to access the data of the frame collected by the Leap Motion 