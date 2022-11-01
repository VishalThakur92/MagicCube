using UnityEngine;
using System.Collections;

namespace MagicCubeVishal
{
    public class InputManager : MonoBehaviour
    {
        #region Params
        //Swipe related Params
        public float MinSwipeLength = 5;
        Vector2 _firstPressPos;
        Vector2 _secondPressPos;
        Vector2 _currentSwipe;


        //Current Swipe Direction
        Globals.SwipeDirection swipeDirection;


        //Pinch in/out related params
        float TouchZoomSpeed = 0.001f;
        #endregion


        #region Unity
        private void Start()
        {
            guiStyle.fontSize = 50; //change the font size
        }

        void Update()
        {
            DetectSwipe();

            DetectPinchInOut();

            DetectPointerUpDown();
        }
        #endregion



        private GUIStyle guiStyle = new GUIStyle();

        private void OnGUI()
        {
            //GUILayout.Label($"Swipe= {swipeDirection}", guiStyle);
        }

        class GetCardinalDirections
        {
            public static readonly Vector2 Up = new Vector2(0, 1);
            public static readonly Vector2 Down = new Vector2(0, -1);
            public static readonly Vector2 Right = new Vector2(1, 0);
            public static readonly Vector2 Left = new Vector2(-1, 0);

            public static readonly Vector2 UpRight = new Vector2(1, 1);
            public static readonly Vector2 UpLeft = new Vector2(-1, 1);
            public static readonly Vector2 DownRight = new Vector2(1, -1);
            public static readonly Vector2 DownLeft = new Vector2(-1, -1);
        }

        void DetectPointerUpDown() {

            if (Input.GetMouseButtonDown(0))
            {
                Globals.OnPointerDown?.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Globals.OnPointerUp?.Invoke();
            }
        }

        void DetectPinchInOut() {
            if (Input.touchSupported)
            {
                // Pinch to zoom
                if (Input.touchCount == 2)
                {
                    // get current touch positions
                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                    // get offset value
                    float deltaDistance = oldTouchDistance - currentTouchDistance;

                    //Broadcast Event
                    Globals.OnPinchInOut.Invoke(deltaDistance, TouchZoomSpeed);
                }
            }

        }

        public void DetectSwipe()
        {
            if (Input.touches.Length == 1)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                {
                    _firstPressPos = new Vector2(t.position.x, t.position.y);
                }

                if (t.phase == TouchPhase.Ended)
                {

                    _secondPressPos = new Vector2(t.position.x, t.position.y);
                    _currentSwipe = new Vector3(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);


                    // Make sure it was a legit swipe, not a tap
                    if (_currentSwipe.magnitude < MinSwipeLength)
                    {
                        // SwipeDirection = Swipe.None;
                        return;
                    }


                    _currentSwipe.Normalize();

                    // use dot product against 4 cardinal directions.
                    // return if one of them is > 0.5f;

                    print(_currentSwipe);

                    //compare north
                    if (Vector2.Dot(_currentSwipe, GetCardinalDirections.Up) > 0.906f)
                    {

                        swipeDirection = Globals.SwipeDirection.up;
                        print("Up!");
                        //return;
                    }
                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.Down) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.down;
                        print("Down!");
                        //return;
                    }
                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.Left) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.left;
                        print("Left");
                        //return;
                    }
                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.Right) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.right;
                        print("Right");
                        //return;
                    }

                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.UpRight) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.upRight;
                        print("UpRight");
                        //return;
                    }

                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.UpLeft) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.upLeft;
                        print("UpLeft");
                        //return;
                    }

                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.DownLeft) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.downLeft;
                        print("DownLeft");
                        //return;
                    }

                    else if (Vector2.Dot(_currentSwipe, GetCardinalDirections.DownRight) > 0.906f)
                    {
                        swipeDirection = Globals.SwipeDirection.downRight;
                        print("DownRight");
                        //return;
                    }

                    Globals.OnSwipe?.Invoke(swipeDirection, true);
                }

            }
        }
    }
}