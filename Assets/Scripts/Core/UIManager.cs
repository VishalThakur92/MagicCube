using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region Parameters
    //Singelton Instance
    public static UIManager Instance { get; private set; }

    [SerializeField]
    Canvas canvas;
    #endregion


    #region Methods
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    //Set the Canvas ScreenOverlay Camera
    public void SetCanvasCamera(Camera camera) {
        canvas.worldCamera = camera;
        camera.gameObject.SetActive(true);
    }
    #endregion
}
