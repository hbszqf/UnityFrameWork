using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 不同平台游戏资源路径关联类
/// </summary>
public class PathConfig
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public const string Version_PATH = "version.json";
    /// <summary> Assets/_Resources/ToAssetBundle</summary>
    public const string ToBundle_PATH = AppConst.DynamicAssetPathPrefix;
    /// <summary>bundles</summary>
    public const string BundleRoot_PATH = "bundles";
    /// <summary>ScenesCopy</summary>
    public const string SceneCopy_PATH = "ScenesCopy";
    /// <summary>Scenes</summary>
    public const string Scene_PATH = "Scenes";
    /// <summary>Audio</summary>
    public const string Audio_PATH = "Audio";
    /// <summary>FMod</summary>
    public const string FMod_PATH = "FMod";
    /// <summary>Audio</summary>
    public const string Model_PATH = "Model";
    /// <summary>Effect</summary>
    public const string Effect_PATH = "Effect";
    /// <summary>Actor</summary>
    public const string Actor_PATH = "Actor";
    /// <summary>ActorClothes</summary>
    public const string ActorClothes_PATH = "ActorClothes";
    /// <summary>Common</summary>
    public const string Common_PATH = "Common";
    /// <summary>UI</summary>
    public const string UI_PATH = "UI";
    /// <summary>UIAtlas</summary>
    public const string UIAtlas_PATH = "UIAtlas";
    /// <summary>Protobuf</summary>
    public const string Protobuf_PATH = "Protobuf";
    /// <summary>BehaviorsTree</summary>
    public const string BehaviorsTree_PATH = "BehaviorsTree";
    
    /// <summary>
    /// Assets/AppName/StaticArt/UI/
    /// </summary>
    public const string Sprite_PATH = AppConst.StaticPathAssetPrefix + "/UI";

    public const string Fairy_PATH = AppConst.DynamicAssetPathPrefix + "/FairyUI";
    /// <summary>Configs</summary>
    public const string Configs_PATH = "Configs";
    /// <summary>.unity3d</summary>
    public const string ExtName = AppConst.ExtName;

    /// <summary>http://10.21.210.21:10002</summary>
    public const string ASSET_SERVER_URL = "http://10.21.210.21:10002";




    /// <summary>
    /// key=assetPath, value=bundleName
    /// </summary>
    private static Dictionary<string, string> pathTab = new Dictionary<string, string>();

    /// <summary>
    /// 通过传入原始资源相对工程的路径，来获取资源的包名
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="subFolder"></param>
    /// <returns></returns>
    public static string GetBundleName(string assetPath)
    {
        assetPath =  assetPath.Replace("/", "_").Replace(@"\", "_");
        if (Path.HasExtension(assetPath))
            assetPath = assetPath.Replace(Path.GetExtension(assetPath), "");
        assetPath = assetPath.ToLower();
        if (!pathTab.ContainsKey(assetPath))
        {
            string name = string.Format("{0}{1}", GuidHelper.Create(GuidHelper.UrlNamespace, assetPath), ExtName);
            // string name = assetPath+ ExtName;
            pathTab.Add(assetPath, name.ToLower());
        }
        //Log.Wsy(string.Format("GetBundleName:{0}\n{1}", assetPath, pathTab[assetPath]));
        return pathTab[assetPath];
    }

    public static string GetAssetPath(string bundleName)
    {
        foreach(var temp in pathTab) {
            if (temp.Value == bundleName) {
                return temp.Key;
            }
        }
        //Log.Wsy(string.Format("GetBundleName:{0}\n{1}", assetPath, pathTab[assetPath]));
        return "not found";
    }

    public static string GetFairyPackageBundleName(string packageName)
    {
        //return "fairy_"+packageName+ExtName;

        return string.Format("{0}{1}", GuidHelper.Create(GuidHelper.UrlNamespace, "fairy_"+packageName), ExtName);
    }


    /// <summary>
    /// 开发环境资源的读取路径(包括预置物等)，提供给resource load的
    /// </summary>
    public static string GetFullPathForResLoad(string path)
    {
        return string.Format("{0}/{1}", ToBundle_PATH, path);
    }

    /// <summary>
    /// 不同平台关联的资源目录
    /// </summary>
    public static string GetPlatFolder()
    {
#if UNITY_STANDALONE
        return "Win";
#elif UNITY_IPHONE
        return "IOS";
#elif UNITY_ANDROID
        return "Android";
#endif
    } 

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = AppConst.AppName.ToLower();

            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    return Application.dataPath + "/../Output/Res/" + GetPlatFolder() + "/" + BundleRoot_PATH + "/";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                    return Application.streamingAssetsPath + "/" + BundleRoot_PATH + "/";
                default:
                    return Application.persistentDataPath + "/" + game + "/";
            }
        }
    }

    /// <summary>
    ///当使用LoadFromCacheOrDownload的时候，这货要加上"file:///",还他妈是三个"/"
    /// </summary>
    public static string AddPathFileHead()
    {
        string path;

#if UNITY_EDITOR
        path = "file:///";
#elif UNITY_STANDALONE_WIN
		path = "file:///";
#elif UNITY_ANDROID
    	path = "file:///";
#elif UNITY_IOS
		path = "file:";
#else
		//Desktop (Mac OS or Windows)
    	path = "file:";
#endif
        return path;
    }


}
