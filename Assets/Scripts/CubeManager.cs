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

    [SerializeField]
    float mousePositionXOnInput , screenhalf;

    #region Methods

    private void Start()
    {
        guiStyle.fontSize = 50; //change the font size
        screenhalf = Screen.width / 2;
    }

    private GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
        GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
        GUILayout.Label($"Swipe Dir = {Globals.currentSwipeDirection}", guiStyle);
    }

    void GrabSelectedCubeUnit() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.LogError(hit.transform.name);
            selectedCubeUnit = hit.transform.GetComponent<CubeUnit>();
            mousePositionXOnInput = Input.mousePosition.x;

        }
    }

    public void Update()
    {
        if (rotating)
            return;

        if (Input.GetMouseButtonUp(0)) {
            GrabSelectedCubeUnit();
        }

        if (Input.GetKeyUp(KeyCode.A))
            StartCoroutine(RotateLeft());

        //if (Input.GetKeyUp(KeyCode.D))
        //    Rotate(Vector3.right);

        if (Input.GetKeyUp(KeyCode.W))
        {
            if (mousePositionXOnInput > screenhalf)
                StartCoroutine(RotateUpLeft());
            else
                StartCoroutine(RotateUpRight());
        }

        //if (Input.GetKeyUp(KeyCode.S))
        //    Rotate(Vector3.down);

    }


    IEnumerator RotateLeft()
    {
        //GrabSelectedCubeUnit();

           rotating = true;

        detectorPlanes.position = selectedCubeUnit.transform.position;

        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(true);
        selectedCubeUnit.ToggleVerticalLeftPlane(false);
        selectedCubeUnit.ToggleVerticalRightPlane(false);

        yield return new WaitForSeconds(.1f);

        detectedCubes.Clear();
        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.horizontalPlane.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }

        selectedCubeUnit.horizontalPlane.Clear();


        

        //Rotate the Rotator
        Rotate(RotationDirection.left);
    }

    IEnumerator RotateUpLeft()
    {
        //GrabSelectedCubeUnit();
        rotating = true;


        detectorPlanes.position = selectedCubeUnit.transform.position;

        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(false);
        selectedCubeUnit.ToggleVerticalLeftPlane(false);
        selectedCubeUnit.ToggleVerticalRightPlane(true);

        yield return new WaitForSeconds(.1f);

        detectedCubes.Clear();
        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.verticalPlaneRight.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }

        selectedCubeUnit.verticalPlaneRight.Clear();

        //Rotate the Rotator
        Rotate(RotationDirection.upLeft);
    }

    IEnumerator RotateUpRight()
    {
        //GrabSelectedCubeUnit();
        rotating = true;


        detectorPlanes.position = selectedCubeUnit.transform.position;

        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(false);
        selectedCubeUnit.ToggleVerticalLeftPlane(true);
        selectedCubeUnit.ToggleVerticalRightPlane(false);

        yield return new WaitForSeconds(.1f);

        detectedCubes.Clear();

        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.verticalPlaneLeft.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }

        selectedCubeUnit.verticalPlaneLeft.Clear();

        //Rotate the Rotator
        Rotate(RotationDirection.upRight);
    }


    public void Rotate(RotationDirection direction)
    {
        if (direction == RotationDirection.left)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        if (direction == RotationDirection.upLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
        if (direction == RotationDirection.upRight)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier)));
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