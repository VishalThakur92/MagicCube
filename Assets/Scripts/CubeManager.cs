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
    List<CubeUnit> detectedCubes = new List<CubeUnit>();

    [SerializeField]
    bool rotating = false;

    [SerializeField]
    float lerpDuration = 5f;


    [SerializeField]
    float rotationMulitplier = 90;

    [SerializeField]
    float mousePositionXOnInput , screenhalf;


    [SerializeField]
    bool isTopFaceSelected = false;


    Globals.SwipeDirection swipeDirection = Globals.SwipeDirection.none;
    //int layer_mask;
    #region Methods

    private void Start()
    {
        guiStyle.fontSize = 50; //change the font size

        //Calculate Screen Width's half
        screenhalf = Screen.width/2;

        //Subscribe to Event Brodcasts
        Globals.OnSwipe += OnSwipe;

        //layer_mask = LayerMask.GetMask("TopFaceDetector");
    }

    private void OnDestroy()
    {
        Globals.OnSwipe -= OnSwipe;
    }

    private GUIStyle guiStyle = new GUIStyle();
    Globals.SwipeDirection latestSwipeDirection;
    private void OnGUI()
    {
        GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
        GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
        GUILayout.Label($"Swipe Dir = {swipeDirection}", guiStyle);
    }


    IEnumerator GrabSelectedCubeUnit() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {

            Debug.LogError(hit.transform.name);
            selectedCubeUnit = hit.transform.GetComponent<CubeUnit>();
            mousePositionXOnInput = Input.mousePosition.x;

            DebugRaydirection = hit.normal;

            LayerMask ignoreLayers = 1<<3;

            yield return new WaitForEndOfFrame();
            if (selectedCubeUnit && Physics.Raycast(selectedCubeUnit.transform.position, hit.normal, out RaycastHit hit2, 100f, ignoreLayers))
            {
                Debug.LogError($"name = {hit2.transform.name} layer = {hit2.transform.gameObject.layer}");
                if (hit2.transform.gameObject.layer == 3)
                {
                    isTopFaceSelected = true;
                    Debug.LogError("Top Face Seleced " + hit2.transform.name);
                }
            }
            else
            {
                isTopFaceSelected = false;
                Debug.LogError("Nothing");
            }

        }
        else {
            selectedCubeUnit = null;
        }
    }

    Vector3 DebugRaydirection;

    public void Update()
    {
        if(selectedCubeUnit)
            Debug.DrawLine(selectedCubeUnit.transform.position, DebugRaydirection, Color.green);

        if (rotating)
            return;

        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(GrabSelectedCubeUnit());
        }

        if (Input.GetKeyUp(KeyCode.Space))
            StartCoroutine(RotateDownLeft());

        if (Input.GetKeyUp(KeyCode.D))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.right);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.left);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.up);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.down);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upLeft);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upRight);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downLeft);
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downRight);
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

    IEnumerator RotateRight()
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
        Rotate(RotationDirection.right);
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

    IEnumerator RotateDownRight()
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
        Rotate(RotationDirection.downRight);
    }

    IEnumerator RotateDownLeft()
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
        Rotate(RotationDirection.downLeft);
    }


    public void Rotate(RotationDirection direction)
    {
        if (direction == RotationDirection.left)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        else if (direction == RotationDirection.right)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0)));

        else if (direction == RotationDirection.upLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
        else if (direction == RotationDirection.downRight)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0)));

        else if (direction == RotationDirection.downLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
        else if (direction == RotationDirection.upRight)
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
        isTopFaceSelected = false;
        selectedCubeUnit = null;
        Debug.LogError("Finish");
    }

    #endregion

    #region Callbacks
    void OnSwipe(Globals.SwipeDirection direction) {
        swipeDirection = direction;
        //avoid multiple inputs
        if (rotating)
            return;

        if (selectedCubeUnit == null)
        {
            //Rotate Camera as per Swipe
            return;
        }

        //Rotate Cube as per Swipe
        switch (swipeDirection)
        {
            case Globals.SwipeDirection.up:
                if(isCubePlacedInLeftScreen())
                    StartCoroutine(RotateUpRight());
                else
                    StartCoroutine(RotateUpLeft());
                break;

            case Globals.SwipeDirection.down:
                if (isCubePlacedInLeftScreen())
                    StartCoroutine(RotateDownLeft());
                else
                    StartCoroutine(RotateDownRight());
                break;
            case Globals.SwipeDirection.left:
                if (isTopFaceSelected)
                    StartCoroutine(RotateDownLeft());
                else
                    StartCoroutine(RotateLeft());
                break;
            case Globals.SwipeDirection.right:
                if (isTopFaceSelected)
                    StartCoroutine(RotateUpRight());
                else
                    StartCoroutine(RotateRight());
                break;
            case Globals.SwipeDirection.upLeft:
                StartCoroutine(RotateUpLeft());
                break;
            case Globals.SwipeDirection.downRight:
                StartCoroutine(RotateDownRight());
                break;
            case Globals.SwipeDirection.upRight:
                StartCoroutine(RotateUpRight());
                break;
            case Globals.SwipeDirection.downLeft:
                StartCoroutine(RotateDownLeft());
                break;
        }

        latestSwipeDirection = swipeDirection;
    }

    bool isCubePlacedInLeftScreen() {
        if (mousePositionXOnInput > screenhalf)
            return false;
        else
            return true;
    }

    //bool isTopMostCube() {

    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, Vector3.left, out hit, 100))
    //    {
    //        Debug.LogError("Cube not at top, Obj at top = " + hit.transform.name);
    //        return false;
    //    }
    //    else
    //    {
    //        Debug.LogError("Top Most Cube seleced");
    //        return true;
    //    }

    //    //Raycast on top of the selected cube
    //    //If hit returns another cube then return false
    //    //If hit returns nothing cube then return true
    //}
    #endregion
}
