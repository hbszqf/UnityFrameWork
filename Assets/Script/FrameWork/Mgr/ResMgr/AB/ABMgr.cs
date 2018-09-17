using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using UnityEngine;

namespace FW
{
    public class WaitForABProxy : CustomYieldInstruction
    {
        //ABManager thiz;
        ABProxy abProxy;
        
        public WaitForABProxy(ABMgr thiz, ABProxy abProxy)
        {
            //this.thiz = thiz;
            this.abProxy = abProxy;


        }

        public override bool keepWaiting
        {
            get
            {
                return this.abProxy.isReady == false;
            }
        }
    }

    public class ABMgr 
    {
        #region AB
        /// <summary>
        /// ab的根目录
        /// </summary>
        public string folder { get; private set; }

        /// <summary>
        /// manifest的名字
        /// </summary>
        public string manifestName { get; private set; }

        /// <summary>
        /// 当前缓存的AB
        /// </summary>
        public Dictionary<string, ABProxy> abDict { get; set; }


        /// <summary>
        /// 清扫标志位
        /// </summary>
        private int sweepMark = 0;

        /// <summary>
        /// manifest
        /// </summary>
        private AssetBundleManifest manifest = null;

        /// <summary>
        /// 
        /// </summary>
        public ABRequestManager abRequestManager = null;

        /// <summary>
        /// 相关的resMgr
        /// </summary>
        private BaseResMgr resMgr;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="folder">ab的跟目录</param>
        /// <param name="assetBundleManifest">ab的assetBundleManifest包名</param>
        public ABMgr Init(BaseResMgr resMgr, string folder, string manifestName)
        {
            this.resMgr = resMgr;
            this.folder = folder;
            this.manifestName = manifestName;
            this.abDict = new Dictionary<string, ABProxy>();

            this.abRequestManager = new ABRequestManager(this);

            this.abRequestManager.onABLoaded += this.OnABLoaded;

            return this;
        }

        void OnABLoaded(string abName, AssetBundle ab)
        {
            ABProxy abProxy = this.GetOrCreateABProxy(abName);
            abProxy.ab = ab;
        }

        public void Update()
        {
            this.abRequestManager.Update();
        }


        /// <summary>
        /// 删除没有使用的
        /// </summary>
        public void UnloadUnused()
        {
            this.sweepMark++;
            //标记
            foreach (var item in abDict)
            {
                ABProxy abProxy = item.Value;
                abProxy.Mark(this.sweepMark);
            }


            //清扫
            List<string> list = null;
            foreach (var item in abDict)
            {
                ABProxy abProxy = item.Value;
                if (abProxy.sweepMark != sweepMark)
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }

                    list.Add(item.Key);
                }
            }
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string abName = list[i];
                    ABProxy ab = abDict[abName];
                    ab.Dispose();
                    abDict.Remove(list[i]);
                }
            }

        }

        /// <summary>
        /// 获取ABProxy
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public ABProxy GetABProxy(string abName)
        {
            if (abDict.ContainsKey(abName))
            {
                var ab = abDict[abName];
                if (ab.isDisposed)
                {
                    return null;
                }
                return ab;
            }
            else
            {
                return null;
            }
        }

        public ABProxy GetOrCreateABProxy(string abName)
        {
            ABProxy abProxy = this.GetABProxy(abName);
            if (abProxy == null)
            {
                //主体
                abProxy = new ABProxy(this, abName);
                this.abDict.Add(abName, abProxy);
                this.abRequestManager.AddAbRequest(abName);

                //依赖项
                string[] dependABNameArray = this.GetManifest().GetAllDependencies(abName);
                for (int i = 0; i < dependABNameArray.Length; i++)
                {
                    string dependABName = dependABNameArray[i];
                    ABProxy dependABProxy = this.GetOrCreateABProxy(dependABName);
                    abProxy.AddDependABProxy(dependABProxy);

                }
            }
            return abProxy;
        }


        /// <summary>
        /// 获取manifest
        /// </summary>
        /// <returns></returns>
        public AssetBundleManifest GetManifest()
        {
            if (this.manifest == null)
            {
                string path = Path.Combine(this.folder, this.manifestName);
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                this.manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                ab.Unload(false);
            }
            return this.manifest;

        }

        /// <summary>
        /// 加载一个Prefab
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="fcallback"></param>
        public void LoadPrefab(string abName, string assetName, System.Action<GameObject> fcallback)
        {
            ResMgr.inst.StartCoroutine(LoadPrefabCor(abName, assetName, fcallback));
        }


        /// <summary>
        /// 加载一个Prefab
        /// </summary>
        /// <param name="abName">ab名字</param>
        /// <param name="assetName">asset名字</param>
        /// <param name="fcallback">完成回调</param>
        private IEnumerator LoadPrefabCor(string abName, string assetName, System.Action<GameObject> fcallback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里可以改为LoadPrefabSync
            GameObject go = abProxy.LoadPrefab(assetName);

            fcallback(go);

            yield break;
        }

        /// <summary>
        /// 加载一个asset
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="fcallback"></param>
        public void LoadAsset(string abName, string assetName, System.Action<UnityEngine.Object> fcallback)
        {
            ResMgr.inst.StartCoroutine(LoadAssetCor(abName, assetName, fcallback));
        }

        /// <summary>
        /// 加载一个asset协程
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="fcallback"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetCor(string abName, string assetName, System.Action<UnityEngine.Object> fcallback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里以后考虑改为LoadAssetSync
            UnityEngine.Object go = abProxy.LoadAsset(assetName);

            //yield return Resources.UnloadUnusedAssets();
            fcallback(go);

            yield break;
        }

        /// <summary>
        /// 指定类型加载一个asset
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="T"></param>
        /// <param name="fcallback"></param>
        public void LoadAsset(string abName, string assetName, Type T, System.Action<UnityEngine.Object> fcallback)
        {
            resMgr.StartCoroutine(LoadAssetCor(abName, assetName, T, fcallback));
        }

        /// <summary>
        /// 指定类型加载一个asset协程
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="fcallback"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetCor(string abName, string assetName, Type T, System.Action<UnityEngine.Object> fcallback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里以后考虑改为LoadAssetSync
            UnityEngine.Object go = abProxy.LoadAsset(assetName, T);

            fcallback(go);

            yield break;
        }

        /// <summary>
        /// 指定类型加载所有asset
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="fcallback"></param>
        public void LoadAllAsset(string abName, System.Action<UnityEngine.Object[]> fcallback)
        {
            resMgr.StartCoroutine(LoadAllAssetCor(abName, fcallback));
        }

        /// <summary>
        /// 指定类型加载所有asset协程
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="fcallback"></param>
        /// <returns></returns>
        private IEnumerator LoadAllAssetCor(string abName, System.Action<UnityEngine.Object[]> fcallback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里以后考虑改为LoadAssetSync
            UnityEngine.Object[] go = abProxy.LoadAllAsset();

            fcallback(go);

            yield break;
        }

        /// <summary>
        /// 加载ABProxy
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="fcallback"></param>
        public void LoadABProxy(string abName, System.Action<ABProxy> fcallback)
        {
            resMgr.StartCoroutine(LoadABProxyCor(abName, fcallback));
        }

        /// <summary>
        /// 加载一个AbProxy异步接口
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public WaitForABProxy LoadABProxySync(ABProxy abProxy)
        {
            return new WaitForABProxy(this, abProxy);
        }

        /// <summary>
        /// 加载一个AbProxy
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="fcallback"></param>
        /// <returns></returns>
        private IEnumerator LoadABProxyCor(string abName, System.Action<ABProxy> fcallback = null)
        {
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            fcallback(abProxy);

            yield break;
        }

        /// <summary>
        /// 加载SubAssets
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadSubAssets(string abName, string assetName, System.Action<UnityEngine.Object[]> callback)
        {
            resMgr.StartCoroutine(LoadSubAssetsCor(abName, assetName, callback));
        }

        /// <summary>
        /// 加载SubAsset协程接口
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator LoadSubAssetsCor(string abName, string assetName, System.Action<UnityEngine.Object[]> callback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里以后考虑改为LoadAssetSync
            UnityEngine.Object[] objArray = abProxy.LoadSubAssets(assetName);

            callback(objArray);

            yield break;
        }


        /// <summary>
        /// 加载SubAssets
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadSubAssets(string abName, string assetName, Type type, System.Action<UnityEngine.Object[]> callback)
        {
            resMgr.StartCoroutine(LoadSubAssetsCor(abName, assetName, type, callback));
        }

        /// <summary>
        /// 加载SubAssets协程版
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator LoadSubAssetsCor(string abName, string assetName, Type type, System.Action<UnityEngine.Object[]> callback)
        {
            //读取abProxy
            ABProxy abProxy = GetOrCreateABProxy(abName);
            if (!abProxy.isReady)
            {
                abProxy.AddRef();
                yield return LoadABProxySync(abProxy);
                abProxy.RemoveRef();
            }

            //这里以后考虑改为LoadAssetSync
            UnityEngine.Object[] objArray = abProxy.LoadSubAssets(assetName, type);

            callback(objArray);

            yield break;
        }


        /// <summary>
        /// 强制卸载一个ABProxy
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isABUnload">是否AB已经被卸载了, 如果已经被卸载, proxy就不再次卸载ab</param>
        public void UnloadABProxy(string abName)
        {
            ABProxy abProxy = this.GetABProxy(abName);
            if (abProxy != null)
            {
                abProxy.Dispose();
                this.abDict.Remove(abName);
            }
        }

        /// <summary>
        /// 卸载一个Prefab
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool UnloadPrefab(string abName, GameObject go)
        {
            ABProxy abProxy = GetABProxy(abName);
            if (abProxy == null)
            {
                return false;
            }

            abProxy.UnloadPrefab(go);
            return true;
        }

        /// <summary>
        /// 卸载一个Asset
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        public bool UnloadAsset(string abName, UnityEngine.Object uo)
        {
            ABProxy abProxy = GetABProxy(abName);
            if (abProxy == null)
            {
                return false;
            }
            abProxy.UnloadAsset(uo);
            return true;
        }

        #endregion


    }

}
