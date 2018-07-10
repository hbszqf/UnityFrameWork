using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;


public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Start");

        LuaState lua = new LuaState();
        lua.Start();
        lua.AddSearchPath("D:/learn/UnityFrameWork/Assets/Lua_Game");
     
        lua.DoFile("Main.lua");

    }

    private void Awake()
    {
        Debug.Log("Awake");

    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Update");
    }


}


/// window lua path
