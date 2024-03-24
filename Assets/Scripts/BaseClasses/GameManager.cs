using UnityEngine;

public class GameManager : SingeltonManager<GameManager>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);       
    }

    [SerializeField]
    private LocalizationManager _localizationManager = null;

    static public LocalizationManager LocalizationManager { get => Instance.GetManager(Instance._localizationManager); }
}
