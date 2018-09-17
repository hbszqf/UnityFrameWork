using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FW
{
    public class ABProxy {


        /// <summary>
        /// 所代理的ab對象
        /// </summary>
        public AssetBundle ab { get; set; }

        /// <summary>
        /// ab的名字
        /// </summary>
        public string abName { get; private set; }

        public ABMgr abManager { get; private set; }

        /// <summary>
        /// 是否銷毀
        /// </summary>
        public bool isDisposed { get; private set; }


        /// <summary>
        /// 直接依赖的ab
        /// </summary>
        public HashSet<ABProxy> abSet { get; private set; }

        /// <summary>
        /// 依赖的object, 被 Resources.UnloadUnuseAsset 或者 Resources.UnloadAsset 或者 GameObject.DestroyImmediate 卸载的资源, 其若引用也会被清理
        /// </summary>
        public Dictionary<int, WeakReference> assetDict { get; private set; }

        /// <summary>
        /// 依赖的Prefab, GameObject.Destroy后, 资源会被清楚
        /// </summary>
        public Dictionary<int, GameObject> prefabDict { get; private set; }

        /// <summary>
        /// 是否常驻
        /// </summary>
        public bool isResident { get; set; }

        /// <summary>
        /// 引用数量
        /// </summary>
        public int refCount = 0;

        
        private bool _isReady = false;
        /// <summary>
        /// 是否准备就绪
        /// </summary>
        public bool isReady {
            get {
               
                if (_isReady)
                {
                    return true;
                }

                if (this.ab == null)
                {
                    return false;
                }

                if (abSet != null && abSet.Count > 0)
                {
                    foreach (var abProxy in abSet)
                    {
                        if (abProxy.ab == null) {
                            return false;
                        }
                    }
                }

                this._isReady = true;
                return true;
            }
            
        }


        public void AddRef()
        {
            this.refCount += 1;
        }

        public void RemoveRef()
        {
            this.refCount -= 1;
        }

        /// <summary>
        /// 标记tag 用于判断是否需要清理
        /// </summary>
        public int sweepMark
        {
            get {
                
                return _sweepMark;
            }
            set {
                if (_sweepMark != value)
                {
                    _sweepMark = value;
                    if (abSet != null)
                    {
                        foreach (var item in abSet)
                        {
                            item.sweepMark = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 标记位
        /// </summary>
        private int _sweepMark = 0;
    

        /// <summary>
        /// 造函数
        /// </summary>
        /// <param name="ab">代理这个AssetBundle</param>
        public ABProxy(ABMgr abManager, string abName)
        {
            this.abName = abName;
            this.abManager = abManager;
            //this.ab = ab;
            this.isDisposed = false;
        }

        /// <summary>
        /// 设置清扫标志
        /// </summary>
        /// <param name="mark"></param>
        public void SetSweepMark(int mark)
        {
            //已经销毁了
            if (isDisposed)
            {
                return;
            }
            sweepMark = mark;
            if (abSet != null)
            {
                foreach (var item in abSet)
                {
                    item.sweepMark = mark;
                }
            }
            
        }

        /// <summary>
        /// 标记
        /// </summary>
        /// <param name="mark"></param>
        public void Mark(int mark)
        {
            //已经销毁了
            if (isDisposed)
            {
                return;
            }

            //是否常驻
            if (isResident)
            {
                this.SetSweepMark(mark);
                return;
            }

            //还有外部引用
            if (this.refCount > 0)
            {
                this.SetSweepMark(mark);
                return;
            }

            //没准备好
            if (!isReady) {
                this.SetSweepMark(mark);
                return;
            }

            

            if (this.ab == null) {
                return;
            }

            //清理asset
            if (assetDict != null && assetDict.Count > 0)
            {
                //标记已经释放的asset
                List<int> list = null;
                foreach (var item in assetDict)
                {
                    if ( item.Value.IsAlive == false || item.Value.Target == null)
                    {
                        if(list== null)
                        {
                            list = new List<int>();
                        }
                        list.Add(item.Key);
                    }
                }

                //清理asset
                if (list!=null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        assetDict.Remove(list[i]);
                    }
                }

                //清理完成后, 如果还有关联的object对象, 则清0
                if (assetDict.Count > 0)
                {
                    this.SetSweepMark(mark);
                    return;
                }
            }

            //清理prefab
            if (prefabDict != null && prefabDict.Count > 0)
            {
                //标记已经释放的asset
                List<int> list = null;
                foreach (var item in prefabDict)
                {
                    if (item.Value == null)
                    {
                        if (list == null)
                        {
                            list = new List<int>();
                        }
                        list.Add(item.Key);
                    }
                }

                //清理asset
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        prefabDict.Remove(list[i]);
                    }
                }

                //清理完成后, 如果还有关联的object对象, 则清0
                if (prefabDict.Count > 0)
                {
                    this.SetSweepMark(mark);
                    return;
                }
            }

        }

        public void AddDependABProxy(ABProxy abProxy)
        {
            if(abSet==null)
            {
                abSet = new HashSet<ABProxy>();
            }
            abSet.Add(abProxy);
        }

        /// <summary>
        /// 创建一个Prefab
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public GameObject LoadPrefab(string assetName)
        {
            if (ab == null)
            {
                return null;
            }
            GameObject go = GetPrefab(assetName);
            if (go==null) return null;
            GameObject clone = GameObject.Instantiate(go);
            if (prefabDict == null)
            {
                prefabDict = new Dictionary<int, GameObject>();
            }
            prefabDict.Add(clone.GetHashCode(), clone);
            return clone;
        }

        /// <summary>
        /// 获取一个Prefab
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private GameObject GetPrefab(string assetName)
        {
            if (ab == null)
            {
                return null;
            }
            GameObject go = this.ab.LoadAsset(assetName) as GameObject;
            return go;
        }


        /// <summary>
        /// 载任意asset
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName)
        {
            if (ab == null)
            {
                return null;
            }

            UnityEngine.Object o = this.ab.LoadAsset(assetName);
            if(o==null)
            {
                return null;
            }

            if (assetDict == null)
            {
                assetDict = new Dictionary<int, WeakReference>();
            }

            if (!assetDict.ContainsKey(o.GetHashCode()))
            {
                assetDict.Add(o.GetHashCode(), new WeakReference(o));
            }
            return o;
        }

        /// <summary>
        /// 加载任意指定类型的asset
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName, Type T)
        {
            if (ab == null)
            {
                return null;
            }

            UnityEngine.Object o = this.ab.LoadAsset(assetName,T);
            if (o == null)
            {
                return null;
            }
            if (assetDict == null)
            {
                assetDict = new Dictionary<int, WeakReference>();
            }

            int hash = o.GetHashCode();
            if (!assetDict.ContainsKey(hash))
            {
                assetDict.Add(hash, new WeakReference(o) );
            }
        
            return o;
        }

        /// <summary>
        /// 加载ab里面所有的Asset
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Object[] LoadAllAsset()
        {
            if (ab == null)
            {
                return null;
            }

            UnityEngine.Object[] ret = this.ab.LoadAllAssets();
            if (ret == null)
            {
                return null;
            }
            if (assetDict == null)
            {
                assetDict = new Dictionary<int, WeakReference>();
            }

            for (int i = 0; i < ret.Length; ++i)
            {
                int hash = ret.GetHashCode();
                if (!assetDict.ContainsKey(hash))
                {
                    assetDict.Add(hash, new WeakReference(ret[i]));
                }
            }
            return ret;
        }

        /// <summary>
        /// 加载subAssets
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object[] LoadSubAssets(string assetName)
        {
            if (ab == null)
            {
                return null;
            }
            //加载主Asset, 并缓存
            this.LoadAsset(assetName);
            //
            return this.ab.LoadAssetWithSubAssets(assetName);
        }

        /// <summary>
        /// 加载SubAssets
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public UnityEngine.Object[] LoadSubAssets(string assetName, Type type)
        {
            if (ab == null)
            {
                return null;
            }
            //加载主Asset, 并缓存
            this.LoadAsset(assetName, type);
            //
            return this.ab.LoadAssetWithSubAssets(assetName, type);
        }


        /// <summary>
        /// 卸载Asset
        /// </summary>
        /// <param name="obj"></param>
        public void UnloadAsset(UnityEngine.Object o)
        {
            int hash = o.GetHashCode();
            if (assetDict.ContainsKey(hash))
            {
                if (o is GameObject)
                {
                    //GameObject.DestroyImmediate(o, true);
                    //Resources.UnloadAsset(o);
                }
                else
                {
                    Resources.UnloadAsset(o);
                }
                
                assetDict.Remove(hash);
            }
        }


        /// <summary>
        /// 卸载Prefab
        /// </summary>
        /// <param name="obj"></param>
        public void UnloadPrefab(UnityEngine.GameObject o)
        {
            int hash = o.GetHashCode();
            if (prefabDict.ContainsKey(hash))
            {
                GameObject.Destroy(o);
                prefabDict.Remove(hash);
            }
        }

        #region IDisposable
        public void Dispose()
        {
            isDisposed = true;
            isResident = false;
            if (ab != null)
            {
                try
                {
                    ab.Unload(true);   
                }
                catch (Exception)
                {
                    Log.Print("卸载AB出错:" + this.abName );
                }
                ab = null;

            }

                if (abSet != null)
            {
                abSet.Clear();
                abSet = null;
            }
        
            if(prefabDict!=null)
            { 
                prefabDict.Clear();
                prefabDict = null;
            }

            if(assetDict != null)
            {
                assetDict.Clear();
                assetDict = null;
            }  
        }
        #endregion



}
}