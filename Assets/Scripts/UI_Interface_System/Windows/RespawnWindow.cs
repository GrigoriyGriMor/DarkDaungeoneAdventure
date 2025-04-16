using PlayerControllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnWindow : AbstractWindow
{
    [SerializeField] private Button _respawnBtn;
    [SerializeField] private Button _exitToMainMenuBtn;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _respawnBtn.onClick.AddListener(() =>
        {
            CloseWindow();
            FindAnyObjectByType<PlayerController>().PlayerRespawn();
        });

        _exitToMainMenuBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}