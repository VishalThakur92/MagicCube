using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{

    #region Parameters
    //Singelton Instance
    public static CubeManager Instance { get; private set; }

    //The Currently selected Magic Cube
    public MagicCube currentMagicCube;

    [SerializeField]
    List<MagicCube> allMagicCubes = new List<MagicCube>();

    //The Steps being Recorded with every Cube move
    List<string> recordedSteps = new List<string>();


    //Selected Magic Cube's Individual Cube Unit
    CubeUnit selectedCubeUnit;


    [Space(10)]
    [SerializeField]
    //Cube Units detected via Detector planes are placed inside rotator, and Rotator is rotated by a specific degrees in the specified Input Direction
    Transform rotator;

    //Reference of the Cubes detected by detector plane, these will be placed inside rotator to be rotated in the specified Input Direction
    [SerializeField]
    List<CubeUnit> detectedCubes = new List<CubeUnit>();

    //Is the Magic Cube rotating via a given Input Direction
    [SerializeField]
    bool rotating = false;


    //Time it takes make on Magic cube's Side rotation
    [SerializeField]
    float lerpDuration = 5f;

    //Magic Cube's Side is rotated by this Parameter
    [SerializeField]
    float rotationMulitplier = 90;

    //Mouse position in X-axis when user selects a Cube unit
    float mousePositionXOnInput;

    //the half of the Current Screen's Width
    float screenhalf;

    //Did user select a cube unit on the TOP face of the Magic Cube
    [SerializeField]
    bool isTopFaceSelected = false;



    //Detector planes are used to Grab all corressponding cubes in the specified Input Direction
    [Space(10),SerializeField]
    Transform detectorPlanes;
    [SerializeField]
    public CubePlane horizontalPlane, verticalPlaneLeft, verticalPlaneRight;
    #endregion


    #region Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //change the font size
        guiStyle.fontSize = 50; 

        //Calculate Screen Width's half
        screenhalf = Screen.width/2;
    }


    public void Initialize(Globals.CubeType cubeType) {
        //Disable all magic Cubes
        for (int i = 0; i < allMagicCubes.Count; i++)
            allMagicCubes[i].gameObject.SetActive(false);

        currentMagicCube = allMagicCubes[(int)cubeType];

        //Enable the currently selected Magic Cube type
        currentMagicCube.gameObject.SetActive(true);

        //Subscribe to Event Brodcasts
        Globals.OnSwipe += OnSwipe;
        Globals.OnPointerDown += OnTryGrabCubeUnit;
        Globals.OnPointerUp += OnPointerUp;

    }


    private void OnDestroy()
    {
        Globals.OnSwipe -= OnSwipe;
    }

    private GUIStyle guiStyle = new GUIStyle();
    //Globals.SwipeDirection latestSwipeDirection;
    private void OnGUI()
    {
        //GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
        //GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
        //GUILayout.Label($"Swipe Dir = {swipeDirection}", guiStyle);

        //foreach (string value in recordedSteps)
        //{
        //    GUILayout.Label($"ID : {value.Split('_')[0]} dir = {value.Split('_')[1]}", guiStyle);
        //}
    }


    IEnumerator GrabSelectedCubeUnit() {
        Ray ray = currentMagicCube.respectiveCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            selectedCubeUnit = hit.transform.GetComponent<CubeUnit>();

            if (selectedCubeUnit != null)
            {

                detectedCubes.Clear();
                Debug.LogError(hit.transform.name);
                detectorPlanes.position = selectedCubeUnit.transform.position;
                yield return null;
                ToggleAllPlanes(true);
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


    //Controls for Editor testing
    public void Update()
    {
        if(selectedCubeUnit)
            Debug.DrawLine(selectedCubeUnit.transform.position, DebugRaydirection, Color.green);

        if (rotating)
            return;


        if (Input.GetKeyUp(KeyCode.Space))
            Undo();

        if (Input.GetKeyUp(KeyCode.D))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.right,true);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.left, true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.up, true);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.down, true);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upLeft, true);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.upRight, true);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downLeft, true);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            Globals.OnSwipe.Invoke(Globals.SwipeDirection.downRight, true);
        }

    }

    public void Undo() {
        if (rotating || recordedSteps.Count == 0)
            return;


        rotating = true;
        //Grab Cube ID
        string directionData = recordedSteps[recordedSteps.Count - 1].Split('_')[1];


        

        //Get Rotation Direction
        Globals.CubeRotationDirection parsed_enum = (Globals.CubeRotationDirection)System.Enum.Parse(typeof(Globals.CubeRotationDirection), directionData);

        string cubeData = recordedSteps[recordedSteps.Count - 1].Split('_')[0];
        //List<CubeUnit> cubesFromThePast = new List<CubeUnit>();
        string[] cubeIDs = cubeData.Split('.');
        for (int i = 0; i< cubeIDs.Length; i++)
        {
            CubeUnit newCube = currentMagicCube.allCubeUnits[int.Parse(cubeIDs[i])];
            detectedCubes.Add(newCube);
            newCube.transform.SetParent(rotator, true);
        }
        
        Rotate(GetReverseDirection(parsed_enum));


        recordedSteps.RemoveAt(recordedSteps.Count - 1);
    }

    void OnSwipeRotate(Globals.CubeRotationDirection direction, CubePlane plane) {


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



    public void Rotate(Globals.CubeRotationDirection direction)
    {
        if (direction == Globals.CubeRotationDirection.left)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        else if (direction == Globals.CubeRotationDirection.right)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0)));

        else if (direction == Globals.CubeRotationDirection.upLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
        else if (direction == Globals.CubeRotationDirection.downRight)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0)));

        else if (direction == Globals.CubeRotationDirection.downLeft)
            StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
        else if (direction == Globals.CubeRotationDirection.upRight)
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
            detectedCubes[i].transform.SetParent(currentMagicCube.transform, true);
            //yield return null;
        }


        //Reset Rotator Rotation to zero
        rotator.rotation = Quaternion.Euler(0,0,0);
        isTopFaceSelected = false;
        ToggleAllPlanes(false);
        ClearAllPlanesData();
        selectedCubeUnit = null;

        detectedCubes.Clear();
        Debug.LogError("Finish");
        rotating = false;
    }

    #endregion

    #region Callbacks
    void OnSwipe(Globals.SwipeDirection direction,bool isActualSwipe) {
        //swipeDirection = direction;
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
        switch (direction)
        {
            case Globals.SwipeDirection.up:
                if(isCubePlacedInLeftScreen())
                    OnSwipeRotate(Globals.CubeRotationDirection.upRight, verticalPlaneLeft);
                else
                    OnSwipeRotate(Globals.CubeRotationDirection.upLeft, verticalPlaneRight);
                break;

            case Globals.SwipeDirection.down:
                if (isCubePlacedInLeftScreen())
                    OnSwipeRotate(Globals.CubeRotationDirection.downLeft, verticalPlaneLeft);
                else
                    OnSwipeRotate(Globals.CubeRotationDirection.downRight, verticalPlaneRight);
                break;
            case Globals.SwipeDirection.left:
                if (isTopFaceSelected)
                    OnSwipeRotate(Globals.CubeRotationDirection.downLeft, verticalPlaneLeft);
                else
                    OnSwipeRotate(Globals.CubeRotationDirection.left,horizontalPlane);
                break;
            case Globals.SwipeDirection.right:
                if (isTopFaceSelected)
                    OnSwipeRotate(Globals.CubeRotationDirection.upRight, verticalPlaneLeft);
                else
                    OnSwipeRotate(Globals.CubeRotationDirection.right, horizontalPlane);
                break;
            case Globals.SwipeDirection.upLeft:
                OnSwipeRotate(Globals.CubeRotationDirection.upLeft, verticalPlaneRight);
                break;
            case Globals.SwipeDirection.downRight:
                OnSwipeRotate(Globals.CubeRotationDirection.downRight, verticalPlaneRight);
                break;
            case Globals.SwipeDirection.upRight:
                OnSwipeRotate(Globals.CubeRotationDirection.upRight, verticalPlaneLeft);
                break;
            case Globals.SwipeDirection.downLeft:
                OnSwipeRotate(Globals.CubeRotationDirection.downLeft, verticalPlaneLeft);
                break;
        }

        //latestSwipeDirection = swipeDirection;
    }




    void RecordCubeRotation(List<CubeUnit> cubes, Globals.CubeRotationDirection direction) {
        string value = "";
        //Record Cube IDs
        for (int i = 0; i < cubes.Count; i++) {
            value += cubes[i].uniqueID;
            if(i<cubes.Count-1)
               value += ".";
        }

        if (string.IsNullOrEmpty(value))
            return;

        //Record CubeRotationDirection
        value += "_" + System.Enum.GetName(typeof(Globals.CubeRotationDirection), direction);


        Debug.LogError("Record Entry = " + value);

        recordedSteps.Add(value);
    }

    public void ToggleAllPlanes(bool flag)
    {
        horizontalPlane.gameObject.SetActive(flag);
        verticalPlaneLeft.gameObject.SetActive(flag);
        verticalPlaneRight.gameObject.SetActive(flag);
    }

    public void ClearAllPlanesData()
    {
        horizontalPlane.Clear();
        verticalPlaneLeft.Clear();
        verticalPlaneRight.Clear();
    }


    public void OnFinish() {
        currentMagicCube.gameObject.SetActive(false);
        currentMagicCube = null;

        //UnSubscribe to Event Brodcasts
        Globals.OnSwipe -= OnSwipe;
        Globals.OnPointerDown -= OnTryGrabCubeUnit;
        Globals.OnPointerUp -= OnPointerUp;
    }

    void OnTryGrabCubeUnit() {
        StartCoroutine(GrabSelectedCubeUnit());
    }

    void OnPointerUp() {
        ToggleAllPlanes(false);
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


    Globals.CubeRotationDirection GetReverseDirection(Globals.CubeRotationDirection direction)
    {
        switch (direction)
        {
            case Globals.CubeRotationDirection.left:
                return Globals.CubeRotationDirection.right;

            case Globals.CubeRotationDirection.right:
                return Globals.CubeRotationDirection.left;

            case Globals.CubeRotationDirection.upLeft:
                return Globals.CubeRotationDirection.downRight;

            case Globals.CubeRotationDirection.downRight:
                return Globals.CubeRotationDirection.upLeft;

            case Globals.CubeRotationDirection.upRight:
                return Globals.CubeRotationDirection.downLeft;

            case Globals.CubeRotationDirection.downLeft:
                return Globals.CubeRotationDirection.upRight;

            default:
                Debug.LogError("Unable to get Reverse Direction");
                return Globals.CubeRotationDirection.left;
        }
    }
    #endregion


}
