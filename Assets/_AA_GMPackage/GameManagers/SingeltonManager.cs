using UnityEngine;

namespace SupportSystems
{
    public class SingeltonManager<S> : MonoBehaviour where S : SingeltonManager<S>
    {
        private static S instance;
        public static S Instance { get { return instance; } }

        public static bool IsInstantiated { get { return instance != null; } }

        public virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("[Singleton] Объект этого типа уже существует.", gameObject);
                Destroy(gameObject);
            }
            else
                instance = (S)this;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        protected virtual T GetManager<T>(T manager) where T : class
        {
            if (manager != null)
                return manager;
            
            Debug.LogError("[Singleton] Менеджер не определён.", gameObject);
            return FindObjectOfType(typeof(T), false) as T;
        }
    }
}