using System.Collections;
using TMPro;
using UnityEngine;

public class LocalizationItem : MonoBehaviour
{
    [SerializeField] private string textID = "none";

    private TMP_Text text;
    [HideInInspector] public string TextID { get => textID; }

    private IEnumerator Start()
    {
        text = GetComponent<TMP_Text>();
        if (text == null)
            Debug.LogError($"Localization Error: gameObjct: {gameObject.name}, text with ID {TextID} don't have a component TMP_Text.", gameObject);

        while (!GameManager.LocalizationManager.GetLoadingLocalizationStatus())
            yield return new WaitForFixedUpdate();

        GameManager.LocalizationManager.InitItem(this);
    }

    public void SetLocalizationText(string _text)
    {
        text.text = _text;
    }
}
