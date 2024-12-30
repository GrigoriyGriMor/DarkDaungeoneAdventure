using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUIElement : WinEffect
{
    [SerializeField] private RectTransform _rootPanel;
    [SerializeField] private float _moveSpeed = 1f;

    [Space(20)]
    [SerializeField] private float _overScale = 1.1f;

    Coroutine _moveCoroutine;

    [SerializeField] bool debugStart = true;

    private void Start()
    {
        if (_rootPanel == null)
            _rootPanel = GetComponent<RectTransform>();
    }

    public void SetRoot(RectTransform root)
    {
        _rootPanel = root;
    }

    public override void ScaleUI(bool open)
    {
        if (_moveCoroutine != null)
            return;

        _moveCoroutine = StartCoroutine(OutHideUICoroutine(open));
    }

    IEnumerator OutHideUICoroutine(bool open)
    {
        float target = open ? _overScale : 0;

        while (Mathf.Abs(_rootPanel.localScale.x - target) > 0.02f)
        {
            yield return new WaitForFixedUpdate();

            float scale = Mathf.Lerp(_rootPanel.localScale.x, target, _moveSpeed);
            _rootPanel.localScale = new Vector2(scale, scale);
        }

        if (_rootPanel.localScale.x > 1)
        {
            target = 1f;

            while (Mathf.Abs(_rootPanel.localScale.x - target) > 0.02f)
            {
                yield return new WaitForFixedUpdate();

                float scale = Mathf.Lerp(_rootPanel.localScale.x, target, _moveSpeed);
                _rootPanel.localScale = new Vector2(scale, scale);
            }
        }

        _rootPanel.localScale = new Vector2(target, target);
        _moveCoroutine = null;
    }

    [ContextMenu("DebugStart")]
    void DebugStart()
    {
        OutHideUI(debugStart);
    }
}
