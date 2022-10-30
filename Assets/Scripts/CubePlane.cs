using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlane : MonoBehaviour
{

    [SerializeField]
    public List<CubeUnit> detectedCubes = new List<CubeUnit>();

    private void OnTriggerEnter(Collider other)
    {
        CubeUnit newCube = other.GetComponent<CubeUnit>();
        if(newCube && !detectedCubes.Contains(newCube))
            detectedCubes.Add(newCube);
    }



    public void Clear() {
        detectedCubes.Clear();
    }

}
