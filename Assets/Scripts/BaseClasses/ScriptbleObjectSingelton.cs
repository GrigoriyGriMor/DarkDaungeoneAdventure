using System.Linq;
using UnityEngine;

namespace Config
{
    public class ScriptbleObjectSingelton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.isEditor)
                    {
                        _instance = Resources.LoadAll<T>("").First();
                    }
                    else
                    {
                        _instance = ScriptableSingletonsStorage.Instance.GetInstance<T>();
                    }
                }
                return _instance;
            }
        }
    }
}