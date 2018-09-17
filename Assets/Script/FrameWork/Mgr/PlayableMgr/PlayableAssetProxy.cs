using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine.Timeline;

namespace FW
{
    public class PlayableAssetProxy 
    {

        public class ReferenceBind
        {
            public PropertyName key;
            public string name;
        }

        public class GenericBind
        {
            public Object key;
            public string name;
        }

        public PlayableAsset playableAsset;

        public bool isReady = false;

        public System.Action<PlayableAssetProxy> readyListener;

        public List<GenericBind> genericBindList = new List<GenericBind>();
        public List<ReferenceBind> referenceBindList = new List<ReferenceBind>();

        public PlayableAssetProxy(string path)
        {
            ResMgr.inst.LoadAsset(path, this.OnLoad);
        }

        private void OnLoad(Object uo)
        {

            var asset = uo as TimelineAsset;
            if (asset != null)
            {
                foreach (var ouput in asset.outputs)
                {
                    var bind = new GenericBind()
                    {
                        key = ouput.sourceObject,
                        name = ouput.streamName,
                    };
                    genericBindList.Add(bind);
                }

                foreach (var track in asset.GetOutputTracks())
                {
                    var control = track as ControlTrack;
                    if (control == null) continue;
                    foreach (var clip in control.GetClips())
                    {
                        var controlAsset = clip.asset as ControlPlayableAsset;
                        if (controlAsset == null) continue;
                        var bind = new ReferenceBind()
                        {
                            key = controlAsset.sourceGameObject.exposedName,
                            name = clip.displayName,
                        };
                        this.referenceBindList.Add(bind);
                    }
                }
            }

            this.playableAsset = asset;
            this.isReady = true;
            if (this.readyListener != null)
            {
                this.readyListener(this);
            }
            
            this.readyListener = null;


        }

    }

}
