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

    [HideInInspector] public SupportClasses.WindowName ParentWindow { get => _parentWindow; set => _parentWindow = value; }
    private SupportClasses.WindowName _parentWindow;

    [Header("Notification")]
    [SerializeField] private bool _isNotification = false;

    public virtual void Start()
    {
        if (!_winAnim)
            _winAnim = GetComponent<Animator>();
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

    public virtual async void CloseWindow()
    {
        if (_winAnim)
        {
            _winAnim.SetTrigger("Deactive");
            await Task.Delay(_waitAnimTime);
        } 

        gameObject.SetActive(false);

        if (_parentWindow != SupportClasses.WindowName.None)
            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(_parentWindow);

        Destroy(gameObject);
    }

    IEnumerator NotificationTimeout()
    {
        yield return new WaitForSeconds(3);
        CloseWindow();
    }
}
