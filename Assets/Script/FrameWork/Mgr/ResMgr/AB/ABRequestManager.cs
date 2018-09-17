using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace FW
{ 
    /// <summary>
    /// ABRequ
    /// </summary>
    public class ABRequestManager
    {
        //ab加载完成回调
        public delegate void OnABLoaded(string abName, AssetBundle ab);
        public OnABLoaded onABLoaded;

        /// <summary>
        /// ab的文件夹
        /// </summary>
        private string folder;

        //private ABManager abManager;

        /// <summary>
        /// 当前正在请求的AB
        /// </summary>
        private Dictionary<string, AssetBundleCreateRequest> abRequestDict { get; set; }

        public ABRequestManager(ABMgr abManager)
        {
            this.folder = abManager.folder;
            //this.abManager = abManager;
            abRequestDict = new Dictionary<string, AssetBundleCreateRequest>();
        }

        public void Update()
        {
            //Log.Print("abRequestDict.Count:" + abRequestDict.Count);
            if (abRequestDict.Count == 0)
            {
                return;
            }
            
            //查找已经完成的ab
            List<string> list = null;
            foreach (var item in abRequestDict)
            {
                if (item.Value.isDone)
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(item.Key);
                }
            }

            //处理已完成的AB
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string abName = list[i];
                    var abRequest = abRequestDict[abName];
                    abRequestDict.Remove(abName);
                    //回调
                    onABLoaded(abName, abRequest.assetBundle);                
                }
            }
        }

        public void AddAbRequest(string abName)
        {       
            if (abRequestDict.ContainsKey(abName))
            {
                return;
            }

            string path = Path.Combine(this.folder, abName);
            abRequestDict[abName] = AssetBundle.LoadFromFileAsync(path);
        }
    }


}