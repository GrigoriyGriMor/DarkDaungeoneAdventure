using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private List<LocalizationItem> itemList = new List<LocalizationItem>();
    bool isReady = false;


    public bool GetLoadingLocalizationStatus()
    {
        return isReady;
    }

    public void InitItem(LocalizationItem _item)
    {
        foreach (LocalizationItem item in itemList)
        {
            if (item.TextID == _item.TextID)
                return;
        }

        itemList.Add(_item);

        _item.SetLocalizationText("крутой текст"/*get localization text in localization file*/);
    }
}