using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBarController : MonoBehaviour
{
    [SerializeField] private RectTransform _bar;
    [SerializeField] private Animator _animator;

    [Header(""), SerializeField]
    private float _criticalHealPointPrecent = 30;

    [Header("Animation Param")]
    [SerializeField] private string _critHealActivatorBoolParam = "LittleHeal";
    [SerializeField] private string _healUpActivatorTriggerParam = "HealUp";

    private float _maxBarRectWight;

    private void Start()
    {
        _maxBarRectWight = _bar.sizeDelta.x;
    }

    public void UpdateHealValue(float healPointsPrecent, bool healUp = false)
    {
        float sizeX = (healPointsPrecent * _maxBarRectWight / 100);
        _bar.sizeDelta = new Vector2(sizeX > _maxBarRectWight ? _maxBarRectWight : sizeX, _bar.sizeDelta.y);

        if (healUp)
            _animator.SetTrigger(_healUpActivatorTriggerParam);

        if (healPointsPrecent <= _criticalHealPointPrecent)
            _animator.SetBool(_critHealActivatorBoolParam, true);
        else
            _animator.SetBool(_critHealActivatorBoolParam, false);
    }
}
