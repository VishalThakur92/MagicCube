using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUnit : MonoBehaviour
{
    [SerializeField]
    public CubePlane horizontalPlane,verticalPlane;


    public void ToggleHorizontalPlane(bool flag)
    {
        horizontalPlane.gameObject.SetActive(flag);
    }

    public void ToggleVerticalPlane(bool flag)
    {
        verticalPlane.gameObject.SetActive(flag);
    }
}
