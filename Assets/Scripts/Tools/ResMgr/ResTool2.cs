using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UObject = UnityEngine.Object;
using FairyGUI;

namespace LuaFramework
{ 


    public abstract class ResTool2 {

        

#if UNITY_EDITOR && DEV_MODE
        public bool isABMode = false;
#else
        public bool isABMode = true;
#endif

        /// <summary>
        /// 单例
        /// </summary>
        static RealResTool _instance = null;
        /// <summary>
        /// 单例
        /// </summary>
        public static ResTool2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RealResTool();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 相对路径转换绝对路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        protected string RelativePath2AssetPath(string relativePath)
        {
            return PathConfig.ToBundle_PATH + "/" + relativePath;
        }

        /// <summary>
        /// 加载一个Prefab(Instantiate后的), 不使用后 GameObejct.Destroy来销毁
        /// </summary>
        /// <param name="relative">相对于 DynamicArt的路径</param>
        /// <param name="callback">完成回调</param>
        public abstract void CreatePrefab(string relative, Action<GameObject> callback);

        /// <summary>
        /// 加载一个Asset, Resources.UnloadUnusedAssets(当没有引用的时候) 或者 直接GameObject.DestroyImm 销毁
        /// </summary>
        /// <param name="relative"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public abstract void LoadAsset(string relative, Type type, Action<UObject> callback);

        /// <summary>
        /// 加载SubAssets
        /// </summary>
        /// <param name="relative"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public abstract void LoadSubAssets(string relative, Type type, Action<UObject[]> callback);
        public abstract void LoadSubAssets(string relative, Action<UObject[]> callback);

        /// <summary>
        /// 加载FairyGUI的Package
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="callback"></param>
        public abstract void LoadFairyUIPackage(string packageName, Action<UIPackage> callback);

        public virtual void UnloadFairyUIPackage(string packageName)
        {
            UIPackage.RemovePackage(packageName);
        }

        /// <summary>
        /// 卸载不使用的资源
        /// </summary>
        public abstract void UnloadUnused(Action callback);

        /// <summary>
        /// 卸载一个Asset
        /// </summary>
        /// <param name="relative"></param>
        /// <param name="ui"></param>
        public abstract bool UnloadAsset(string relative, UObject ui);

        //卸载一个Prefab
        public abstract bool DestroyPrefab(string relative, GameObject go);

//#if UNITY_EDITOR && DEV_MODE
//#else
        public abstract void LoadABProxy(string abName, Action<AB.ABProxy> callback);
        //public abstract void LoadABProxyList(List<string> abNameList, Action callback);
        //public abstract ABProxy GetABProxy(string abName);
//#endif




    }
#if UNITY_EDITOR && DEV_MODE
    public class RealResTool : ResTool2
    {
        protected const float delay = 0.2f;
        public override void LoadAsset(string relative, Type type, Action<UObject> callback)
        {
            Timers.inst.Add(UnityEngine.Random.Range(0.05f, delay), 1, (time) => {
                string assetpath = RelativePath2AssetPath(relative);
                UObject uo = UnityEditor.AssetDatabase.LoadAssetAtPath(assetpath, type);
                if (uo == null)
                {
                    Log.Print(string.Format("<color=red>Asset Load Error:</color>{0}\n", assetpath));
                }
                callback(uo);
            });    //模拟加载延迟，方便调试bug
        }

        public override void CreatePrefab(string relative, Action<GameObject> callback)
        {
            
            //callback(GameObject.Instantiate<GameObject>(go));
            Timers.inst.Add(UnityEngine.Random.Range(0.05f, delay), 1, (time) => {
                string assetpath = RelativePath2AssetPath(relative);
                GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
                if (go == null)
                {
                    Log.Print(string.Format("<color=red>Prefab Load Error:</color>{0}\n", assetpath));
                }
                callback(GameObject.Instantiate<GameObject>(go));
            });    //模拟加载延迟，方便调试bug
        }

        public override void UnloadUnused(Action callback)
        {
            Log.Print("UnloadUnused");
            callback();
        }

        public override void LoadFairyUIPackage(string packageName, Action<UIPackage> callback)
        {
            Timers.inst.Add(delay, 1, (time) => {
                string assetpath = PathConfig.Fairy_PATH + "/" + packageName;
                UIPackage uiPackage = UIPackage.AddPackage(assetpath, (name, extension, type) =>
                {
                    UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(name + extension, type);
                    return obj;
                });
                callback(uiPackage);
            });    //模拟加载延迟，方便调试bug
        }

        public override void LoadSubAssets(string relative, Type type, Action<UObject[]> callback)
        {

            //callback(list.ToArray());
            Timers.inst.Add(delay, 1, (time) => {
                string assetpath = RelativePath2AssetPath(relative);
                UObject[] objArray = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetpath);
                List<UObject> list = new List<UObject>();
                foreach (var item in objArray)
                {
                    if (item.GetType() == type) list.Add(item);
                }
                callback(list.ToArray());
            });
        }

        public override void LoadSubAssets(string relative, Action<UObject[]> callback)
        {
            
            //callback(objArray);
            Timers.inst.Add(delay, 1, (time) => {
                string assetpath = RelativePath2AssetPath(relative);
                UObject[] objArray = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetpath);
                callback(objArray);
            });
        }

        public override void LoadABProxy(string abName, Action<AB.ABProxy> callback)
        {
            throw new NotImplementedException();
        }

        public override bool UnloadAsset(string relative, UObject uo)
        {
            if (uo.GetType() == typeof(GameObject))
            { return false; }
            Resources.UnloadAsset(uo);
            return true;
        }

        public override bool DestroyPrefab(string relative, GameObject go)
        {
            GameObject.Destroy(go);
            return true;
        }
    }

#else
    public class RealResTool : ResTool2
    {
        private AB.ABManager GetABManager()
        {
            return AB.ABManager.Instance;
        }

        public override void LoadABProxy(string abName, Action<AB.ABProxy> callback)
        {
            GetABManager().LoadABProxy(abName, callback);
        }

        public override void LoadAsset(string relative, Type type, Action<UObject> callback)
        {
            string assetpath = RelativePath2AssetPath(relative);
            GetABManager().LoadAsset(PathConfig.GetBundleName(assetpath), assetpath, type, callback);
        }

        public override void CreatePrefab(string relative, Action<GameObject> callback)
        {
            string assetpath = RelativePath2AssetPath(relative);
            GetABManager().LoadPrefab(PathConfig.GetBundleName(assetpath), assetpath,  callback);
        }

        public override void UnloadUnused(Action callback)
        {
            GetABManager().StartCoroutine(UnloadUnusedCor(callback));
        }

        public IEnumerator UnloadUnusedCor(Action callback)
        { 
            yield return Resources.UnloadUnusedAssets(); ;
            System.GC.Collect();
            GetABManager().UnloadUnused();
            callback();
        }

        public void LoadABProxyList(List<string> abNameList, Action callback)
        {
            GetABManager().LoadABProxyList(abNameList, callback);
        }

        public AB.ABProxy GetABProxy(string abName)
        {
            return GetABManager().GetABProxy(abName);
        }

        public override void LoadFairyUIPackage(string packageName, Action<UIPackage> callback)
        {
            string abName = PathConfig.GetFairyPackageBundleName(packageName);
            GetABManager().LoadABProxy(abName, abProxy => {
                if (abProxy.ab == null)
                {
                    Debug.LogErrorFormat("LoadFairyUIPackage失败:{0}", packageName);
                    callback(null);
                }
                else {
                    UIPackage uiPackage = UIPackage.AddPackage(abProxy.ab, false);
                    abProxy.isResident = true;
                    //回调
                    callback(uiPackage);
                }
                
            });
        }

        public override void UnloadFairyUIPackage(string packageName) {
            UIPackage.RemovePackage(packageName);
            string abName = PathConfig.GetFairyPackageBundleName(packageName);
            GetABManager().UnloadABProxy(abName);
        }
        public override void LoadSubAssets(string relative, Type type, Action<UObject[]> callback)
        {
            string assetpath = RelativePath2AssetPath(relative);
            GetABManager().LoadSubAssets(PathConfig.GetBundleName(assetpath), assetpath, type, callback);
        }

        public override void LoadSubAssets(string relative, Action<UObject[]> callback)
        {
            string assetpath = RelativePath2AssetPath(relative);
            GetABManager().LoadSubAssets(PathConfig.GetBundleName(assetpath), assetpath, callback);
        }

        public override bool UnloadAsset(string relative, UObject uo)
        {
            string assetpath = RelativePath2AssetPath(relative);
            string abName = PathConfig.GetBundleName(assetpath);
            return GetABManager().UnloadAsset(abName, uo);
        }

        public override bool DestroyPrefab(string relative, GameObject go)
        {
            string assetpath = RelativePath2AssetPath(relative);
            string abName = PathConfig.GetBundleName(assetpath);
            return GetABManager().UnloadPrefab(abName, go);
        }
    }
#endif
}

