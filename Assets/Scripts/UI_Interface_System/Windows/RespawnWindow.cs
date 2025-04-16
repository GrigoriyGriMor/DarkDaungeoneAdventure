using PlayerControllers;
using UnityEngine;
using UnityEngine.UI;

public class RespawnWindow : AbstractWindow
{
    [SerializeField] private Button _respawnBtn;
    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _respawnBtn.onClick.AddListener(() =>
        {
            CloseWindow();
            GameObject.FindAnyObjectByType<PlayerController>().PlayerRespawn();
        });
    }
}