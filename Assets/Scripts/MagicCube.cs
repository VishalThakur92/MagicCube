using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCube : MonoBehaviour
{
    //This Magic Cube's Type
    public Globals.CubeType cubeType;

    public Camera respectiveCamera;

    //All Cube units for this size of Rubik Cube
    [SerializeField]
    public List<CubeUnit> allCubeUnits = new List<CubeUnit>();

}
