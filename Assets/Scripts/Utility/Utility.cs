
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MagicCubeVishal
{
    public class Utility : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        List<GameObject> list = new List<GameObject>();

        [ContextMenu("Do")]
        public void Do()
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].name = "Cube " + (i);
                list[i].GetComponent<CubeUnit>().uniqueID = i;
            }
        }

#endif
    }
}