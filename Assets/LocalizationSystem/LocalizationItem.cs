using System.Collections;
using TMPro;
using UnityEngine;

public class LocalizationItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string textID = "baseText";
    [HideInInspector] public string TextID { get => textID; }

    private IEnumerator Start()
    {
        while (!GameManager.LocalizationManager.GetLoadingLocalizationStatus())
            yield return new WaitForFixedUpdate();

        GameManager.LocalizationManager.InitItem(this);
    }

    public void SetLocalizationText(string _text)
    {
        text.text = _text;
    }
}
