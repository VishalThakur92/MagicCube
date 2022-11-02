using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicCubeVishal
{
    //Used to detect Cube Faces on one of the Sides of the Magic Cube
    public class CubeSolvedDetectorPlane : MonoBehaviour
    {
        [SerializeField]
        public List<CubeFace> detectedCubeFaces = new List<CubeFace>();


        private void OnTriggerEnter(Collider other)
        {
            CubeFace newCubeFace = other.GetComponent<CubeFace>();

            if (newCubeFace && !detectedCubeFaces.Contains(newCubeFace))
                detectedCubeFaces.Add(newCubeFace);
        }


        public void ClearDetectedCubeFaces()
        {
            detectedCubeFaces.Clear();
        }
    }
}
