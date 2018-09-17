using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

namespace FW
{
    public class PlayableProxy : MonoBehaviour
    {
        public PlayableControllerAsset asset;
        private PlayableDirector director;//

        private float startPlayTime = 0;

        private bool loop = false;

        private void Awake()
        {
            this.director = this.gameObject.GetComponent<PlayableDirector>();

            if (this.director == null)
            {
                this.director = this.gameObject.AddComponent<PlayableDirector>();
            }

            this.director.playableAsset = null;
            this.director.timeUpdateMode = DirectorUpdateMode.GameTime;


        }

        public void Play(string action, bool loop)
        {
            if (this.asset == null)
            {
                return;
            }

            string path = null;
            foreach (var item in this.asset.list)
            {
                if (action == item.name)
                {
                    path = item.path;
                }
            }

            if (path == null)
            {
                return;
            }

            this.startPlayTime = Time.time;
            this.loop = loop;

            PlayableAssetProxy proxy = PlayableMgr.inst.GetPlayableAssetProxy(path);

            proxy.readyListener -= PlayAssetProxy;
            if (proxy.isReady)
            {
                this.PlayAssetProxy(proxy);
            }
            else
            {
                proxy.readyListener += PlayAssetProxy;
            }
        }

        private void PlayAssetProxy(PlayableAssetProxy assetProxy)
        {
            if (this == null)
            {
                return;
            }

            //播放资源
            this.director.playableAsset = assetProxy.playableAsset;

            //绑定
            foreach (var genericBind in assetProxy.genericBindList)
            {
                this.director.SetGenericBinding(genericBind.key, this.GetBindGameObject(genericBind.name));
            }
            foreach (var refereneceBind in assetProxy.referenceBindList)
            {
                this.director.SetReferenceValue(refereneceBind.key, this.GetBindGameObject(refereneceBind.name));
            }

            //播放模式
            this.director.extrapolationMode = this.loop ? DirectorWrapMode.Loop : DirectorWrapMode.Hold;
            //开始播放
            this.director.Play();
            //指定播放时间
            this.director.time = Time.time - startPlayTime;
        }

        private void PlayAssetInner(PlayableAsset asset)
        {
 
        }

        private GameObject GetBindGameObject(string name)
        {
            Transform tran = this.transform.Find(name);
            if (tran == null)
            {
                return this.gameObject;
            }
            return tran.gameObject;
        }

    }



}
