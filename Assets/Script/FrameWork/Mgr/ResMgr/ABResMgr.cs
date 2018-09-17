using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

#if AB_MODE || !UNITY_EDITOR

namespace FW {
    public class ABResMgr : BaseResMgr
    {

        ABMgr abMgr = new ABMgr();


        private void Awake()
        {
            abMgr.Init(this, ResHelper.GetABFolder(), ResHelper.AB_FOLDER_NAME);
        }

        void Update()
        { 
            abMgr.Update();
        }

        public override void LoadAsset(string relative, Action<UnityEngine.Object> callback)
        {
            string assetPath = RelativePath2AssetPath(relative);
            string abName = ResHelper.GetABName(assetPath);
            abMgr.LoadAsset(abName, assetPath, callback);
        }

        public override bool UnloadAsset(string relative, UnityEngine.Object uo)
        {
            string assetPath = RelativePath2AssetPath(relative);
            string abName = ResHelper.GetABName(assetPath);
            abMgr.UnloadAsset(abName, uo);
            return true;
        }

        public override void LoadFairyUIPackage(string packageName, Action<UIPackage> callback)
        {
            string abName = ResHelper.GetABName(packageName);
            abMgr.LoadABProxy(abName, abProxy => {
                if (abProxy.ab == null)
                {
                    Debug.LogErrorFormat("LoadFairyUIPackage失败:{0}", packageName);
                    callback(null);
                }
                else
                {
                    UIPackage uiPackage = UIPackage.AddPackage(abProxy.ab, false);
                    abProxy.isResident = true;
                    //回调
                    callback(uiPackage);
                }

            });
        }

        
        public override void UnloadFairyUIPackage(string packageName)
        {
            UIPackage.RemovePackage(packageName);
            string abName = ResHelper.GetABName(packageName);
            abMgr.UnloadABProxy(abName);
        }



        public override void UnloadUnused(Action callback)
        {
            abMgr.UnloadUnused();
            callback();
        }

        public override void CreatePrefab(string relative, Action<GameObject> callback)
        {
            string assetPath = RelativePath2AssetPath(relative);
            string abName = ResHelper.GetABName(assetPath);
            abMgr.LoadPrefab(abName, assetPath, callback);
        }

        public void LoadABProxy(string abName, Action<ABProxy> callback)
        {
            abMgr.LoadABProxy(abName, callback);
        }

        public ABProxy GetABProxy(string abName)
        {
            return abMgr.GetABProxy(abName);
        }

        public override bool DestroyPrefab(string relative, GameObject go)
        {
            string assetPath = RelativePath2AssetPath(relative);
            string abName = ResHelper.GetABName(assetPath);
            abMgr.UnloadPrefab(abName, go);
            return true;
        }

        public override IEnumerator UnzipRes()
        {
            //包体内的sbf
            WWW wwwInner = FileUtil.ReadFromStreamingAssets("Res/sbf");
            WWW wwwOuter = FileUtil.ReadFromPersistentData("Res/sbf");
            yield return wwwInner;
            yield return wwwOuter;

            SBF sbfInner = new SBF(wwwInner.text);
            SBF sbfOuter = new SBF(wwwOuter.text);

            //外部目录sbf
            List<SBF.SBFItem> diff,more,less;
            sbfInner.Compare(sbfOuter, out diff, out more, out less);

            //把outer中 多的和不一样的 复制出去
            List<SBF.SBFItem> move = new List<SBF.SBFItem>(diff.Count + more.Count);
            move.AddRange(diff);
            move.AddRange(more);

            //复制队列
            Dictionary<SBF.SBFItem, WWW> moveDict = new Dictionary<SBF.SBFItem, WWW>();
            //同时复制的最大数量
            int max = 30;
            //复制资源
            for (int i = 0; i < move.Count; i++)
            {
                //加入队列
                {
                    var item = move[i];
                    var www = FileUtil.ReadFromStreamingAssets("Res/" + item.path);
                    moveDict.Add(item, www);
                }

                //队列没满
                if (moveDict.Count < max)
                {
                    continue;
                }

                //等等一帧
                yield return null;

                //遍历下载队列
                while (true)
                {
                    //查找已经完成的
                    List<KeyValuePair<SBF.SBFItem, WWW>> doneList = null;
                    foreach (var pair in moveDict)
                    {
                        if (pair.Value.isDone)
                        {
                            if (doneList == null)
                            {
                                doneList = new List<KeyValuePair<SBF.SBFItem, WWW>>();
                            }
                            doneList.Add(pair);
                        }
                    }

                    if (doneList != null)
                    {
                        foreach (var pair in doneList)
                        {
                            var item = pair.Key;
                            var www = pair.Value;
                            if (www.error == null)
                            {
                                FileUtil.WriteToPersistentData("Res/" + item.path, www.bytes);
                            }
                            moveDict.Remove(item);
                        }
                    }

                    if (moveDict.Count <= max)
                    {
                        break;
                    }

                    //等等一帧
                    yield return null;
                }  
            }


            //复制sbf
            FileUtil.WriteToPersistentData("Res/sbf", wwwInner.bytes);
            yield break;
        }
    }

}

#endif