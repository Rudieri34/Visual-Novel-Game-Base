using UnityEngine;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System;

namespace InnominatumDigital.Base
{
    public class SingletonBase<T> : MonoBehaviour
    where T : Component
    {
        private static object _lock = new object();

        private static T _instance;
        public static T Instance
        {
            get { return GetInstance(); }
        }

        public static T GetInstance()
        {
            if (applicationIsQuitting)
                return null;

            return _instance;
        }

        //Token
        private CancellationTokenSource _sourceToken;
        protected CancellationToken CancellationToken { get; private set; }

        protected virtual void Awake()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this as T;
                    DontDestroyOnLoad(gameObject);

                }
                else
                {
                    gameObject.transform.SetParent(null);
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }

            _sourceToken = new CancellationTokenSource();
            CancellationToken = _sourceToken.Token;

        }

        public virtual void Init(){
            
        }

        private static bool applicationIsQuitting = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;

            if (_sourceToken != null)
                _sourceToken.Cancel();
        }
    }
}
