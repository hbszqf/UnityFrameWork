using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using Consolation;


public class Main : MonoBehaviour {


    [Header("是否显示GUI Console 调试信息窗")]
    public bool showConsole = false;

    
    [Header("是否显示FPS")]
    public bool showFps = false;

    // Use this for initialization
    IEnumerator Start () {
        Debug.Log("Start");

        //LuaState lua = new LuaState();
        //lua.Start();
        //lua.AddSearchPath("D:/learn/UnityFrameWork/Assets/Lua_Game");

        //lua.DoFile("Main.lua");


        Console console = gameObject.GetComponent<Console>();
        if (console != null) { console.enabled = showConsole; }
        else if (showConsole)
        {
            console = gameObject.AddComponent<Console>();
            console.shakeToOpen = false;
        }

        ShowFPS showFPS = gameObject.GetComponent<ShowFPS>();
        if (showFPS != null) { showFPS.enabled = showFps; }
        else if (showFps)
        {
            showFPS = gameObject.AddComponent<ShowFPS>();
        }

        WWW www = new WWW(string.Format("{0}/{1}", Application.streamingAssetsPath, "version.json"));
        yield return www;
        if (www.error != null)
        {

            yield break;
        }

        yield break;

    }

     void Awake()
    {
        Debug.Log("Awake");

    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Update");
    }


}


/// window lua path
