using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlane : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> detectedCubes = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(!detectedCubes.Contains(other.gameObject))
            detectedCubes.Add(other.gameObject);
    }


    public void Clear() {
        detectedCubes.Clear();
        gameObject.SetActive(false);
    }

}
