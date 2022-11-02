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


        //private void Update()
        //{
        //    if(Input.GetKeyUp(KeyCode.Space))
        //        StartCoroutine(IsSolvedBehaviour());
        //}
        private void Start()
        {
            //StartCoroutine(IsSolvedBehaviour());
            //IsSolved();
        }

        //Keep Rotating this magic Cube
        public void RotateCrazy() {
            StartCoroutine(RotateCrazyBehaviour());
        }

        IEnumerator RotateCrazyBehaviour() {
            while (true) {
                transform.Rotate(0, 6.0f * Globals.magicCubeCrazyRotationMultiplier * Time.deltaTime, 0);
                yield return null;
            }
        }

        public IEnumerator IsSolvedBehaviour()
        {
            int totalSolvedSides = 0;
            int i, j;
            //Enable Detector for all 6 sides
            for (i = 0; i < 6; i++)
            {
                //Enable the Detector plane
                cubeSolvedDetectorPlanes[i].gameObject.SetActive(true);

                //Wait for one frame, now the Detector plane will grab all the cubes
                yield return new WaitForSeconds(.1f);
                //yield return null;

                //Grab color of first grabbed cube
                Globals.CubeColor initialColor = cubeSolvedDetectorPlanes[i].detectedCubeFaces[0].color;

                //Debug.LogError($"{cubeSolvedDetectorPlanes[i]} detected {cubeSolvedDetectorPlanes[i].detectedCubeFaces.Count} cube faces");

                //Compare it against the rest of others
                //If this side is solved then all cube faces should have the same color
                for (j = 1; j < cubeSolvedDetectorPlanes[i].detectedCubeFaces.Count; j++)
                {
                    //A cube face did not have the same color, hence we consider this side is not solved and moreover the magic cube itself is not solved
                    if (cubeSolvedDetectorPlanes[i].detectedCubeFaces[j].color != initialColor)
                    {
                        isSolved = false;
                        //Disable All Detector Faces, so that we can re-detect upon re-enabling the detector planes
                        for (i = 0; i < 6; i++)
                        {
                            yield return null;
                            cubeSolvedDetectorPlanes[i].ClearDetectedCubeFaces();
                            cubeSolvedDetectorPlanes[i].gameObject.SetActive(false);
                        }
                        yield break;
                    }
                }

                //Total 6 sides, Mark one more side as being solved
                totalSolvedSides++;
            }

            //Check if all 6 sides are solved
            if (totalSolvedSides == 6)
                isSolved = true;
            else
                isSolved = false;


            //Disable All Detector Faces, so that we can re-detect upon re-enabling the detector planes
            for (i = 0; i < 6; i++)
            {
                yield return null;
                cubeSolvedDetectorPlanes[i].ClearDetectedCubeFaces();
                cubeSolvedDetectorPlanes[i].gameObject.SetActive(false);
            }
        }
    }
}
