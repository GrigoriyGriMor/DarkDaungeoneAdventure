using UnityEngine;

public class GameManager : SingeltonManager<GameManager>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (_localizationManager == null) Debug.LogError("GameManager havn't link to _localizationManager. You need Add this link to stability work system", gameObject);
    }

    [SerializeField]
    private LocalizationManager _localizationManager = null;

    static public LocalizationManager LocalizationManager { get => Instance.GetManager(Instance._localizationManager); }
}
