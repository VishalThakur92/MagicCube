using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal {
    public class CubeCamera : MonoBehaviour
    {

        Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
            Initialize();
            guiStyle.fontSize = 40;
        }

        public void Initialize() {
            Globals.OnPinchInOut += OnZoom;
        }

        void OnDestroy() {
            Globals.OnPinchInOut -= OnZoom;
        }

        private GUIStyle guiStyle = new GUIStyle();
        //Globals.SwipeDirection latestSwipeDirection;
        private void OnGUI()
        {
            //GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
            //GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
            //GUILayout.Label($"Swipe Dir = {swipeDirection}", guiStyle);

            //foreach (string value in rotationSteps)
            //{
            //    GUILayout.Label($"ID : {value.Split('_')[0]} dir = {value.Split('_')[1]}", guiStyle);
            //}

            //GUILayout.Label($"Ortho size = {cam.orthographicSize}", guiStyle);

            //GUILayout.Label($"deltaMagnitudeDiff = {deltaMagDiff} speed = {sped}", guiStyle);


        }

        //private void Update()
        //{
        //    if (cam.orthographicSize < Globals.MinZoomBound)
        //    {
        //        cam.orthographicSize = Globals.MinZoomBound;
        //    }
        //    else if (cam.orthographicSize > Globals.MaxZoomBound)
        //    {
        //        cam.orthographicSize = Globals.MaxZoomBound;
        //    }
        //}

        float deltaMagDiff = 0, sped = 0;
        void OnZoom(float deltaMagnitudeDiff, float speed)
        {
            deltaMagDiff = deltaMagnitudeDiff;
            sped = speed;
            cam.orthographicSize += deltaMagnitudeDiff * speed;

            //Clamp Values to avoid overflow
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, Globals.MinZoomBound, Globals.MaxZoomBound);
        }
    }
}
