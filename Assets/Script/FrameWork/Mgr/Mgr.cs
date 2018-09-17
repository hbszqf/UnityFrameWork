using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FW
{
    public class Mgr 
    {
        GameObject gameObject;
        internal static Mgr _inst;
        public static Mgr inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new Mgr();
                }

                return _inst;
            }

            protected set
            {
                _inst = value;
            }
        }

        protected Dictionary<Type, BaseMgr> mgrMap = new Dictionary<Type, BaseMgr>();

        public Mgr()
        {
            _inst = this;
            this.gameObject = new GameObject();
            this.gameObject.name = "Mgr";
            GameObject.DontDestroyOnLoad(gameObject);
            //gameObject.AddComponent<MonoBehaviour>();
        }

        public T GetMgr<T>() where T:BaseMgr
        {
            T t = this.gameObject.GetComponent<T>();
            if (t == null) {
                this.gameObject.AddComponent<T>();
            }
            return t;
        }

        public T AddMgr<T>() where T : BaseMgr
        {
            T t = this.gameObject.GetComponent<T>();
            if (t == null)
            {
                t = this.gameObject.AddComponent<T>();
            }
            return t;
        }
    }
}
