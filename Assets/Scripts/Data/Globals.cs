using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    public static class Globals
    {
        #region Enumerations
        //Possible directions of Swipes in the Current Input System
        public enum SwipeDirection
        {
            up = 0,
            down,
            left,
            right,

            upLeft,
            upRight,
            downLeft,
            downRight
        }

        //Possible Cube Rotation Directions
        public enum CubeRotationDirection
        {
            left=0,
            right,
            upLeft,
            downLeft,
            upRight,
            downRight
        }


        //Possible types of Rubik Cubes in the game
        public enum CubeType
        {
            size2 = 0,
            size3,
            size4,
            size5,
            size6
        }

        //Possible types of Cube Unit colors in the Magic Cube
        public enum CubeColor
        {
            red = 0,
            green,
            blue,
            orange,
            white,
            yellow
        }
        #endregion


        #region Parameters

        //------CubeManager params
        //Minimum and Maximum Shuffle Steps can be used to Lower/Increase the Game Difficulty
        public static int MinimumShuffleSteps = 2;
        public static int MaximumShuffleSteps = 2;
        public static float secondsToWaitBeforeShuffle = .5f;
        public static int magicCubeCrazyRotationMultiplier = 20;


        //False - Game timer while playing game is hidden
        public static bool ShowGameTimer = true;



        //------Camera Related Params
        public static float MinZoomBound = .6f, MaxZoomBound =1.90f;



        //Game Related Params
        public static string gameOverMessage = "Well Done!! You solved the Magic Cube!! \n Time Taken :  ";
        #endregion


        #region Events

        //-----Game Events---------
        //Invoked when a Magic Cube's shuffling is Completed
        public static Action OnCubeShuffleComplete;

        //Invoked when a user solves the selected Magic Cube
        public static Action OnCubeSolved;

        //Invoked when a user knows he has solved the cube and he chooses to go back to the Main menu
        public static Action OnCubeSolvedAcknowledged;



        //-----Input Events---------
        //Invoked when a user does a swipe
        public static Action<SwipeDirection, bool> OnSwipe;

        //Invoked when a user taps down on scren
        public static Action OnPointerDown;

        //Invoked when a user lifts the taps from the scren
        public static Action OnPointerUp;

        //Invoked when a user lifts the taps from the scren
        public static Action<float,float> OnPinchInOut;
        #endregion
    }
}