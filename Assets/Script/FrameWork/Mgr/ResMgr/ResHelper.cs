using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FW
{
    public class ResHelper
    {

        public const string RES_PATH = "Assets/Res/";
        public const string UI_PATH = RES_PATH + "UI/";
        public const string SBF_NAME = "sbf";
        public const string AB_FOLDER_NAME = "AB";


        #region 通用对外接口
        /// <summary>
        /// 根据asset的路径 获取ab的名字
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetABName(string assetPath)
        {
            assetPath = assetPath.ToLower().Replace("/", "|").Replace(@"\", "|");
            MD5 md5 = MD5.Create();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(assetPath);
            byte[] hash = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <returns></returns>
        public static string GetResFolder()
        {
            if (Application.isEditor)
            {
                return GetEditorResFolder();
            }
            else
            {
                return GetAppResFolder();
            }
        }

        /// <summary>
        /// 获取AB路径
        /// </summary>
        /// <returns></returns>
        public static string GetABFolder()
        {
            return GetResFolder() + ResHelper.AB_FOLDER_NAME + "/";
        }

        /// <summary>
        /// 获取lua的路径
        /// </summary>
        /// <returns></returns>
        public static string GetLuaFolder()
        {
            return GetResFolder() + "Lua/";
        }

        //获取sbf的路径
        public static string GetSBFPath()
        {
            return GetResFolder() + "sbf";
        }

#endregion

        #region 私有方法
        private static string GetOsDir()
        {
            
#if UNITY_STANDALONE
            return "Win";
#elif UNITY_ANDROID
            return "Android";            
#elif UNITY_IPHONE
            return "iOS";        
#else
            throw new System.Exception();
#endif
        }

        private static string GetEditorResFolder()
        {
            return Application.dataPath + "/../../StreamingAssets/" + GetOsDir() + "/Res/";
        }

        private static string GetAppResFolder()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                    return Application.streamingAssetsPath + "/Res/";
                default:
                    return Application.persistentDataPath + "/Res/";
            } 
        }
        #endregion

        #region 编辑器模式对外接口
#if UNITY_EDITOR
        public static string GetResFolder(UnityEditor.BuildTarget buildTarget)
        {
            switch (buildTarget) {
                case UnityEditor.BuildTarget.Android:
                    return Application.dataPath + "/../External/StreamingAssets/Android/Res/";
                    
                case UnityEditor.BuildTarget.iOS:
                    return Application.dataPath + "/../External/StreamingAssets/iOS/Res/";
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return Application.dataPath + "/../External/StreamingAssets/Win/Res/";
                default:
                    throw new System.Exception();
            }
           
        }

        public static string GetABFolder(UnityEditor.BuildTarget buildTarget)
        {
            return GetResFolder(buildTarget) + "AB/";
        }

        public static string GetLuaFolder(UnityEditor.BuildTarget buildTarget)
        {
            return GetResFolder(buildTarget) + "Lua/";
        }

        //获取sbf的路径
        public static string GetSBFPath(UnityEditor.BuildTarget buildTarget)
        {
            return GetResFolder(buildTarget) + "sbf";
        }

        /// <summary>
        /// 获取编辑的Assets的父目录, Assets的目录为 GetAssetsRoot() + "Assets"
        /// </summary>
        /// <returns></returns>
        public static string GetAssetsRoot()
        {
            return Path.GetDirectoryName(Application.dataPath) + "/";
        }

        /// <summary>
        /// 获取Assets的目录
        /// </summary>
        /// <returns></returns>
        public static string GetAssetsResFolder()
        {
            return GetAssetsRoot() + ResHelper.RES_PATH;
        }

        /// <summary>
        /// 获取Assets的目录
        /// </summary>
        /// <returns></returns>
        public static string GetAssetsResWichIsNotABFolder()
        {
            return GetAssetsRoot() + "Assets/ResWhichIsNotAB/";
        }
#endif
        #endregion



    }

}
