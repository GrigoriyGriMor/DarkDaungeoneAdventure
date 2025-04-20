using Game.Core;
using PlayerControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBackToMenu : MonoBehaviour
{
    [SerializeField] private SupportClasses.WindowName windowToOpen;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            StartCoroutine(IShowWindow());
         }
    }

    private IEnumerator IShowWindow()
    {
        GameManager.Instance.GetManager<WindowsManager>().OpenWindow(windowToOpen, SupportClasses.WindowName.InGameHUD);
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
    }
}
