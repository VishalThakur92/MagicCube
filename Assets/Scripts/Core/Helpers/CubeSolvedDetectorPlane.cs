using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicCubeVishal
{
    //Used to detect Cube Faces on one of the Sides of the Magic Cube
    public class CubeSolvedDetectorPlane : MonoBehaviour
    {
        //Cube FAces detected by OnTriggerEnter()
        [SerializeField]
        public List<CubeFace> detectedCubeFaces = new List<CubeFace>();


        private void OnTriggerEnter(Collider other)
        {
            CubeFace newCubeFace = other.GetComponent<CubeFace>();

            //Add newly detected cube Face to the list
            if (newCubeFace && !detectedCubeFaces.Contains(newCubeFace))
                detectedCubeFaces.Add(newCubeFace);
        }


        private void OnDisable()
        {
            ClearDetectedCubeFaces();
        }


        //Clear Cache
        public void ClearDetectedCubeFaces()
        {
            detectedCubeFaces.Clear();
        }
    }
}
