using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
namespace FW
{

    [System.Serializable]
    public class PlayableControllerItemAsset 
    {
        public string name;
        public string path;
    }

    [Serializable]
    public class PlayableControllerAsset : ScriptableObject
    {
        [UnityEngine.SerializeField]
        public List<PlayableControllerItemAsset> list = new List<PlayableControllerItemAsset>();
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            //EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
            ScriptableObject bullet = ScriptableObject.CreateInstance<PlayableControllerAsset>();
            AssetDatabase.CreateAsset(bullet, "Assets/Test.asset");
        }
    }

}