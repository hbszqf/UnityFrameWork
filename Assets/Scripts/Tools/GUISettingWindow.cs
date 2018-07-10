using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LuaFramework;
using LuaInterface;
using System;

public class GUISettingWindow 
{
    public bool IsInited { get; protected set; }

    float musicVolume;
    float soundVolume;
    float timeScale;
    bool isInnerNet;

    public void Init(LuaManager luaMgr)
    {
        object[] retMusic = Util.CallMethod("GameAudioMgrUtil", "GetMusicVolume", null);
        object[] retSound = Util.CallMethod("GameAudioMgrUtil", "GetSoundVolume", null);

        musicVolume = Convert.ToSingle(retMusic[0]);
        soundVolume = Convert.ToSingle(retSound[0]);
        timeScale = 1;
        IsInited = true;
    }

    void DrawSlide(string name, ref float refValue, System.Action<float> valueChangeCallback,
        float valueStart = 0, float valueEnd = 1,
        string valueFormat = "{0:P}", float nameWidth = 120,
        int sliderWidth = 150)
    {
        GUILayout.BeginHorizontal("box");
        {
            GUILayout.Label(name, GUILayout.Width(nameWidth));
            float preValue = refValue;
            refValue = GUILayout.HorizontalSlider(preValue, valueStart, valueEnd, GUILayout.Width(sliderWidth));
            if (refValue != preValue)
            {
                // the value has be changed
                valueChangeCallback(refValue);
            }

            GUILayout.Space(10);
            GUILayout.Label(string.Format(valueFormat, refValue));
        }
        GUILayout.EndHorizontal();
    }

    void OnMusicVolumeChanged(float newValue)
    {
        Util.CallMethod("GameAudioMgrUtil", "SetMusicVolume", newValue);
        Util.CallMethod("GameAudioMgrUtil", "SaveMusicVolume", null);
    }

    void OnSoundVolumeChanged(float newValue)
    {
        Util.CallMethod("GameAudioMgrUtil", "SetSoundVolume", newValue);
        Util.CallMethod("GameAudioMgrUtil", "SaveSoundVolume", null);
    }

    void OnTimeScaleChanged(float newValue)
    {
        Time.timeScale = newValue;
    }

    public void DoSettingWindow(int wndId)
    {
        GUIStyle style = GUI.skin.GetStyle("label");
        int preFontSize = style.fontSize;
        style.fontSize = 18;

        GUILayout.BeginVertical("box");
        DrawSlide("背景音乐：", ref musicVolume, OnMusicVolumeChanged);
        DrawSlide("音效：", ref soundVolume, OnSoundVolumeChanged);
        DrawSlide("TimeScale：", ref timeScale, OnTimeScaleChanged, 0, 3, "{0:N1}");

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("网络设置", GUILayout.Width(120));

        isInnerNet = VersionInfo.Instance.Type == VersionInfo.HoutaiType.内网;

        bool preNetState = isInnerNet;
        isInnerNet = GUILayout.Toggle(isInnerNet, "内网");
        if (preNetState != isInnerNet)
        {
            if (isInnerNet)
            {

            }

            VersionInfo.Instance.Type = isInnerNet ? VersionInfo.HoutaiType.内网 : VersionInfo.HoutaiType.外网测试;
            PlayerPrefs.SetInt("networkstate", isInnerNet ? 0 : 1);
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("进入测试场景")) 
        {
            Util.CallMethod("SpecialSceneTest", "EnterTestScene", null);
        }

        if (GUILayout.Button("退出测试场景")) 
        {
            Util.CallMethod("SpecialSceneTest", "LeaveTestScene", null);
        }
        GUILayout.EndVertical();

        style.fontSize = preFontSize;
    }
}
