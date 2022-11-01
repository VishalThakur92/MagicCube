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
        }

        public void Initialize() {
            Globals.OnPinchInOut += OnZoom;
        }

        void OnDestroy() {
            Globals.OnPinchInOut -= OnZoom;
        }


        void OnZoom(float deltaMagnitudeDiff, float speed)
        {
            cam.orthographicSize += deltaMagnitudeDiff * speed;

            //Clamp Values to avoid overflow
            cam.orthographicSize = Mathf.Clamp(cam.fieldOfView, Globals.MinZoomBound, Globals.MaxZoomBound);


            if (cam.fieldOfView < Globals.MinZoomBound)
            {
                cam.orthographicSize = Globals.MinZoomBound;
            }
            else
            if (cam.fieldOfView > Globals.MaxZoomBound)
            {
                cam.orthographicSize = Globals.MaxZoomBound;
            }
        }
    }
}
