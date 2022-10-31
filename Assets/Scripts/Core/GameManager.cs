using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Parameters
    //Singelton Instance
    public static GameManager Instance { get; private set; }

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
    public void ExitGame() {
        //OnFinish behaviour for UIManager
        UIManager.Instance.OnFinish();

        //OnFinish behaviour for CubeManager
        CubeManager.Instance.OnFinish();
    }

    //Save game
    public void SaveGame() {

    }
    #endregion

}
