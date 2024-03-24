using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public sealed class ScriptableSingletonsStorage : MonoBehaviour
    {

        private static ScriptableSingletonsStorage _instance;
        public static ScriptableSingletonsStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ScriptableSingletonsStorage>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        [SerializeField]
        private List<ScriptableObject> _singletons;

        public List<ScriptableObject> GetSingletons() { return _singletons; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(_instance);
            }
            else if (_instance != this)
            {
                _instance._singletons.Clear();
                _instance._singletons.AddRange(_singletons);
                Destroy(this);
            }
        }

        public T GetInstance<T>() where T : ScriptableObject
        {
            for (int i = 0; i < _singletons.Count; i++)
            {
                if (_singletons[i] is T)
                {
                    return _singletons[i] as T;
                }
            }
            //Logger.LogErrorFormat("Singleton {0} is not in storage!", typeof(T).ToString());
            return null;
        }
    }
}