using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Parameters
    //Singelton Instance
    public static GameManager Instance { get; private set; }
    //Currently Selected Magic Cube's type
    [SerializeField]
    Globals.CubeType currentMagicCubeType;

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
        currentMagicCubeType = type;
        CubeManager.Instance.Initialize(currentMagicCubeType);

        UIManager.Instance.SetCanvasCamera(CubeManager.Instance.currentMagicCube.respectiveCamera);
    }

    public void StartGame(int cubeType) {
        StartGame((Globals.CubeType)cubeType);
    }
    #endregion

}
