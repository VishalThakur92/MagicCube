using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCubeVishal
{
    public class CubeManager : MonoBehaviour{
        #region Parameters
        //Singelton Instance
        public static CubeManager Instance { get; private set; }


        //Selected Magic Cube's prefab is instantiated as child of this obj
        [SerializeField]
        Transform magicCubeParent, magicCubeCameraParent;

        [SerializeField]
        List<MagicCube> allMagicCubes = new List<MagicCube>();



        //[SerializeField]
        LayerMask cubeUnitRayLayerMask = 1 << 6;//Raycast on Cube Unit only


        LayerMask ignoreLayersTopFaceDetection = 1 << 3;//Raycast Top Face Detector Plane only

        [Space(10)]
        [SerializeField]
        //Cube Units detected via Detector planes are placed inside rotator, and Rotator is rotated by a specific degrees in the specified Input Direction
        Transform rotator;

        //Reference of the Cubes detected by detector plane, these will be placed inside rotator to be rotated in the specified Input Direction
        List<CubeUnit> detectedCubes = new List<CubeUnit>();

        //Is the Magic Cube rotating via a given Input Direction
        [SerializeField]
        bool rotating = false;


        //Time it takes make on Magic cube's Side rotation
        [SerializeField]
        float lerpDuration = 5f, magicCubeRotationLerpDuration = 1;

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
        [SerializeField]
        Transform detectorPlanesParent;
        [SerializeField]
        public CubeUnitDetectorPlane planeY, planeZ, planeX;

        [Header("Debug Only, Donot Edit")]
        [Space(10)]
        [SerializeField]
        //Selected Magic Cube's Individual Cube Unit
        CubeUnit selectedCubeUnit;
        //The Steps being Recorded with every Cube move

        [SerializeField]
        public List<string> rotationSteps = new List<string>();
        //The Currently selected Magic Cube
        public MagicCube currentMagicCube;

        //The Currently selected Magic Cube's Render Camera
        public Camera currentMagicCubeCamera;
        #endregion


        #region Testing

        private GUIStyle guiStyle = new GUIStyle();
        //Globals.SwipeDirection latestSwipeDirection;
        private void OnGUI()
        {
            //GUILayout.Label($"Mouse Position x = {Input.mousePosition.x} y = {Input.mousePosition.y}", guiStyle);
            //GUILayout.Label($"Screen Width = {Screen.width / 2}", guiStyle);
            //GUILayout.Label($"Swipe Dir = {swipeDirection}", guiStyle);

            //foreach (string value in rotationSteps)
            //{
            //    GUILayout.Label($"ID : {value.Split('_')[0]} dir = {value.Split('_')[1]}", guiStyle);
            //}

                //GUILayout.Label($"rotating = {rotating}", guiStyle);
        }



        Vector3 DebugRaydirection;


        //Controls for Editor testing
        public void Update()
        {
            if (selectedCubeUnit)
                Debug.DrawLine(selectedCubeUnit.transform.position, DebugRaydirection, Color.green);

            if (rotating)
                return;


            //if (Input.GetKeyUp(KeyCode.Space))
            //    Undo();



            if (Input.GetKeyUp(KeyCode.D))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.right, true);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.left, true);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.up, true);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.down, true);
            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.upLeft, true);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.upRight, true);
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.downLeft, true);
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                Globals.OnSwipe?.Invoke(Globals.SwipeDirection.downRight, true);
            }
            //else if (Input.GetKeyUp(KeyCode.P))
            //{
            //    Shuffle();
            //}

        }
        #endregion

        #region Unity

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
            screenhalf = Screen.width / 2;
        }

        #endregion


        #region Core
        //Initialize with a specified Cube Type
        public void Initialize(Globals.CubeType cubeType)
        {
            UnSubsribeFromEvents();
            //Load and instantitate specified Cube prefab
            currentMagicCube = Instantiate(allMagicCubes[(int)cubeType], magicCubeParent);

            //Load and instantitate specified Cube's Camera
            currentMagicCubeCamera = Instantiate(allMagicCubes[(int)cubeType].respectiveCamera, magicCubeCameraParent);

            //Apply rotations if alrady there, this is used when we Load a saved game
            if (rotationSteps.Count > 0)
            {
                ApplyAllRotationSteps();
            }
            else//Fresh new Game
            {
                Shuffle();
            }
        }

        void SubsribeToEvents()
        {
            //Subscribe to Event Brodcasts
            Globals.OnSwipe += OnSwipe;
            Globals.OnPointerDown += OnTryGrabCubeUnit;
            Globals.OnPointerUp += OnPointerUp;
        }

        void UnSubsribeFromEvents()
        {
            //Subscribe to Event Brodcasts
            Globals.OnSwipe -= OnSwipe;
            Globals.OnPointerDown -= OnTryGrabCubeUnit;
            Globals.OnPointerUp -= OnPointerUp;
        }

        void Shuffle() {
            StartCoroutine(ShuffleBehaviour());
        }


        //Shuffle the Current Magic Cube Randomly
        IEnumerator ShuffleBehaviour() {
            //Unsubscribe from Input Events as we donot want to take user input while SHUFFLE in progress
            UnSubsribeFromEvents();

            //Wait for x seconds before shuffling
            yield return new WaitForSeconds(Globals.secondsToWaitBeforeShuffle);

            //total number of shuffle steps - RANOM
            int shuffleSteps = Random.Range(Globals.MinimumShuffleSteps , Globals.MaximumShuffleSteps);
            //Debug.LogError($"Will Rotate for {shuffleSteps} steps");
            //CubeUnit randomCubeUnit;
            Globals.SwipeDirection randomSwipe;
            //Shuffle the magic Cube x times where, x = shuffleSteps
            for (int i = 0; i < shuffleSteps; i++) {
                //rotating = true;
                //Select a random cube unit
                selectedCubeUnit = currentMagicCube.allCubeUnits[Random.Range(0, currentMagicCube.allCubeUnits.Count)];

                //Select a random rotation
                randomSwipe = (Globals.SwipeDirection)Random.Range(0, 8);

                //Debug.LogError($"Applying random rotation: {randomSwipe} on {selectedCubeUnit.name}");

                detectedCubes.Clear();
                detectorPlanesParent.position = selectedCubeUnit.transform.position;
                ClearAllPlanesData();
                ToggleAllPlanes(true);

                //Wait for x seconds as to Simulate a click
                yield return new WaitForSeconds(.1f);

                OnPointerUp();

                //Apply random rotation to the Random Cube units
                OnSwipe(randomSwipe, false);

                //Wait untill this animation

                //yield return new WaitForSeconds(2);
                yield return new WaitUntil(() => !rotating);
            }

            //Clear Rotation Steps as we dont want the user to keep pressing UNDO and setting the Cube to Solved state
            rotationSteps.Clear();


            //Shuffle Complete, Now we want the user to be able to give inputs to rotate the Cube
            SubsribeToEvents();

            //Enable Cube Solved Detectors

        }


        //Applies All Rotation steps in the rotationSteps List
        void ApplyAllRotationSteps()
        {
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

                Rotate(parsed_enum, false);
            }

            //Clear all rotation Steps since all of them have been applied
            //rotationSteps.Clear();

            //Once all step rotations have been applied
            SubsribeToEvents();
        }


        IEnumerator GrabSelectedCubeUnit()
        {
            //Cache mouse position X while a potential cube unit is selected
            mousePositionXOnInput = Input.mousePosition.x;
            Ray ray = currentMagicCube.respectiveCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x , Input.mousePosition.y , currentMagicCube.respectiveCamera.nearClipPlane));

            
            if (Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, cubeUnitRayLayerMask, QueryTriggerInteraction.UseGlobal))
            {
                //Debug.LogError(hit.transform.name);
                selectedCubeUnit = hit.transform.GetComponent<CubeUnit>();

                if (selectedCubeUnit != null)
                {

                    detectedCubes.Clear();
                    //Debug.LogError(hit.transform.name);
                    detectorPlanesParent.position = selectedCubeUnit.transform.position;
                    ClearAllPlanesData();
                    yield return null;
                    ToggleAllPlanes(true);


                    DebugRaydirection = hit.normal;

                    yield return new WaitForEndOfFrame();

                    //Check if the selected Cube is in the TOP face of the MAGIC Cube
                    if (selectedCubeUnit && Physics.Raycast(selectedCubeUnit.transform.position, hit.normal, out RaycastHit hit2, ~ignoreLayersTopFaceDetection))
                    {
                        //Debug.LogError($"name = {hit2.transform.name} layer = {hit2.transform.gameObject.layer}");
                        if (hit2.transform.gameObject.layer == 3)
                        {
                            isTopFaceSelected = true;
                            //Debug.LogError("Top Face Seleced " + hit2.transform.name);
                        }
                    }
                    else
                    {
                        isTopFaceSelected = false;
                        //Debug.LogError("Nothing");
                    }
                }

            }
            else
            {
                //No Cube unit was grabbed
                selectedCubeUnit = null;
            }
        }

        public void Undo()
        {
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
            for (int i = 0; i < cubeIDs.Length; i++)
            {
                CubeUnit newCube = currentMagicCube.allCubeUnits[int.Parse(cubeIDs[i])];
                detectedCubes.Add(newCube);
                newCube.transform.SetParent(rotator, true);
            }

            Rotate(GetReverseDirection(parsed_enum), true);


            rotationSteps.RemoveAt(rotationSteps.Count - 1);
        }



        void OnSwipeRotate(Globals.CubeRotationDirection direction, CubeUnitDetectorPlane plane)
        {
            RecordCubeRotation(plane.detectedCubeUnits, direction);

            //Grab all Horizontal Cube units
            detectedCubes.AddRange(plane.detectedCubeUnits);

            //Set Rotator as their parent
            for (int i = 0; i < detectedCubes.Count; i++)
            {
                detectedCubes[i].transform.SetParent(rotator, true);
            }

            //Rotate the Rotator
            Rotate(direction, true);
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
            if (async)
            {
                StartCoroutine(RotationBehaviourAsync(targetRotation));
            }
            //Rotate in sync manner
            else
            {
                RotationBehaviour(targetRotation);
            }
        }


        void RotationBehaviour(Quaternion targetRotation)
        {

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
            Quaternion startRotation = transform.localRotation;

            while (timeElapsed < lerpDuration)
            {
                rotator.localRotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            rotator.rotation = targetRotation;
            yield return new WaitForEndOfFrame();

            //Put Detected Cubes back into Rubik Cube from Rotator
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
            //Debug.LogError("Finish");

            if (checkIfSolved)
            {
                //Check if magic Cube is solved
                yield return currentMagicCube.IsSolvedBehaviour();

                if (currentMagicCube.isSolved)
                {
                    Debug.LogError($"{currentMagicCube} is solved!!! Game Complete Screen!!");
                }
                else//Continue Game, let user continue solve the Cube
                    rotating = false;
            }
            else
                rotating = false;
        }



        IEnumerator MagicCubeRotationBehaviourAsync(Quaternion targetRotation)
        {
            //put Current magic Cube inside Rotator
            currentMagicCube.transform.SetParent(rotator.transform, true);

            float timeElapsed = 0;
            Quaternion startRotation = transform.localRotation;


            while (timeElapsed < magicCubeRotationLerpDuration)
            {
                rotator.localRotation = Quaternion.Slerp(rotator.localRotation, targetRotation, timeElapsed / magicCubeRotationLerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            //Iron out any missed values
            rotator.rotation = targetRotation;
            yield return new WaitForEndOfFrame();

            //put Current magic Cube back to magicCubeParent
            currentMagicCube.transform.SetParent(magicCubeParent, true);

            //Iron out any missed values
            rotator.rotation = Quaternion.Euler(0, 0, 0);

            rotating = false;
        }


        void RecordCubeRotation(List<CubeUnit> cubes, Globals.CubeRotationDirection direction)
        {
            string value = "";
            //Record Cube IDs
            for (int i = 0; i < cubes.Count; i++)
            {
                value += cubes[i].uniqueID;
                if (i < cubes.Count - 1)
                    value += ".";
            }

            if (string.IsNullOrEmpty(value))
                return;

            //Record CubeRotationDirection
            value += "_" + System.Enum.GetName(typeof(Globals.CubeRotationDirection), direction);


            //Debug.LogError("Record Entry = " + value);

            rotationSteps.Add(value);
        }

        public void ToggleAllPlanes(bool flag)
        {
            planeY.gameObject.SetActive(flag);
            planeZ.gameObject.SetActive(flag);
            planeX.gameObject.SetActive(flag);
        }

        public void ClearAllPlanesData()
        {
            planeY.ClearDetectedCubeUnits();
            planeZ.ClearDetectedCubeUnits();
            planeX.ClearDetectedCubeUnits();
        }

        #endregion
        bool checkIfSolved = false;

        #region Callbacks
        void OnSwipe(Globals.SwipeDirection direction, bool isActualSwipe)
        {
            //if(isActualSwipe)
                //Debug.LogError($"OnSwipe {direction}");
            //swipeDirection = direction;
            //avoid multiple inputs
            if (rotating)
                return;

            rotating = true;

            if (selectedCubeUnit == null)
            {
                //Rotate Magic Cube as per Swipe
                switch (direction)
                {
                    case Globals.SwipeDirection.up:
                        //if mouse pos to left of screen rotate from DownLeft to Top Right
                        if (isInputPosXInLeftScreen())
                        {
                            StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier)));
                        }
                        //if mouse pos to right of screen rotate from down right to Top left
                        else
                        {
                            StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
                        }
                        break;


                    case Globals.SwipeDirection.left:
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, rotationMulitplier, 0)));
                        break;
                    case Globals.SwipeDirection.right:
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, -rotationMulitplier, 0)));
                        break;


                    case Globals.SwipeDirection.down:
                        //if mouse pos to left of screen rotate from Top Right to DownLeft
                        if (isInputPosXInLeftScreen())
                        {
                            StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
                        }
                        //if mouse pos to right of screen rotate from Top left to down right 
                        else
                        {
                            StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0)));
                        }
                        break;


                    case Globals.SwipeDirection.upLeft:
                        //Rotate Cube Down from DownRight to UpLeft
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(-rotationMulitplier, 0, 0)));
                        break;

                    case Globals.SwipeDirection.upRight:
                        //Rotate Cube Down from Downleft to UpRight
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, rotationMulitplier)));
                        break;

                    case Globals.SwipeDirection.downLeft:
                        //Rotate Cube Down from upRight to downLeft
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(0, 0, -rotationMulitplier)));
                        break;

                    case Globals.SwipeDirection.downRight:
                        //Rotate Cube Down from upleft to downRight
                        StartCoroutine(MagicCubeRotationBehaviourAsync(transform.rotation * Quaternion.Euler(rotationMulitplier, 0, 0)));
                        break;
                }
                //We just wanted to rotate the Magic Cube not it's Row/Column
                return;
            }


            //Rotate Magic Cube's Row/Coulumn as per Swipe
            switch (direction)
            {
                case Globals.SwipeDirection.up:
                    if (isInputPosXInLeftScreen())
                        OnSwipeRotate(Globals.CubeRotationDirection.upRight, planeZ);
                    else
                        OnSwipeRotate(Globals.CubeRotationDirection.upLeft, planeX);
                    break;

                case Globals.SwipeDirection.down:
                    if (isInputPosXInLeftScreen())
                        OnSwipeRotate(Globals.CubeRotationDirection.downLeft, planeZ);
                    else
                        OnSwipeRotate(Globals.CubeRotationDirection.downRight, planeX);
                    break;
                case Globals.SwipeDirection.left:
                    if (isTopFaceSelected)
                        OnSwipeRotate(Globals.CubeRotationDirection.downLeft, planeZ);
                    else
                        OnSwipeRotate(Globals.CubeRotationDirection.left, planeY);
                    break;
                case Globals.SwipeDirection.right:
                    if (isTopFaceSelected)
                        OnSwipeRotate(Globals.CubeRotationDirection.upRight, planeZ);
                    else
                        OnSwipeRotate(Globals.CubeRotationDirection.right, planeY);
                    break;
                case Globals.SwipeDirection.upLeft:
                    OnSwipeRotate(Globals.CubeRotationDirection.upLeft, planeX);
                    break;
                case Globals.SwipeDirection.downRight:
                    OnSwipeRotate(Globals.CubeRotationDirection.downRight, planeX);
                    break;
                case Globals.SwipeDirection.upRight:
                    OnSwipeRotate(Globals.CubeRotationDirection.upRight, planeZ);
                    break;
                case Globals.SwipeDirection.downLeft:
                    OnSwipeRotate(Globals.CubeRotationDirection.downLeft, planeZ);
                    break;
            }

            checkIfSolved = isActualSwipe;
        }






        private void OnDestroy()
        {
            UnSubsribeFromEvents();
        }

        public void OnFinish()
        {
            Destroy(currentMagicCube.gameObject);
            Destroy(currentMagicCubeCamera.gameObject);

            rotationSteps.Clear();

            UnSubsribeFromEvents();
        }

        void OnTryGrabCubeUnit()
        {
            StartCoroutine(GrabSelectedCubeUnit());
        }

        void OnPointerUp()
        {
            ToggleAllPlanes(false);
        }
        #endregion


        #region utility
        bool isInputPosXInLeftScreen()
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
}
