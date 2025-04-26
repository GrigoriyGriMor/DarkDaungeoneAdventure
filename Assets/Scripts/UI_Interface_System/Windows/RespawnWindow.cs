using PlayerControllers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnWindow : AbstractWindow
{
    [SerializeField] private Button _respawnBtn;
    [SerializeField] private Button _exitToMainMenuBtn;

    bool _respawnInit = false;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _respawnBtn.onClick.AddListener(() =>
        {
            if (!_respawnInit)
                InitRespawnPlayer();
        });

        _exitToMainMenuBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    async void InitRespawnPlayer()
    {
        _respawnInit = true;

        await Task.Delay(500);
        FindAnyObjectByType<PlayerController>().PlayerRespawn();
        CloseWindow();
        _respawnInit = false;
    }
}