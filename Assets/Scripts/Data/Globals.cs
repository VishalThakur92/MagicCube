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
        #endregion


        #region Parameters
        //Minimum and Maximum Shuffle Steps can be used to Lower/Increase the Game Difficulty
        public static int MinimumShuffleSteps = 15;
        public static int MaximumShuffleSteps = 20;
        public static float secondsToWaitBeforeShuffle = .5f;
        #endregion


        #region Events
        //Invoked when a user does a swipe
        public static Action<SwipeDirection, bool> OnSwipe;

        //Invoked when a user taps down on scren
        public static Action OnPointerDown;

        //Invoked when a user lifts the taps from the scren
        public static Action OnPointerUp;

        #endregion
    }
}