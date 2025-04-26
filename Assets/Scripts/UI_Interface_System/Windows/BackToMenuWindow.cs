using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenuWindow : AbstractWindow
{
    [SerializeField] private Button _exitToMainMenuBtn;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _exitToMainMenuBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}