using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Parameters
    //Singelton Instance
    public static GameManager Instance { get; private set; }

    //[SerializeField]
    //TextAsset savedData;
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


     void Start()
    {
        if (!DoesSaveGameExist())
        {
            PlayerPrefs.DeleteAll();
            UIManager.Instance.ToggleLoadGameButton(false);
        }
        else{

            UIManager.Instance.ToggleLoadGameButton(true);
        }
    }

    //Starts the Game with the Specified Parameters
    void StartGame(Globals.CubeType type)
    {
        //set the specified Cube type
        CubeManager.Instance.Initialize(type);

        //Set Canvas Camera as per the selected respective cube
        UIManager.Instance.SetCanvasCamera(CubeManager.Instance.currentMagicCube.respectiveCamera);

    }


    //Start the game with the Specified Cube type
    public void StartGame(int cubeType) {
        StartGame((Globals.CubeType)cubeType);
    }

    //Exit current Game and goto main menu
    public void ExitGame()
    {
        SaveGame();

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
    public void SaveGame() {
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
        for (int i = 0; i < data.Count; i++) {
            PlayerPrefs.SetString(i.ToString(), data[i]);
        }
    }


    //Loads the Last Saved Game if Any
    public void LoadSavedGame() {

        for (int i = 0; i < CubeManager.Instance.rotationSteps.Count; i++) {
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

        for (int i = 0; i < dataCount; i++) {
            string newValue = PlayerPrefs.GetString(i.ToString());
            CubeManager.Instance.rotationSteps.Add(newValue);
        }

        StartGame((Globals.CubeType)cubeType);

        //Start A new Game and apply rotations to the respective cube
    }

    bool DoesSaveGameExist() {

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
    #endregion

}
