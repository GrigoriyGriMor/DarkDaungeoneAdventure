using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : AbstractWindow
{
    [SerializeField] private Button _closeBtn;

    public override void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            CloseWindow();
        });
    }
}
