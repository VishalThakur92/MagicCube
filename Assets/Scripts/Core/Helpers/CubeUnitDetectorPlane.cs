using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    //Used to detect Cube Units one of the Magic Cube on an AXIS
    public class CubeUnitDetectorPlane : MonoBehaviour
    {

        //Detected Cube units via OnTriggerEnter()
        [SerializeField]
        public List<CubeUnit> detectedCubeUnits = new List<CubeUnit>();


        private void OnTriggerEnter(Collider other)
        {
            CubeUnit newCubeUnit = other.GetComponent<CubeUnit>();

            if (newCubeUnit && !detectedCubeUnits.Contains(newCubeUnit))
                detectedCubeUnits.Add(newCubeUnit);
        }


        //Clear Cache
        public void ClearDetectedCubeUnits()
        {
            detectedCubeUnits.Clear();
        }
    }
}