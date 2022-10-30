using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{

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





    public static Action<SwipeDirection,bool> OnSwipe;
}
