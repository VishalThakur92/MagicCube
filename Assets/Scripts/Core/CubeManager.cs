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

    //The Currently selected Magic Cube's Render Camera
    public Camera currentMagicCubeCamera;

    //Selected Magic Cube's prefab is instantiated as child of this obj
    [SerializeField]
    Transform magicCubeParent, magicCubeCameraParent;

    [SerializeField]
    List<MagicCube> allMagicCubes = new List<MagicCube>();

    //The Steps being Recorded with every Cube move
    public List<string> rotationSteps = new List<string>();


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

        currentMagicCube = Instantiate(allMagicCubes[(int)cubeType] , magicCubeParent);

        currentMagicCubeCamera = Instantiate(allMagicCubes[(int)cubeType].respectiveCamera, magicCubeCameraParent);

        //currentMagicCube = newMagicCube;

        //Apply rotations if alrady there, this is used when we Load a saved game
        if (rotationSteps.Count > 0)
        {
            ApplyAllRotationSteps();
        }
        else//Fresh new Game
        {
            SubsribeToInputEvents();
        }
    }

    void SubsribeToInputEvents() {
        //Subscribe to Event Brodcasts
        Globals.OnSwipe += OnSwipe;
        Globals.OnPointerDown += OnTryGrabCubeUnit;
        Globals.OnPointerUp += OnPointerUp;
    }


    void ApplyAllRotationSteps() {

        rotating = true;

        //Wait for All Steps to be applied
        for (int j = 0; j < rotationSteps.Count; j++)
        {
            //Grab Cube ID
            string directionData = rotationSteps[j].Split('_')[1];
            //Get Rotation Direction
            Globals.CubeRotationDirection parsed_enum = (Globals.CubeRotationDirection)System.Enum.Parse(typeof(Globals.CubeRotationDirection), directionData);

            string cubeData = rotationSteps[j].Split('_')[0];
            string[] cubeIDs = cubeData.Split('.');
            for (int i = 0; i < cubeIDs.Length; i++)
            {
                CubeUnit newCube = currentMagicCube.allCubeUnits[int.Parse(cubeIDs[i])];
                detectedCubes.Add(newCube);
                newCube.transform.SetParent(rotator, true);
            }

            Rotate(parsed_enum,false);
        }

        //Clear all rotation Steps since all of them have been applied
        //rotationSteps.Clear();

        //Once all step rotations have been applied
        SubsribeToInputEvents();
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

        foreach (string value in rotationSteps)
        {
            GUILayout.Label($"ID : {value.Split('_')[0]} dir = {value.Split('_')[1]}", guiStyle);
        }
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
                ClearAllPlanesData();
                yield return null;
                ToggleAllPlanes(true);
                mousePositionXOnInput = Input.mousePosition.x;


                DebugRaydirection = hit.normal;

                LayerMask ignoreLayers = 1 << 3;

                yield return new WaitForEndOfFrame();

                //Check if the selected Cube is in the TOP face of the MAGIC Cube
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
        if (rotating || rotationSteps.Count == 0)
            return;


        rotating = true;
        //Grab Cube ID
        string directionData = rotationSteps[rotationSteps.Count - 1].Split('_')[1];


        //Get Rotation Direction
        Globals.CubeRotationDirection parsed_enum = (Globals.CubeRotationDirection)System.Enum.Parse(typeof(Globals.CubeRotationDirection), directionData);

        string cubeData = rotationSteps[rotationSteps.Count - 1].Split('_')[0];
        //List<CubeUnit> cubesFromThePast = new List<CubeUnit>();
        string[] cubeIDs = cubeData.Split('.');
        for (int i = 0; i< cubeIDs.Length; i++)
        {
            CubeUnit newCube = currentMagicCube.allCubeUnits[int.Parse(cubeIDs[i])];
            detectedCubes.Add(newCube);
            newCube.transform.SetParent(rotator, true);
        }
        
        Rotate(GetReverseDirection(parsed_enum),true);


        rotationSteps.RemoveAt(rotationSteps.Count - 1);
    }


    //Does a specified step
    //public void Redo() {

    //    rotating = true;
    //    //Grab Cube ID
    //    string directionData = rotationSteps[rotationSteps.Count - 1].Split('_')[1];




    //    //Get Rotation Direction
    //    Globals.CubeRotationDirection parsed_enum = (Globals.CubeRotationDirection)System.Enum.Parse(typeof(Globals.CubeRotationDirection), directionData);

    //    string cubeData = rotationSteps[rotationSteps.Count - 1].Split('_')[0];
    //    //List<CubeUnit> cubesFromThePast = new List<CubeUnit>();
    //    string[] cubeIDs = cubeData.Split('.');
    //    for (int i = 0; i < cubeIDs.Length; i++)
    //    {
    //        CubeUnit newCube = currentMagicCube.allCubeUnits[int.Parse(cubeIDs[i])];
    //        detectedCubes.Add(newCube);
    //        newCube.transform.SetParent(rotator, true);
    //    }

    //    Rotate(GetReverseDirection(parsed_enum));


    //    rotationSteps.RemoveAt(rotationSteps.Count - 1);
    //}


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
        Rotate(direction,true);
    }



    public void Rotate(Globals.CubeRotationDirection direction, bool async)
    {
        Quaternion targetRotation = new Quaternion();

        //Calculate as per direction
        if (direction == Globals.CubeRotationDirection.left)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
        }
        else if (direction == Globals.CubeRotationDirection.right)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0)));
        }

        else if (direction == Globals.CubeRotationDirection.upLeft)
        {
            targetRotation = transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
        }
        else if (direction == Globals.CubeRotationDirection.downRight)
        {
            targetRotation = transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0)));
        }
        else if (direction == Globals.CubeRotationDirection.downLeft)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
        }
        else if (direction == Globals.CubeRotationDirection.upRight)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier);
            //StartCoroutine(RotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier)));
        }


        //Rotate in Async manner
        if (async) {
            StartCoroutine(RotationBehaviourAsync(targetRotation));
        }
        //Rotate in sync manner
        else{
            RotationBehaviour(targetRotation);
        }
    }


    void RotationBehaviour(Quaternion targetRotation) {

        rotator.rotation = targetRotation;

        for (int i = 0; i < detectedCubes.Count; i++)
        {
            detectedCubes[i].transform.SetParent(currentMagicCube.transform, true);
        }


        //Reset Rotator Rotation to zero
        rotator.rotation = Quaternion.Euler(0, 0, 0);
        isTopFaceSelected = false;
        ToggleAllPlanes(false);
        ClearAllPlanesData();
        selectedCubeUnit = null;

        detectedCubes.Clear();
        Debug.LogError("Finish");
        rotating = false;
    }

    IEnumerator RotationBehaviourAsync(Quaternion targetRotation)
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

        rotationSteps.Add(value);
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


    public void OnFinish()
    {
        Destroy(currentMagicCube.gameObject);
        Destroy(currentMagicCubeCamera.gameObject);

        rotationSteps.Clear();


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
