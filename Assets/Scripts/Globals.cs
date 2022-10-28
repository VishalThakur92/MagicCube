using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{

    public enum SwipeDirection {
        up,
        down,
        left,
        right
    }

    public static SwipeDirection currentSwipeDirection;
}
