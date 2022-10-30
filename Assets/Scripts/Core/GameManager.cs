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

    void Start()
    {
        StartGame();
    }


    public void StartGame() {
        CubeManager.Instance.Initialize(currentMagicCubeType);

        UIManager.Instance.SetCanvasCamera(CubeManager.Instance.currentMagicCube.respectiveCamera);
    }
    #endregion

}
