using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FW
{
    public class NetMgr : BaseMgr
    {

        Dictionary<int, NetProxy> socketClientProxyDict = new Dictionary<int, NetProxy>();
        public NetProxy GetNetProxy(int slot)
        {
            if (!socketClientProxyDict.ContainsKey(slot))
            {
                var proxy = new NetProxy(slot);
                socketClientProxyDict.Add(slot, proxy);
            }
            return socketClientProxyDict[slot];
        }


        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            foreach (var item in socketClientProxyDict)
            {
                item.Value.Update();
            }

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            foreach (var item in socketClientProxyDict)
            {
                item.Value.OnDestroy();
            }
        }

        private void OnApplicationQuit()
        {
            foreach (var item in socketClientProxyDict)
            {
                item.Value.OnApplicationQuit();
            }
        }
    }
}

