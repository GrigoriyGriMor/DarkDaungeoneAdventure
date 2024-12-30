using Game.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : AbstractManager
{
    [Header("Localization File Name")]
    [SerializeField] private string _localizationFileName = "LocalizationData.json";
    private LocalizationTranslateDirectory _localizationDirectory = new LocalizationTranslateDirectory();

    private string[] _languageTypes;//специально не определили

    private string _currentLocalizationType = "";

    private List<LocalizationItem> itemList = new List<LocalizationItem>();

    private void Start()
    {
        LocalizationData localizationData = LoadLocalizationAllData();
        if (localizationData == null) 
            return;

        _languageTypes = new string[localizationData.data.Count];
        for (int k = 0; k < localizationData.data.Count; k++)
            _languageTypes[k] = localizationData.data[k].languageType;

        if (PlayerPrefs.HasKey("LanguageType"))
        {
            _currentLocalizationType = PlayerPrefs.GetString("LanguageType");
            for (int i = 0; i < localizationData.data.Count; i++)
            {
                if (localizationData.data[i].languageType == _currentLocalizationType)
                { 
                    _localizationDirectory = localizationData.data[i];
                    break;
                }
            }
        }
        else
        {
            _localizationDirectory = localizationData.data[0];
            _currentLocalizationType = localizationData.data[0].languageType;

            PlayerPrefs.SetString("LanguageType", _currentLocalizationType);
        }

        isReady = true;
    }

    LocalizationData LoadLocalizationAllData()
    {
        LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(File.ReadAllText($"{Application.streamingAssetsPath}/{_localizationFileName}"));
        if (localizationData.data.Count == 0)
        {
            Debug.LogError("Localiztion File " + _localizationFileName + " have null data");
            return null;
        }

        return localizationData;
    }

    #region Gettings
    public string[] GetAllLocalizationTypes()
    {
        return _languageTypes;
    }

    public string GetCurrentLocalizationType()
    {
        return _currentLocalizationType;
    }

    public bool GetLoadingLocalizationStatus()
    {
        return isReady;
    }

    public string GetText(string textID)
    {
        for (int i = 0; i < _localizationDirectory.itemData.Count; i++)
            if (textID == _localizationDirectory.itemData[i].textID)
                return _localizationDirectory.itemData[i].localizationText;

        return "translate missing";
    }
    #endregion

    void OnLevelWasLoaded()
    {
        itemList.Clear();
    }

    public void InitItem(LocalizationItem _item)
    {
        foreach (LocalizationItem item in itemList)
        {
            if (item.TextID == _item.TextID)
                return;
        }

        itemList.Add(_item);

        _item.SetLocalizationText(GetText(_item.TextID));
    }

    public string UpdateLocalizationVersion(string newLocalizationType)
    {
        if (newLocalizationType == _currentLocalizationType) 
            return _currentLocalizationType;

        LocalizationData localizationData = LoadLocalizationAllData();

        if (localizationData != null)
            for (int i = 0; i < localizationData.data.Count; i++)
            {
                if (localizationData.data[i].languageType == newLocalizationType)
                {
                    _localizationDirectory = localizationData.data[i];
                    _currentLocalizationType = localizationData.data[i].languageType;
                    PlayerPrefs.SetString("LanguageType", _currentLocalizationType);

                    foreach (LocalizationItem item in itemList)
                        item.SetLocalizationText(GetText(item.TextID));

                    return _currentLocalizationType;
                }
            }

        return _currentLocalizationType;
    }

}