using UnityEngine;

public class SingeltonManager<S> : MonoBehaviour where S : SingeltonManager<S>
{
    private static S instance;
    public static S Instance { get => instance; }

    public static bool IsInstantiated { get => instance != null; }

    protected virtual void Awake()
    {
        if (instance != null)
            Debug.LogWarning("[Singleton] Trying to instantiate a second instance of singleton class.");
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
        else
            return FindObjectOfType(typeof(T), false) as T;
    }
}
