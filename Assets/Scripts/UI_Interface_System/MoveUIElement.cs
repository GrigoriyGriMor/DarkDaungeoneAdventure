using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class MoveUIElement : WinEffect
{
    [SerializeField] private RectTransform _rootPanel;

    Vector2 startPos = Vector2.zero;
    Vector2 endPos = Vector2.zero;

    [SerializeField] UIMoveType moveType = UIMoveType.Left;
    [SerializeField] private float _moveSpeed = 1f;

    Coroutine _moveCoroutine;

    private void Start()
    {
        if (_rootPanel == null)
            _rootPanel = GetComponent<RectTransform>();
    }

    public void SetRoot(RectTransform root)
    {
        _rootPanel = root;
    }

    public override void MoveUI(UIMoveType moveType, bool open)
    {
        if (_moveCoroutine != null)
            return;

        switch (moveType)
        {
            case UIMoveType.Up:
                startPos = new Vector2(0, open ? -_rootPanel.rect.size.y : 0);
                endPos = new Vector2(0, open ? 0 : -_rootPanel.rect.size.y);
                break;
            case UIMoveType.Down:
                startPos = new Vector2(0, open ? _rootPanel.rect.size.y : 0);
                endPos = new Vector2(0, open ? 0 : _rootPanel.rect.size.y); 
                break;
            case UIMoveType.Left:
                startPos = new Vector2(open ? _rootPanel.rect.size.x : 0, 0);
                endPos = new Vector2(open ? 0 : _rootPanel.rect.size.x, 0);
                break;
            case UIMoveType.Right:
                startPos = new Vector2(open ? -_rootPanel.rect.size.x : 0, 0);
                endPos = new Vector2(open ? 0 : -_rootPanel.rect.size.x, 0);
                break;
            default:
                startPos = new Vector2(0, open ? _rootPanel.rect.size.x : 0);
                endPos = new Vector2(0, open ? 0 : -_rootPanel.rect.size.y);
                break;
        }

        _moveCoroutine = StartCoroutine(MoveUICoroutine(moveType));
    }

    IEnumerator MoveUICoroutine(UIMoveType moveType)
    {
        _rootPanel.localPosition = startPos;

        while (Vector2.Distance(_rootPanel.localPosition, endPos) > 0.02f)
        {
            yield return new WaitForFixedUpdate();
            _rootPanel.localPosition = Vector2.Lerp(_rootPanel.localPosition, endPos, _moveSpeed);
        }

        _rootPanel.localPosition = endPos;
        _moveCoroutine = null;
    }

    [SerializeField] bool debugStart = true;

    [ContextMenu("DebugStart")]
    void DebugStart()
    {
        MoveUI(moveType, debugStart);
    }
}

[Serializable]
public enum UIMoveType
{
    None,
    Right,
    Left,
    Up,
    Down,
}
