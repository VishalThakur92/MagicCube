using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUnit : MonoBehaviour
{
    [SerializeField]
    public CubePlane horizontalPlane, verticalPlaneLeft, verticalPlaneRight;


    public void ToggleHorizontalPlane(bool flag)
    {
        horizontalPlane.gameObject.SetActive(flag);
    }

    public void ToggleVerticalLeftPlane(bool flag)
    {
        verticalPlaneLeft.gameObject.SetActive(flag);
    }
    public void ToggleVerticalRightPlane(bool flag)
    {
        verticalPlaneRight.gameObject.SetActive(flag);
    }

    public void ToggleAllPlanes(bool flag)
    {
        horizontalPlane.gameObject.SetActive(flag);
        verticalPlaneLeft.gameObject.SetActive(flag);
        verticalPlaneRight.gameObject.SetActive(flag);
    }

    public void ClearAllPlanesData() {
        horizontalPlane.Clear();
        verticalPlaneLeft.Clear();
        verticalPlaneRight.Clear();
    }
}
