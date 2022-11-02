using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    public class MagicCube : MonoBehaviour
    {
        //This Magic Cube's Type
        public Globals.CubeType cubeType;

        public Camera respectiveCamera;


        //Is this Cube solved
        public bool isSolved = false;

        [SerializeField]
        List<CubeSolvedDetectorPlane> cubeSolvedDetectorPlanes = new List<CubeSolvedDetectorPlane>();

        //All Cube units for this size of Rubik Cube
        [SerializeField]
        public List<CubeUnit> allCubeUnits = new List<CubeUnit>();

        private void Start()
        {
            StartCoroutine(IsSolvedBehaviour());
            //IsSolved();
        }


        IEnumerator IsSolvedBehaviour()
        {
            int totalSolvedSides = 0;
            int i, j;
            //Enable Detector for all 6 sides
            for (i = 0; i < 6; i++)
            {
                cubeSolvedDetectorPlanes[i].gameObject.SetActive(true);

                yield return new WaitForEndOfFrame();

                Globals.CubeColor initialColor = cubeSolvedDetectorPlanes[i].detectedCubeFaces[0].color;

                for (j = 0; j < 4; j++)
                {
                    //Is not solved
                    if (cubeSolvedDetectorPlanes[i].detectedCubeFaces[j].color != initialColor)
                    {
                        isSolved = false;
                        yield break;
                    }
                }

                //Has checked on side Now move on to next side
                totalSolvedSides++;
            }

            //Check if all 6 sides are solved
            if (totalSolvedSides == 6)
                isSolved = true;
            else
                isSolved = false;

            for (i = 0; i < 6; i++)
            {
                cubeSolvedDetectorPlanes[i].gameObject.SetActive(false);
                cubeSolvedDetectorPlanes[i].ClearDetectedCubeFaces();
                yield return null;
            }
        }

        void IsSolved()
        {
            int totalSolvedSides = 0;
            int i, j;
            //Enable Detector for all 6 sides
            for (i = 0; i < 6; i++)
            {
                cubeSolvedDetectorPlanes[i].gameObject.SetActive(true);
                //yield return new WaitForEndOfFrame();

                //yield return new WaitForEndOfFrame();

                Globals.CubeColor initialColor = cubeSolvedDetectorPlanes[i].detectedCubeFaces[0].color;

                for (j = 0; j < 4; j++)
                {
                    //Is not solved
                    if (cubeSolvedDetectorPlanes[i].detectedCubeFaces[j].color != initialColor)
                    {
                        isSolved = false;
                        //yield break;
                    }
                }

                //Has checked on side Now move on to next side
                totalSolvedSides++;
            }

            //Check if all 6 sides are solved
            if (totalSolvedSides == 6)
                isSolved = true;
            else
                isSolved = false;
        }
    }
}
