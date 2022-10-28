
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    [SerializeField]
    List<GameObject> list = new List<GameObject>();

    [ContextMenu("Do")]
    public void Do() {
        for (int i = 0; i < list.Count; i++) {
            list[i].name = "Cube " + (i + 1);
        }
    }
}
#endif