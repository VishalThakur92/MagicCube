using UnityEngine;
using System.IO;

public class RuntimeText : MonoBehaviour
{

    public static void WriteString(TextAsset textAsset, string newData)

    {
        //File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), newData);
        //EditorUtility.SetDirty(TEXT_ASSET);

    }

    public static void ReadString(TextAsset textAsset)
    {
        Debug.LogError("savedData : \n" + textAsset.text);
        //string path = Application.persistentDataPath + "/test.txt";

        ////Read the text from directly from the test.txt file
        //TextAsset r;
        //StreamReader reader = new StreamReader();

        //Debug.Log(reader.ReadToEnd());

        //reader.Close();

    }

}

