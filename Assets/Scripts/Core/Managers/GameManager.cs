using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicCubeVishal
{
    public class GameManager : MonoBehaviour
    {
        #region Parameters
        //Singelton Instance
        public static GameManager Instance { get; private set; }

        //Seconds taken to solve a Magic Cube in a respective session
        float secondsTaken = 0;
        string gameTimeFormatted;
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


        void Start()
        {
            //PlayerPrefs.DeleteAll();
            if (!DoesSaveGameExist())
            {
                PlayerPrefs.DeleteAll();
                UIManager.Instance.ToggleLoadGameButton(false);
            }
            else
            {

                UIManager.Instance.ToggleLoadGameButton(true);
            }

            SubscribeEvents();
        }


        private void OnDestroy()
        {
            UnSubscribeEvents();
        }
        #endregion

        #region Core
        void SubscribeEvents()
        {
            Globals.OnCubeSolved += OnCubeSolved;
            Globals.OnCubeShuffleComplete += OnCubeShuffleComplete;
        }
        void UnSubscribeEvents()
        {
            Globals.OnCubeSolved -= OnCubeSolved;
            Globals.OnCubeShuffleComplete -= OnCubeShuffleComplete;
        }


        //Starts the Game with the Specified Parameters
        void StartGame(Globals.CubeType type)
        {
            //set the specified Cube type
            CubeManager.Instance.Initialize(type);

            //Set Canvas Camera as per the selected respective cube
            UIManager.Instance.SetCanvasCamera(CubeManager.Instance.currentMagicCubeCamera);

        }


        //Start the game with the Specified Cube type
        public void StartGame(int cubeType)
        {
            StartGame((Globals.CubeType)cubeType);
        }

        //Exit current Game and goto main menu
        public void ExitGame()
        {
            //SaveGame();

            //OnFinish behaviour for UIManager
            UIManager.Instance.OnFinish();

            //OnFinish behaviour for CubeManager
            CubeManager.Instance.OnFinish();

            if (!DoesSaveGameExist())
            {
                PlayerPrefs.DeleteAll();
                UIManager.Instance.ToggleLoadGameButton(false);
            }
            else
            {

                UIManager.Instance.ToggleLoadGameButton(true);
            }

        }

        //Save game
        public void SaveGame()
        {
            //Clear old Data if any
            PlayerPrefs.DeleteAll();

            //Grab rotationSteps from CubeManager
            List<string> data = new List<string>();
            data.AddRange(CubeManager.Instance.rotationSteps);

            //Save total number of Data Count
            PlayerPrefs.SetInt("dataCount", data.Count);

            //Save total number of Data Count
            PlayerPrefs.SetInt("cubeType", (int)CubeManager.Instance.currentMagicCube.cubeType);

            //Save Data
            for (int i = 0; i < data.Count; i++)
            {
                PlayerPrefs.SetString(i.ToString(), data[i]);
            }
        }


        //Loads the Last Saved Game if Any
        public void LoadSavedGame()
        {

            for (int i = 0; i < CubeManager.Instance.rotationSteps.Count; i++)
            {
                CubeManager.Instance.Undo();
            }

            int cubeType = PlayerPrefs.GetInt("cubeType");
            int dataCount = PlayerPrefs.GetInt("dataCount");

            if (dataCount == 0)
            {
                Debug.LogError("No steps Found");
                PlayerPrefs.DeleteAll();
                StartGame(Globals.CubeType.size3);
                return;
            }

            List<string> rotationSteps = new List<string>();

            CubeManager.Instance.rotationSteps.Clear();

            for (int i = 0; i < dataCount; i++)
            {
                string newValue = PlayerPrefs.GetString(i.ToString());
                CubeManager.Instance.rotationSteps.Add(newValue);
            }

            StartGame((Globals.CubeType)cubeType);

            //Start A new Game and apply rotations to the respective cube
        }


        public void JumpToDevProfile() {
            Application.OpenURL("https://www.linkedin.com/in/vishal-thakur-56304266/");
        }
        bool DoesSaveGameExist()
        {

            if (PlayerPrefs.HasKey("dataCount"))
            {
                int dataCount = PlayerPrefs.GetInt("dataCount");
                if (dataCount == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void AcknowledgeOnCubeSolved() {
            //Reset Cube Manager
            CubeManager.Instance.Reset();

            //Reset Timer Text
            UIManager.Instance.gameTimerText.text = "00:00";
        }


        public void StartGameTimer()
        {
            StartCoroutine(GameTimerBehaviour());
        }

        public void StopGameTimer()
        {
            StopAllCoroutines();
            //StopCoroutine(GameTimerBehaviour());
        }

        IEnumerator GameTimerBehaviour() {
            secondsTaken = 0;
            string seconds = "--";
            string minutes = "--";


            while (true)
            {
                //Debug.LogError($"{secondsTaken}");
                secondsTaken += Time.deltaTime;
                minutes = Mathf.Floor(secondsTaken / 60).ToString("00");
                seconds = (secondsTaken % 60).ToString("00");
                gameTimeFormatted = string.Format("{0}:{1}", minutes, seconds);
                //Show Time in the Text Component
                UIManager.Instance.gameTimerText.text = gameTimeFormatted;

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion


        #region Callbacks

        //Decide what Happens when a cube is solved
        void OnCubeSolved()
        {
            UIManager.Instance.HUDMenu.gameObject.SetActive(false);
            UIManager.Instance.gameCompleteMenu.gameObject.SetActive(true);

            //Cube solved Stop the Game timer
            StopGameTimer();




            //Set Game Complete Message
            UIManager.Instance.gameCompleteMessageText.text = Globals.gameOverMessage + gameTimeFormatted;

            //On Win Behaviour - Keep the Cube Spinning Animation
            CubeManager.Instance.currentMagicCube.RotateCrazy();
        }

        //Decide what happens when magic Cube's Shuffling has been completed
        void OnCubeShuffleComplete() {
            //Start Game Timer
            StartGameTimer();
        }
        #endregion

    }
}