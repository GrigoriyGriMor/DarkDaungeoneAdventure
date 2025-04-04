using UnityEngine;

public class StaminaBarController : MonoBehaviour
{
    [SerializeField] private RectTransform _bar;
    [SerializeField] private Animator _animator;

    [Header(""), SerializeField]
    private float _criticalStaminaPointPrecent = 30;

    [Header("Animation Param")]
    [SerializeField] private string _critHealActivatorBoolParam = "LittleStamina";
    [SerializeField] private string _staminaUpActivatorTriggerParam = "StaminaUp";

    private float _maxBarRectWight;

    private void Start()
    {
        _maxBarRectWight = _bar.sizeDelta.x;
    }

    public void UpdateHealValue(float staminaPointsPrecent, bool staminaUp = false)
    {
        float sizeX = (staminaPointsPrecent * _maxBarRectWight / 100);
        _bar.sizeDelta = new Vector2(sizeX > _maxBarRectWight ? _maxBarRectWight : sizeX, _bar.sizeDelta.y);

        if (staminaUp)
            _animator.SetTrigger(_staminaUpActivatorTriggerParam);

        if (staminaPointsPrecent <= _criticalStaminaPointPrecent)
            _animator.SetBool(_critHealActivatorBoolParam, true);
        else
            _animator.SetBool(_critHealActivatorBoolParam, false);
    }
}
