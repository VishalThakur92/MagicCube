using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    List<CubeUnit> allCubes = new List<CubeUnit>();

    public enum RotationDirection {
        left,
        right,
        upLeft,
        downLeft,
        upRight,
        downRight
    }


    [SerializeField]
    List<string> recordedSteps = new List<string>();

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
        //GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
        //GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
        //GUILayout.Label($"Swipe Dir = {swipeDirection}", guiStyle);

        foreach (string value in recordedSteps)
        {
            GUILayout.Label($"ID : {value.Split('_')[0]} dir = {value.Split('_')[1]}", guiStyle);
        }
    }


    IEnumerator GrabSelectedCubeUnit() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            selectedCubeUnit = hit.transform.GetComponent<CubeUnit>();

            if (selectedCubeUnit != null)
            {

                detectedCubes.Clear();
                Debug.LogError(hit.transform.name);
                detectorPlanes.position = selectedCubeUnit.transform.position;
                yield return null;
                selectedCubeUnit.ToggleAllPlanes(true);
                mousePositionXOnInput = Input.mousePosition.x;


                DebugRaydirection = hit.normal;

                LayerMask ignoreLayers = 1 << 3;

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

        }
        else {
           // selectedCubeUnit = null;
        }
    }

    Vector3 DebugRaydirection;

    public void Update()
    {
        if(selectedCubeUnit)
            Debug.DrawLine(selectedCubeUnit.transform.position, DebugRaydirection, Color.green);

        if (rotating)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(GrabSelectedCubeUnit());
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectedCubeUnit?.ToggleAllPlanes(false);
        }

        if (Input.GetKeyUp(KeyCode.Space))
            Undo();

        if (Input.GetKeyUp(KeyCode.D))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.right,true);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.left, true);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.up, true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.down, true);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upLeft, true);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upRight, true);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downLeft, true);
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downRight, true);
        }

        //if (Input.GetKeyUp(KeyCode.S))
        //    Rotate(Vector3.down);

    }

    void Undo() {
        if (rotating || recordedSteps.Count == 0)
            return;


        rotating = true;
        //Grab Cube ID
        string directionData = recordedSteps[recordedSteps.Count - 1].Split('_')[1];


        

        //Get Rotation Direction
        RotationDirection parsed_enum = (RotationDirection)System.Enum.Parse(typeof(RotationDirection), directionData);

        string cubeData = recordedSteps[recordedSteps.Count - 1].Split('_')[0];
        //List<CubeUnit> cubesFromThePast = new List<CubeUnit>();
        string[] cubeIDs = cubeData.Split('.');
        for (int i = 0; i< cubeIDs.Length; i++)
        {
            CubeUnit newCube = allCubes[int.Parse(cubeIDs[i])];
            detectedCubes.Add(newCube);
            newCube.transform.SetParent(rotator, true);
        }
        
        Rotate(GetReverseDirection(parsed_enum));


        recordedSteps.RemoveAt(recordedSteps.Count - 1);
    }

    void OnSwipeRotate(RotationDirection direction, CubePlane plane) {


        RecordCubeRotation(plane.detectedCubes , direction);

        //Grab all Horizontal Cube units
        detectedCubes.AddRange(plane.detectedCubes);

        //Set Rotator as their parent
        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(rotator, true);
        }

        //Rotate the Rotator
        Rotate(direction);
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
            //yield return null;
        }


        //Reset Rotator Rotation to zero
        rotator.rotation = Quaternion.Euler(0,0,0);
        isTopFaceSelected = false;
        selectedCubeUnit?.ToggleAllPlanes(false);
        selectedCubeUnit?.ClearAllPlanesData();
        selectedCubeUnit = null;

        detectedCubes.Clear();
        Debug.LogError("Finish");
        rotating = false;
    }

    #endregion

    #region Callbacks
    void OnSwipe(Globals.SwipeDirection direction,bool isActualSwipe) {
        swipeDirection = direction;
        //avoid multiple inputs
        if (rotating)
            return;

        if (selectedCubeUnit == null)
        {
            //Rotate Camera as per Swipe
            return;
        }


        rotating = true;

        ////Record Input
        //if (isActualSwipe)
        //{
        //    //RecordCubeRotation();
        //}

        //Rotate Cube as per Swipe
        switch (swipeDirection)
        {
            case Globals.SwipeDirection.up:
                if(isCubePlacedInLeftScreen())
                    OnSwipeRotate(RotationDirection.upRight, selectedCubeUnit.verticalPlaneLeft);
                else
                    OnSwipeRotate(RotationDirection.upLeft, selectedCubeUnit.verticalPlaneRight);
                break;

            case Globals.SwipeDirection.down:
                if (isCubePlacedInLeftScreen())
                    OnSwipeRotate(RotationDirection.downLeft, selectedCubeUnit.verticalPlaneLeft);
                else
                    OnSwipeRotate(RotationDirection.downRight, selectedCubeUnit.verticalPlaneRight);
                break;
            case Globals.SwipeDirection.left:
                if (isTopFaceSelected)
                    OnSwipeRotate(RotationDirection.downLeft, selectedCubeUnit.verticalPlaneLeft);
                else
                    OnSwipeRotate(RotationDirection.left,selectedCubeUnit.horizontalPlane);
                break;
            case Globals.SwipeDirection.right:
                if (isTopFaceSelected)
                    OnSwipeRotate(RotationDirection.upRight, selectedCubeUnit.verticalPlaneLeft);
                else
                    OnSwipeRotate(RotationDirection.right, selectedCubeUnit.horizontalPlane);
                break;
            case Globals.SwipeDirection.upLeft:
                OnSwipeRotate(RotationDirection.upLeft, selectedCubeUnit.verticalPlaneRight);
                break;
            case Globals.SwipeDirection.downRight:
                OnSwipeRotate(RotationDirection.downRight, selectedCubeUnit.verticalPlaneRight);
                break;
            case Globals.SwipeDirection.upRight:
                OnSwipeRotate(RotationDirection.upRight, selectedCubeUnit.verticalPlaneLeft);
                break;
            case Globals.SwipeDirection.downLeft:
                OnSwipeRotate(RotationDirection.downLeft, selectedCubeUnit.verticalPlaneLeft);
                break;
        }

        latestSwipeDirection = swipeDirection;
    }




    void RecordCubeRotation(List<CubeUnit> cubes,RotationDirection direction) {
        string value = "";
        //Record Cube IDs
        for (int i = 0; i < cubes.Count; i++) {
            value += cubes[i].uniqueID;
            if(i<cubes.Count-1)
               value += ".";
        }

        //Record rotationDirection
        value += "_" + System.Enum.GetName(typeof(RotationDirection), direction);


        Debug.LogError("Record Entry = " + value);

        recordedSteps.Add(value);
    }
    #endregion


    #region utility

    bool isCubePlacedInLeftScreen()
    {
        if (mousePositionXOnInput > screenhalf)
            return false;
        else
            return true;
    }


    RotationDirection GetReverseDirection(RotationDirection direction)
    {
        switch (direction)
        {
            case RotationDirection.left:
                return RotationDirection.right;

            case RotationDirection.right:
                return RotationDirection.left;

            case RotationDirection.upLeft:
                return RotationDirection.downRight;

            case RotationDirection.downRight:
                return RotationDirection.upLeft;

            case RotationDirection.upRight:
                return RotationDirection.downLeft;

            case RotationDirection.downLeft:
                return RotationDirection.upRight;

            default:
                Debug.LogError("Unable to get Reverse Direction");
                return RotationDirection.left;
        }
    }
    #endregion



    #region Deprecated Methods
    //IEnumerator RotateLeft()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.horizontalPlane.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //        //yield return null;
    //    }
    //    yield return null;

    //    //Rotate the Rotator
    //    Rotate(RotationDirection.left);
    //}

    //IEnumerator RotateRight()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.horizontalPlane.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //    }
    //    yield return null;

    //    //selectedCubeUnit.horizontalPlane.Clear();

    //    //Rotate the Rotator
    //    Rotate(RotationDirection.right);
    //}

    //IEnumerator RotateUpLeft()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.verticalPlaneRight.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //    }
    //    yield return null;

    //    //selectedCubeUnit.verticalPlaneRight.Clear();

    //    //Rotate the Rotator
    //    Rotate(RotationDirection.upLeft);
    //}

    //IEnumerator RotateUpRight()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.verticalPlaneLeft.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //    }
    //    yield return null;


    //    //Rotate the Rotator
    //    Rotate(RotationDirection.upRight);
    //}

    //IEnumerator RotateDownRight()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.verticalPlaneRight.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //    }

    //    yield return null;
    //    //selectedCubeUnit.verticalPlaneRight.Clear();

    //    //Rotate the Rotator
    //    Rotate(RotationDirection.downRight);
    //}

    //IEnumerator RotateDownLeft()
    //{
    //    //Grab all Horizontal Cube units
    //    detectedCubes.AddRange(selectedCubeUnit.verticalPlaneLeft.detectedCubes);

    //    //Set Rotator as their parent
    //    for (int i = 0; i < detectedCubes.Count; i++)
    //    {
    //        detectedCubes[i].transform.SetParent(rotator, true);
    //    }
    //    yield return null;

    //    //Rotate the Rotator
    //    Rotate(RotationDirection.downLeft);
    //}
    #endregion
}
