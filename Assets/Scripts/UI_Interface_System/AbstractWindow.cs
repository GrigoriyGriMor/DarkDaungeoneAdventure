using Game.Core;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AbstractWindow : MonoBehaviour
{
    [Header("In Animator use -Active- or -Deactive- triggers")]
    [SerializeField] private Animator _winAnim;
    [SerializeField] private int _waitAnimTime = 1000;

    private SupportClasses.WindowName _parentWindow;

    [Header("Notification")]
    [SerializeField] private bool _isNotification = false;

    [HideInInspector] public bool BlockedClosing { get => _blockedClose; }
    [SerializeField] private bool _blockedClose = false;

    public virtual void Init(SupportClasses.WindowName parentWin)
    {
        if (_winAnim == null)
            _winAnim = GetComponent<Animator>();

        _parentWindow = parentWin;
        gameObject.SetActive(true);

        if (_winAnim)
            _winAnim.SetTrigger("Activate");

        if (_isNotification)
            StartCoroutine(NotificationTimeout());
    }

    public virtual void OpenWindow(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    { 
        _parentWindow = parentWin;

        gameObject.SetActive(true);

        if (_winAnim)
            _winAnim.SetTrigger("Activate");

        if (_isNotification)
            StartCoroutine(NotificationTimeout());
    }

    public virtual async Task CloseWindow()
    {
        if (_blockedClose)
            return;

        if (_winAnim)
        {
            _winAnim.SetTrigger("Deactive");
            await Task.Delay(_waitAnimTime);
        }

        gameObject.SetActive(false);

        if (_parentWindow != SupportClasses.WindowName.None)
            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(_parentWindow, SupportClasses.WindowName.None, true);

        Destroy(gameObject);
    }

    public virtual void HardClose()
    {
        Destroy(gameObject);
    }

    IEnumerator NotificationTimeout()
    {
        yield return new WaitForSeconds(3);
        CloseWindow();
    }
}
