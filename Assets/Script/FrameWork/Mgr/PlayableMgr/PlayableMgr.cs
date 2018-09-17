using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace FW
{
    public class PlayableMgr : BaseMgr
    {
        private static PlayableMgr _inst = null;
        public static PlayableMgr inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = Mgr._inst.AddMgr<PlayableMgr>();
                }
                return _inst;
            }
        }


        public Dictionary<string, PlayableAssetProxy> playableProxyDict = new Dictionary<string, PlayableAssetProxy>();

        public PlayableAssetProxy GetPlayableAssetProxy(string path)
        {
            
            if (!playableProxyDict.ContainsKey(path))
            {
                var assetProxy = new PlayableAssetProxy(path);
                playableProxyDict.Add(path, assetProxy);
            }
            return playableProxyDict[path];
        }
    }

}
