using System.Collections;
using TMPro;
using UnityEngine;

public class LocalizationSwapActivator : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropDownBtn;

    IEnumerator Start()
    {
        if (_dropDownBtn == null)
            _dropDownBtn = GetComponent<TMP_Dropdown>();

        while (!GameManager.LocalizationManager.GetLoadingLocalizationStatus())
            yield return new WaitForFixedUpdate();

        Init();
    }

    public void Init()
    {
        string[] localizationTypes = GameManager.LocalizationManager.GetAllLocalizationTypes();
        string currentType = GameManager.LocalizationManager.GetCurrentLocalizationType();

        _dropDownBtn.options.Clear();

        for (int i = 0; i < localizationTypes.Length; i++) 
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();

            newOption.text = localizationTypes[i];
            _dropDownBtn.options.Add(newOption);

            if (newOption.text == currentType)
            {
                _dropDownBtn.captionText.text = currentType;
                _dropDownBtn.value = i;
            }
        }

        _dropDownBtn.onValueChanged.AddListener((value) => UpdateCurrentType(value));
    }

    void UpdateCurrentType(int typeNumber)
    {
        GameManager.LocalizationManager.UpdateLocalizationVersion(_dropDownBtn.options[typeNumber].text);
    }

    private void OnDestroy()
    {
        _dropDownBtn.onValueChanged.RemoveAllListeners();
    }
}
