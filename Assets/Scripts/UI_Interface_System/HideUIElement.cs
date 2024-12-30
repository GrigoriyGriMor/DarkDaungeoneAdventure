using System.Collections;
using UnityEngine;

public class HideUIElement : WinEffect
{
    [SerializeField] private CanvasGroup _canvasGroupPanel;
    [SerializeField] private float _moveSpeed = 1f;

    Coroutine _moveCoroutine;

    [SerializeField] bool debugStart = true;

    private void Start()
    {
        if (_canvasGroupPanel == null)
            _canvasGroupPanel = GetComponent<CanvasGroup>();
    }
     
    public void SetRoot(CanvasGroup root)
    {
        _canvasGroupPanel = root;
    }

    public override void OutHideUI(bool open)
    {
        if (_moveCoroutine != null)
            return;

        _moveCoroutine = StartCoroutine(OutHideUICoroutine(open));
    }

    IEnumerator OutHideUICoroutine(bool open)
    {
        int target = open ? 1 : 0;

        while (Mathf.Abs(_canvasGroupPanel.alpha - target) > 0.02f)
        {
            yield return new WaitForFixedUpdate();
            _canvasGroupPanel.alpha = Mathf.Lerp(_canvasGroupPanel.alpha, target, _moveSpeed);
        }

        _canvasGroupPanel.alpha = target;
        _moveCoroutine = null;
    }

    [ContextMenu("DebugStart")]
    void DebugStart()
    {
        OutHideUI(debugStart);
    }
}
