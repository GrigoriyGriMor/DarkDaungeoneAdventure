using Game.Core;
using PlayerControllers;
using UnityEngine;

public class OpenBackToMenu : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            Time.timeScale = 0;
            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(SupportClasses.WindowName.BackToMenuMenu, SupportClasses.WindowName.InGameHUD);
        }
    }
}
