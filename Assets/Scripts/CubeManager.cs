using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public enum RotationDirection {
        left,
        right,
        upLeft,
        downLeft,
        upRight,
        downRight
    }

    [SerializeField]
    CubeUnit selectedCubeUnit;

    [SerializeField]
    Transform rotator, rubikCube, detectorPlanes;

    [SerializeField]
    List<GameObject> detectedCubes = new List<GameObject>();

    [SerializeField]
    bool rotating = false;

    [SerializeField]
    float lerpDuration = 5f;


    [SerializeField]
    float rotationMulitplier = 90;

    #region Methods

    private void Start()
    {
        guiStyle.fontSize = 50; //change the font size
    }

    private GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
        GUILayout.Label($"Screen Width = {Screen.width/2}", guiStyle);
    }


    public void Update()
    {
        if (rotating)
            return;

        if (Input.GetKeyUp(KeyCode.A))
            StartCoroutine(RotateLeft());

        //if (Input.GetKeyUp(KeyCode.D))
        //    Rotate(Vector3.right);

        if (Input.GetKeyUp(KeyCode.W))
            StartCoroutine(RotateUpLeft());

        //if (Input.GetKeyUp(KeyCode.S))
        //    Rotate(Vector3.down);

    }


    IEnumerator RotateLeft()
    {

        rotating = true;

        detectorPlanes.position = selectedCubeUnit.transform.position;

        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(true);
        selectedCubeUnit.ToggleVerticalLeftPlane(false);
        selectedCubeUnit.ToggleVerticalRightPlane(false);

        yield return new WaitForSeconds(.1f);

        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.horizontalPlane.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }


        //Rotate the Rotator
        Rotate(RotationDirection.left);
    }

    IEnumerator RotateUpLeft()
    {
        rotating = true;
        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(false);
        selectedCubeUnit.ToggleVerticalLeftPlane(false);
        selectedCubeUnit.ToggleVerticalRightPlane(true);

        yield return new WaitForSeconds(.1f);

        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.verticalPlaneRight.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }


        //Rotate the Rotator
        Rotate(RotationDirection.upLeft);
    }


    public void Rotate(RotationDirection direction)
    {
        if (direction == RotationDirection.left)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        if (direction == RotationDirection.upLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(-rotationMulitplier,0 , 0)));
    }


    IEnumerator RotationBehaviour(Quaternion targetRotation)
    {
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;

        while (timeElapsed < lerpDuration)
        {
            rotator.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        rotator.rotation = targetRotation;
        yield return new WaitForEndOfFrame();
        //Put Detected Cubes back into Rubik Cube from Rotator
        // Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rubikCube, true);
            yield return null;
        }


        //Reset Rotator Rotation to zero
        rotator.rotation = Quaternion.Euler(0,0,0);
        rotating = false;
        Debug.LogError("Finish");
    }
    #endregion
}
