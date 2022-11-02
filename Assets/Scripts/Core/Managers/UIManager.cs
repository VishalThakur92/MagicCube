using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MagicCubeVishal
{
    public class UIManager : MonoBehaviour
    {

        #region Parameters
        //Singelton Instance
        public static UIManager Instance { get; private set; }

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        Button loadSavedGameButton;


        //UI Menus
        [SerializeField]
        public GameObject gameCompleteMenu, HUDMenu;
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
        public void SetCanvasCamera(Camera camera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            camera.gameObject.SetActive(true);
        }

        public void OnFinish()
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera.gameObject.SetActive(false);
        }

        public void ToggleLoadGameButton(bool flag)
        {
            loadSavedGameButton.gameObject.SetActive(flag);
        }
        #endregion
    }
}
