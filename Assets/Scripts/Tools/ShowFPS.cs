using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour {

    public float f_UpdateInterval = 0.5F;

    private float f_LastInterval;

    private int i_Frames = 0;

    private float f_Fps;


    GUIStyle fontStyle = new GUIStyle();

    void Start()
    {

        f_LastInterval = Time.realtimeSinceStartup;

        //GUIStyle fontStyle = new GUIStyle();
        fontStyle.fontSize = 25;       //字体大小

    }

    void OnGUI()
    {

         
        if (f_Fps < 10)
            fontStyle.normal.textColor = Color.red;
        else if (f_Fps < 30)
            fontStyle.normal.textColor = Color.yellow;
        else
            fontStyle.normal.textColor = Color.green;

        GUI.Label(new Rect(0, 100, 100, 60), f_Fps.ToString("f2"), fontStyle);
    }

    void Update()
    {
        ++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
        {
            f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

            i_Frames = 0;

            f_LastInterval = Time.realtimeSinceStartup;
        }
    }
    public float GetFPS()
    {
        return f_Fps;
    }
}
