using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
//using Consolation;


public class Main : MonoBehaviour {
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        new FW.Mgr();
        FW.Mgr.inst.AddMgr<FW.ResMgr>();
        FW.Mgr.inst.AddMgr<FW.SceneMgr>();
        FW.Mgr.inst.AddMgr<FW.LuaMgr>().StartUp();
    }

    void Update()
    {


        
    }



}


/// window lua path
