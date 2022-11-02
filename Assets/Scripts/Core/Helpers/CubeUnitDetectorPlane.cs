using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    public class CubeUnitDetectorPlane : MonoBehaviour
    {
        [SerializeField]
        public List<CubeUnit> detectedCubeUnits = new List<CubeUnit>();


        private void OnTriggerEnter(Collider other)
        {
            CubeUnit newCubeUnit = other.GetComponent<CubeUnit>();

            if (newCubeUnit && !detectedCubeUnits.Contains(newCubeUnit))
                detectedCubeUnits.Add(newCubeUnit);
        }



        public void ClearDetectedCubeUnits()
        {
            detectedCubeUnits.Clear();
        }
    }
}