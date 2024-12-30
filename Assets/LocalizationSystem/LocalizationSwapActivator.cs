using Game.Core;
using System.Collections;
using TMPro;
using UnityEngine;

public class LocalizationSwapActivator : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropDownBtn;

    private LocalizationManager _localizationManager;

    IEnumerator Start()
    {
        if (_dropDownBtn == null)
            _dropDownBtn = GetComponent<TMP_Dropdown>();

        while (!GameManager.Instance.GetManager<LocalizationManager>().GetLoadingLocalizationStatus())
            yield return new WaitForFixedUpdate();

        _localizationManager = GameManager.Instance.GetManager<LocalizationManager>();

        Init();
    }

    public void Init()
    {
        string[] localizationTypes = _localizationManager.GetAllLocalizationTypes();
        string currentType = _localizationManager.GetCurrentLocalizationType();

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
        _localizationManager.UpdateLocalizationVersion(_dropDownBtn.options[typeNumber].text);
    }

    private void OnDestroy()
    {
        _dropDownBtn.onValueChanged.RemoveAllListeners();
    }
}
