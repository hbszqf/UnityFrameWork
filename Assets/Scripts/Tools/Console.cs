using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using LuaFramework;

namespace Consolation
{
    /// <summary>
    /// A console to display Unity's debug logs in-game.
    /// </summary>
    class Console : MonoBehaviour
    {
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }
        Vector2 scrollPosition; 
        #region Inspector Settings
        private Rect startRect = new Rect(10, 180, 75, 50);// new Rect (Screen.width - 80, 150, 75, 50);
        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.BackQuote;

        public int fontSize = 22;

        /// <summary>
        /// Whether to open the window by shaking the device (mobile-only).
        /// </summary>
        public bool shakeToOpen = true;

        /// <summary>
        /// The (squared) acceleration above which the window should open.
        /// </summary>
        public float shakeAcceleration = 3f;

        public bool ShowConsoleButton = true;

        #endregion

        readonly List<Log> logs = new List<Log> ();

        bool visible;

        bool collapse = false;

        // Visual elements:

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        const string windowTitle = "Console";
        const int margin = 100;
        static readonly GUIContent clearLabel = new GUIContent ("Clear\nthis", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent ("Collapse", "Hide repeated messages.");

        readonly Rect titleBarRect = new Rect (0, 0, 10000, 20);
        Rect windowRect = new Rect (10, 10, Screen.width - (margin * 2), Screen.height - 20);

        public bool OutPut = true;
        private string outpath;

        bool isSettingWndVisible = false;
        Rect rcSettingWnd = new Rect(new Vector2(80, 120), new Vector2(480, 720));
        GUISettingWindow settingWnd = new GUISettingWindow();

        void Awake ()
        {
            string filePath = "";

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
            filePath = Application.persistentDataPath;    
#else
            string path = Application.dataPath;
            filePath = string.Format ("{0}//../GameDatas", path);
            if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
#endif
            outpath = string.Format ("{0}/Log {1}.txt", filePath, DateTime.Now.ToString ("s").Replace(":","_"));//"/outLog.txt";

            if (System.IO.File.Exists (outpath))
            {
                File.Delete (outpath);
            }
          
            StartCoroutine(GetLuaManger());
        }

        private void Start()
        {
            //windowRect = new Rect(10, 10, Screen.width - (margin * 2), Screen.height - 20);
            windowRect = new Rect(10, 10, 600, 960);
        }

        IEnumerator GetLuaManger()
        {
            LuaManager luaMgr = null;
            while (luaMgr == null)
            {
                luaMgr = AppFacade.Instance.GetManager<LuaManager>();
                yield return 0;
            }

            while (!luaMgr.IsInitLuaOk())
            {
                yield return 0;
            }

            settingWnd.Init(luaMgr);
        }

        void OnEnable ()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable ()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void Update ()
        {
            if (Input.GetKeyDown (toggleKey))
            {
                visible = !visible;
            }

            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
            {
                visible = !visible;
            }
        }

        //float timeScale = 1;

        void OnGUI ()
        {
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.verticalScrollbar.fixedWidth = 35;
            GUI.skin.verticalScrollbarThumb.fixedWidth = 35;

            if (ShowConsoleButton)
            {
                if (GUI.Button (startRect/*new Rect (Screen.width - 100, 60, 75, 50)*/, "Console"))
                    visible = !visible;
            }

            if (visible)
            {
                windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, windowTitle);
            }

            // show setting window button
            if (settingWnd.IsInited)
            {
                if (GUI.Button(new Rect(startRect.xMin, startRect.yMax, startRect.width, startRect.height), "Setting"))
                {
                    isSettingWndVisible = !isSettingWndVisible;
                }
            }
            if (isSettingWndVisible)
            {
                rcSettingWnd = GUILayout.Window(2, rcSettingWnd, DoSettingWindow, "Setting Window");
            }
        }


        void DoSettingWindow(int wndId)
        {
            settingWnd.DoSettingWindow(wndId);
        }

        /// <summary>
        /// A window that displayss the recorded logs.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        void ConsoleWindow (int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView (scrollPosition);

            // Iterate through the recorded logs.
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                // Combine identical messages if collapse option is chosen.
                if (collapse)
                {
                    var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                    if (messageSameAsPrevious)
                    {
                        continue;
                    }
                }

                GUI.contentColor = logTypeColors[log.type];
                GUILayout.Label (log.message);
            }

            GUILayout.EndScrollView ();

            GUI.contentColor = Color.white;

            GUILayout.BeginHorizontal ();

            if (GUILayout.Button (clearLabel) || logs.Count >= 500)
            {               
                logs.Clear ();
            }

            collapse = GUILayout.Toggle (collapse, collapseLabel, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true), GUILayout.Width (80), GUILayout.Height (60));
            if (collapse)
                scrollPosition.y = Mathf.Infinity;

            GUILayout.EndHorizontal ();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow (titleBarRect);
        }

        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog (string message, string stackTrace, LogType type)
        {
            logs.Add (new Log
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });

            //#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
            if (OutPut)
            {
                using (StreamWriter writer = new StreamWriter (outpath, true, Encoding.UTF8))
                {                    
                    writer.WriteLine (message);
                }
            }
            //#endif
        }
    }
}