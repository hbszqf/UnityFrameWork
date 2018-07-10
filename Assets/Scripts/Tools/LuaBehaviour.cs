using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;


public class LuaBehaviour : MonoBehaviour {
    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

    bool nowVisible = true;
    bool newVisible = true;
    

    public LuaFunction onStart = null;
	void Start () {
        if(onStart != null)
        {
            onStart.BeginPCall();
            onStart.PCall();
            onStart.EndPCall();
        }
	}

    public LuaFunction onDestroy = null;
    void OnDestroy()
    {
        if (onDestroy!=null) {
            onDestroy.BeginPCall();
            onDestroy.PCall();
            onDestroy.EndPCall();
            onDestroy.Dispose();
            onDestroy = null;
        }

        if (onStart != null) {
            onStart.Dispose();
            onStart = null;
        }
        if (onUpdate != null)
        {
            onUpdate.Dispose();
            onUpdate = null;
        }

        if (onLateUpdate!=null) {
            onLateUpdate.Dispose();
            onLateUpdate = null;
        }

        if(onApplicationFocus!=null)
        {
            onApplicationFocus.Dispose();
            onApplicationFocus = null;
        }
        if(onApplicationPause!=null)
        {
            onApplicationPause.Dispose();
            onApplicationPause = null;
        }
        if(onBecameInvisible!=null)
        {
            onBecameInvisible.Dispose();
            onBecameInvisible = null;
        }
        if(onBecameVisible!=null)
        {
            onBecameVisible.Dispose();
            onBecameVisible = null;
        }
        if(onCollisionEnter!=null)
        {
            onCollisionEnter.Dispose();
            onCollisionEnter = null;
        }
        if(onCollisionExit!=null)
        {
            onCollisionExit.Dispose();
            onCollisionExit = null;
        }
        if(onCollisionStay!=null)
        {
            onCollisionStay.Dispose();
            onCollisionStay = null;
        }
        if(onDisable!=null)
        {
            onDisable.Dispose();
            onDisable = null;
        }
        if(onEnable!=null)
        {
            onEnable.Dispose();
            onEnable = null;
        }
        if(onRenderImage!=null)
        {
            onRenderImage.Dispose();
            onRenderImage = null;
        }
        if(onTriggerEnter!=null)
        {
            onTriggerEnter.Dispose();
            onTriggerEnter = null;
        }
        if(onTriggerExit!=null)
        {
            onTriggerExit.Dispose();
            onTriggerExit = null;
        }
        if(onTriggerStay!=null)
        {
            onTriggerStay.Dispose();
            onTriggerStay = null;
        }
    }

    // Update is called once per frame
    public LuaFunction onUpdate = null;
    //void Update () {
    //    if (onUpdate != null)
    //    {
    //        onStart.BeginPCall();
    //        onStart.PCall();
    //        onStart.EndPCall();
    //    }
    //}

    // OnLateUpdate is called once per frame
    public LuaFunction onLateUpdate = null;
    //private void LateUpdate()
    //{
    //    if (onLateUpdate!=null)
    //    {
    //        onLateUpdate.BeginPCall();
    //        onLateUpdate.PCall();
    //        onLateUpdate.EndPCall();

    //    }
    //}

    //OnApplicationFocus
    public LuaFunction onApplicationFocus = null;
    private void OnApplicationFocus(bool hasFocus) {
        if (onApplicationFocus != null)
        {
            onApplicationFocus.BeginPCall();
            onApplicationFocus.Push(hasFocus);
            onApplicationFocus.PCall();
            onApplicationFocus.EndPCall();

        }
    }

    public LuaFunction onApplicationPause = null;
    void OnApplicationPause(bool pauseStatus)
    {
        if (onApplicationPause != null)
        {
            onApplicationPause.BeginPCall();
            onApplicationPause.Push(pauseStatus);
            onApplicationPause.PCall();
            onApplicationPause.EndPCall();

        }
    }

    public LuaFunction onBecameInvisible = null;
    void OnBecameInvisible()
    {
        newVisible = false;
    }

    public LuaFunction onBecameVisible = null;
    void OnBecameVisible()
    {
        newVisible = true;
    }

    public LuaFunction onCollisionEnter = null;
    void OnCollisionEnter(Collision collision)
    {
        if (onCollisionEnter != null)
        {
            onCollisionEnter.BeginPCall();
            onCollisionEnter.PushObject(collision);
            onCollisionEnter.PCall();
            onCollisionEnter.EndPCall();
        }
    }

    public LuaFunction onCollisionExit = null;
    void OnCollisionExit(Collision collision)
    {
        if (onCollisionExit != null)
        {
            onCollisionExit.BeginPCall();
            onCollisionExit.PushObject(collision);
            onCollisionExit.PCall();
            onCollisionExit.EndPCall();
        }
    }

    public LuaFunction onCollisionStay = null;
    void OnCollisionStay(Collision collision)
    {
        if (onCollisionStay != null)
        {
            onCollisionStay.BeginPCall();
            onCollisionStay.PushObject(collision);
            onCollisionStay.PCall();
            onCollisionStay.EndPCall();
        }
    }

    public LuaFunction onDisable = null;
    void OnDisable()
    {
        if (onDisable != null)
        {
            onDisable.BeginPCall();
            onDisable.PCall();
            onDisable.EndPCall();
        }
    }

    public LuaFunction onEnable = null;
    void OnEnable()
    {
        if (onEnable != null)
        {
            onEnable.BeginPCall();
            onEnable.PCall();
            onEnable.EndPCall();
        }
    }

    public LuaFunction onRenderImage = null;
    void OnRenderImage(RenderTexture src,  RenderTexture dest)
    {
        if (onRenderImage != null)
        {
            onRenderImage.BeginPCall();
            onRenderImage.PushObject(src);
            onRenderImage.PushObject(dest);
            onRenderImage.PCall();
            onRenderImage.EndPCall();
        }
    }

    public LuaFunction onTriggerEnter = null;
    void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter != null)
        {
            onTriggerEnter.BeginPCall();
            onTriggerEnter.PushObject(other);
            onTriggerEnter.PCall();
            onTriggerEnter.EndPCall();
        }
    }

    public LuaFunction onTriggerExit = null;
    void OnTriggerExit(Collider other)
    {
        if (onTriggerExit != null)
        {
            onTriggerExit.BeginPCall();
            onTriggerExit.PushObject(other);
            onTriggerExit.PCall();
            onTriggerExit.EndPCall();
        }
    }

    public LuaFunction onTriggerStay = null;
    void OnTriggerStay(Collider other)
    {
        if (onTriggerStay != null)
        {
            onTriggerStay.BeginPCall();
            onTriggerStay.PushObject(other);
            onTriggerStay.PCall();
            onTriggerStay.EndPCall();
        }
    }


    private void Update()
    {
        if (nowVisible == newVisible) {
            return;
        }
        nowVisible = newVisible;
        if (newVisible)
        {
            if (onBecameVisible!=null) {
                onBecameVisible.BeginPCall();
                onBecameVisible.PCall();
                onBecameVisible.EndPCall();
            }
        }
        else {
            if(onBecameInvisible!=null) {
                onBecameInvisible.BeginPCall();
                onBecameInvisible.PCall();
                onBecameInvisible.EndPCall();
            }
        }
    }


}



