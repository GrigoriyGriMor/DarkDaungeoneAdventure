using System.Collections;
using UnityEngine;

public class HealBarController : MonoBehaviour
{
    [SerializeField] private RectTransform _bar;
    [SerializeField] private Animator _animator;

    [SerializeField] private RectTransform _barEffectVisual;
    [SerializeField] private float _barEffectMoveTime = 1f;
    private Coroutine _barEffectCoroutine;

    [Header(""), SerializeField]
    private float _criticalHealPointPrecent = 30;

    [Header("Animation Param")]
    [SerializeField] private string _critHealActivatorBoolParam = "LittleHeal";
    [SerializeField] private string _healUpActivatorTriggerParam = "HealUp";

    private float _maxBarRectWight;

    bool _isReady = false;
    public bool IsReady => _isReady;

    private void Start()
    {
        _maxBarRectWight = _bar.sizeDelta.x;
        _barEffectVisual.sizeDelta = new Vector2(_bar.sizeDelta.x, _bar.sizeDelta.y); ;

        _isReady = true;
    }

    public void UpdateHealValue(float healPointsPrecent, bool healUp = false)
    {
        _barEffectVisual.sizeDelta = new Vector2(_bar.sizeDelta.x, _bar.sizeDelta.y); ;

        float sizeX = (healPointsPrecent * _maxBarRectWight / 100);
        _bar.sizeDelta = new Vector2(sizeX > _maxBarRectWight ? _maxBarRectWight : sizeX, _bar.sizeDelta.y);

        if (healUp)
            _animator.SetTrigger(_healUpActivatorTriggerParam);

        if (healPointsPrecent <= _criticalHealPointPrecent)
            _animator.SetBool(_critHealActivatorBoolParam, true);
        else
            _animator.SetBool(_critHealActivatorBoolParam, false);

        if (_barEffectCoroutine != null)
            StopCoroutine(_barEffectCoroutine);

        if (gameObject.activeInHierarchy) 
            _barEffectCoroutine = StartCoroutine(BarEffectCoroutine());
    }

    private IEnumerator BarEffectCoroutine()
    {
        if (_barEffectVisual.sizeDelta.x > _bar.sizeDelta.x)
        { 
            float difference = _barEffectVisual.sizeDelta.x - _bar.sizeDelta.x;
            difference = difference / 5;

            float timer = 0;
            float value = 0;
            while (timer < _barEffectMoveTime)
            {
                timer += Time.deltaTime;
                value = Mathf.Lerp(_barEffectVisual.sizeDelta.x, _bar.sizeDelta.x, difference * Time.deltaTime);
                _barEffectVisual.sizeDelta = new Vector2(value, _barEffectVisual.sizeDelta.y); ;
                yield return null;
            }
        }

        _barEffectVisual.sizeDelta = _bar.sizeDelta;
        _barEffectCoroutine = null;
    }
}
