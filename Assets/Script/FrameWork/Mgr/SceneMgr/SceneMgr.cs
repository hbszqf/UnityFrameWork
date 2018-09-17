using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace FW
{
    public class SceneMgr : BaseMgr
    {


        const string EMPTY_SCENE = "Assets/_Scenes/Empty.unity";

        /// <summary>
        /// 是否忙
        /// </summary>
        private bool isBusy = false;

        /// <summary>
        /// 当前加载进度
        /// </summary>
        private AsyncOperation syncOperator = null;

        /// <summary>
        /// 当前场景名字
        /// </summary>
        private string sceneName = null;

        /// <summary>
        /// 单例
        /// </summary>
        private static SceneMgr _inst = null;
        public static SceneMgr inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = Mgr.inst.AddMgr<SceneMgr>();
                }
                return _inst;
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="path">“相对于与 Assets/Res”</param>
        /// <param name="completeCallback"></param>
        public void LoadScene(string path, Action<bool> completeCallback)
        {

            string assetPath = "Assets/Res/" + path;
            LoadSceneInner(assetPath, completeCallback);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="completeCallback"></param>
        public void UnloadScene(Action<bool> completeCallback = null)
        {
            LoadSceneInner(EMPTY_SCENE, completeCallback);
        }

        /// <summary>
        /// 加载一个场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="progressUpdate"></param>
        /// <param name="completeCallback"></param>
        private void LoadSceneInner(string assetPath, Action<bool> completeCallback = null)
        {
            StartCoroutine(LoadSceneCor(assetPath, completeCallback));
        }

        /// <summary>
        /// 开启一个协程来加载场景
        /// </summary>
        private IEnumerator LoadSceneCor(string newSceneName, Action<bool> completeCallback = null)
        {
            //是否空闲
            if (isBusy)
            {
                completeCallback(false);
                yield break;
            }

            //当前场景一样 直接加载完成 (大小写不敏感)
            if (this.sceneName == newSceneName)
            {
                completeCallback(true);
                yield break;
            }

            //旧场景的名字
            string sceneName_pre = this.sceneName;
            //场景加载完成
            this.sceneName = null;

            //标记为忙
            isBusy = true;

            syncOperator = null;

            //加载Load 卸载场景
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EMPTY_SCENE);

            //如果是Load界面直接加载完成
            if (newSceneName == EMPTY_SCENE)
            {
                isBusy = false;
                completeCallback(true);
                yield break;
            }

            //加载前的准备工作
            yield return StartCoroutine(PrepareLoad(sceneName_pre, newSceneName));

            //异步加载
            syncOperator = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newSceneName);

            //加载失败
            if (syncOperator == null)
            {
                isBusy = false;
                completeCallback(false);
                yield break;
            }

            //等待加载完成
            yield return syncOperator;

            //加载完成
            this.sceneName = newSceneName; //设置当前场景名字
            isBusy = false;
            completeCallback(true);

            yield break;
        }


        IEnumerator PrepareLoad(string sceneName_pre, string sceneName)
        {
#if UNITY_EDITOR && !AB_MODE
#else

            var resMgr = ResMgr.inst;

            //清除ab
            if (sceneName_pre != null && sceneName_pre != "" && sceneName_pre != EMPTY_SCENE)
            {
                string abName_pre = ResHelper.GetABName(sceneName);
                ABProxy abProxy = resMgr.GetABProxy(abName_pre);
                if (abProxy != null)
                {
                    //清除ab
                    abProxy.isResident = false;
                }
            }

            //加载ab
            if (sceneName != null && sceneName != "" && sceneName != EMPTY_SCENE)
            {
                string abName = ResHelper.GetABName(sceneName);
                bool abfinish = false;
                resMgr.LoadABProxy(abName, (abProxy)=>{
                    abProxy.isResident = true;
                    abfinish = true;
                });
        
                while (true)
                {
                    if (abfinish)
                    {
                        break;
                    }
                    yield return null;
                }
            }

            //清理资源
            bool finish = false;
            ResMgr.inst.UnloadUnused(() => {
                finish = true;
            });
            while (true)
            {
                if (finish == true)
                {
                    break;
                }
                yield return null;
            }
#endif
            yield break;

        }
    }

}
