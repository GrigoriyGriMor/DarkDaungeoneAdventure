using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationEditor : MonoBehaviour
{
    [Header("Файл с которым работаешь сейчас")]
    [SerializeField] private string workWithFile_Name = "LocalizationData.json";
    [SerializeField] private LocalizationData _localizationData;

    [Header("Перед командой Upload Translate File укажите нужный формат"), SerializeField]
    private string _newLanguageType = "none";

    [Header("Update text ID"), SerializeField]
    private string _lastTextID = "none";
    [SerializeField] private string _newTextID = "none";

    [ContextMenu("Load File Data")]
    public void LoadFileData()
    {
        if (File.Exists($"{Application.streamingAssetsPath}/{workWithFile_Name}"))
        {
            _localizationData = JsonUtility.FromJson<LocalizationData>(File.ReadAllText($"{Application.streamingAssetsPath}/{workWithFile_Name}"));
            Debug.Log("File was loaded");
        }
        else
            Debug.LogError("File not found, maybe workWithFile_Name not actuality");
    }

    [ContextMenu("Save Data")]
    public void SaveFileData()
    {
        if (File.Exists($"{Application.streamingAssetsPath}/{workWithFile_Name}"))
        {
            Debug.Log("Good result");
            File.WriteAllText($"{Application.streamingAssetsPath}/{workWithFile_Name}", JsonUtility.ToJson(_localizationData));
        }
        else
        {
            Debug.Log($"File not found! \n Start creating file: {Application.streamingAssetsPath}/{workWithFile_Name}");

            File.WriteAllText($"{Application.streamingAssetsPath}/{workWithFile_Name}", JsonUtility.ToJson(_localizationData));
            Debug.Log("File created and save new data");
        }
    }

    [ContextMenu("Upload Translate File")]
    public void UploadFileToTranslite()
    {
        if (_localizationData.data.Count > 0)
            SaveFileData();
        else
            LoadFileData();

        string tF = _newLanguageType + "|";
        for (int i = 0; i < _localizationData.data[0].itemData.Count; i++)
            tF += $"{_localizationData.data[0].itemData[i].localizationText}\n";

        if (File.Exists(Application.streamingAssetsPath + "TranslateFile.txt"))
            File.Delete(Application.streamingAssetsPath + "TranslateFile.txt");

        File.WriteAllText(Application.streamingAssetsPath + "/TranslateFile.txt", tF);

        _newLanguageType = "none";
        Debug.LogWarning("File WAS UPLOAD... \nTranslate version file and swap him in folder StreamingAssets!!!");
    }

    [ContextMenu("Add Translate File Data")]
    public void AddTranslateFileData()
    {
        string newTranslateData = File.ReadAllText(Application.streamingAssetsPath + "/TranslateFile.txt");
        
        string[] ntdArray = newTranslateData.Split('|');
        string ntdLanguageType = ntdArray[0];
        ntdArray = ntdArray[1].Split('\n');

        LoadFileData();
        if (ntdArray.Length - 1 != _localizationData.data[0].itemData.Count)
        {
            Debug.LogError("string count in Translate Data (" + (ntdArray.Length - 1) + ") != string count in Localization Data (" + _localizationData.data[0].itemData.Count + ").\nCheck your TranslateFile.txt\nImport operation is break ");
            return;
        }

        for (int j = 0; j < _localizationData.data.Count; j++)
        {
            if (_localizationData.data[j].languageType == ntdLanguageType)
            {
                for (int k = 0; k < _localizationData.data[j].itemData.Count; k++)
                    _localizationData.data[j].itemData[k].localizationText = ntdArray[k];

                SaveFileData();
                return;
            }
        }

        LocalizationTranslateDirectory newData = new LocalizationTranslateDirectory();
        newData.languageType = ntdLanguageType;

        for (int i = 0; i < ntdArray.Length - 1; i++)
        {
            LocalizationTextItemData textData = new LocalizationTextItemData();
            textData.textID = _localizationData.data[0].itemData[i].textID;
            textData.localizationText = ntdArray[i];

            newData.itemData.Add(textData);
        }

        _localizationData.data.Add(newData);
        SaveFileData();
    }

    [ContextMenu("UpdateTextID")] 
    public void UpdateTextID()
    {
        LoadFileData();

        for (int i = 0; i < _localizationData.data.Count; i++)
            for (int j = 0; j < _localizationData.data.Count; j++)
                if (_localizationData.data[i].itemData[j].textID == _lastTextID)
                    _localizationData.data[i].itemData[j].textID = _newTextID;

        _lastTextID = "none";
        _newTextID = "none";

        Debug.Log($"Text ID was Upadte. \nLAST TEXT ID - {_lastTextID}\nCURRENT TEXT ID - {_newTextID}");
        Debug.LogWarning("DON'T FORGET SAVE");
    }
}

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationTranslateDirectory> data = new List<LocalizationTranslateDirectory>();
}

[System.Serializable]
public class LocalizationTranslateDirectory
{
    public string languageType = "none_language";
    public List<LocalizationTextItemData> itemData = new List<LocalizationTextItemData>();
}

[System.Serializable]
public class LocalizationTextItemData
{
    public string textID = "none";
    public string localizationText = "";
}
