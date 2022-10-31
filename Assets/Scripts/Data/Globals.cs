using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    #region Enumerations
    //Possible directions of Swipes in the Current Input System
    public enum SwipeDirection {
        none,

        up,
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
        left,
        right,
        upLeft,
        downLeft,
        upRight,
        downRight
    }


    //Possible types of Rubik Cubes in the game
    public enum CubeType {
        size2 = 0,
        size3,
        size4,
        size5,
        size6
    }
    #endregion


    #region Events
    //Invoked when a user does a swipe
    public static Action<SwipeDirection,bool> OnSwipe;
    #endregion
}
