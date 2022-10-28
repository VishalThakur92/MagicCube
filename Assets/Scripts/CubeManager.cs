using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    CubeUnit selectedCubeUnit;

    [SerializeField]
    Transform rotator, rubikCube;

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

        //if (Input.GetKeyUp(KeyCode.W))
        //    StartCoroutine(RotateUp());

        //if (Input.GetKeyUp(KeyCode.S))
        //    Rotate(Vector3.down);

    }


    IEnumerator RotateLeft()
    {
        rotating = true;
        //Enable Horizontal Plane for selected Cube unit
        selectedCubeUnit.ToggleHorizontalPlane(true);
        selectedCubeUnit.ToggleVerticalPlane(false);

        yield return new WaitForSeconds(.1f);

        //Grab all Horizontal Cube units
        detectedCubes.AddRange(selectedCubeUnit.horizontalPlane.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
            yield return null;
        }


        //Rotate the Rotator to left
        Rotate(Vector3.left);
    }


    public void Rotate(Vector3 direction)
    {
        if (direction == Vector3.left)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        else if (direction == Vector3.right)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0)));
        if (direction == Vector3.up)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier)));
        else if (direction == Vector3.down)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
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
