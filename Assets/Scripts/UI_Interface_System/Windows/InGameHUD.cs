using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : AbstractWindow
{
    [SerializeField] private Button _settingBtn;

    public override void Start()
    {
        base.Start();

        _settingBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(SupportClasses.WindowName.MMSettingPanel, SupportClasses.WindowName.InGameHUD);
            CloseWindow();
        });
    }
}
