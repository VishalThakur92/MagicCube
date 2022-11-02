using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    //The Magic Cube Class
    public class MagicCube : MonoBehaviour
    {
        #region Params
        //This Magic Cube's Type
        public Globals.CubeType cubeType;

        //The Camera that is tuned to Render this Magic Cube
        public Camera respectiveCamera;

        //Is this Cube solved
        public bool isSolved = false;

        //List of all Detector planes that determine if the Cube is solved
        [SerializeField]
        List<CubeSolvedDetectorPlane> cubeSolvedDetectorPlanes = new List<CubeSolvedDetectorPlane>();

        //All Cube units for this size of Rubik Cube
        [SerializeField]
        public List<CubeUnit> allCubeUnits = new List<CubeUnit>();
        #endregion


        #region Core

        //Keep Rotating this magic Cube
        public void RotateCrazy() {
            StartCoroutine(RotateCrazyBehaviour());
        }

        //Async Rotation of this Magic Cube, Called upon Cube Solved
        IEnumerator RotateCrazyBehaviour() {
            while (true) {
                transform.Rotate(0, 6.0f * Globals.magicCubeCrazyRotationMultiplier * Time.deltaTime, 0);
                yield return null;
            }
        }


        //Determine if this magic cube is solved with the help of detector planes
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
        #endregion
    }
}
