using Game.Core;
using PlayerControllers;
using System.Collections;
using UnityEngine;

public class OpenBackToMenu : MonoBehaviour
{
    [SerializeField] private SupportClasses.WindowName windowToOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController controller))
        {
            StartCoroutine(IShowWindow(controller));
        }
    }

    private IEnumerator IShowWindow(PlayerController controller)
    {
        GameManager.Instance.GetManager<WindowsManager>().OpenWindow(windowToOpen, SupportClasses.WindowName.InGameHUD);
        yield return new WaitForSeconds(1);
        controller.DeactivateModules();
    }
}
