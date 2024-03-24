using BaseClasses;
using PlayerControllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class HealModule : AbstractModul
{
    [SerializeField] private float maxHeal = 100;
    [SerializeField] private float currentHeal = 0;

    [SerializeField] private float healPrecent = 100;

    private float lightDamagePrecentValue = 5f;
    private float hardDamagePrecentValue = 10;

    [Header("")]
    [SerializeField] private float vingetteStartHealPrecent = 50f;
    [SerializeField] private float vignetteMaxIntensity = 0.6f;

    [Header("")]
    public UnityEvent _die = new UnityEvent();

    private void Start()
    {
        currentHeal = maxHeal;
        UpdatePrecentValue();
        //загрузка из системы сохранения
    }

    public void SetDamage(float _damage)
    {
        if (currentHeal <= 0) return;

        //запрос в систему блока, есть ли блок, если есть то режем урон, если нет, то передаем весь урон
        currentHeal -= _damage;
        UpdatePrecentValue();

        if (currentHeal <= 0)
            Die();
    }

    void Die()
    {
        currentHeal = 0;
        UpdatePrecentValue();

        _die.Invoke();
    }

    void UpdatePrecentValue() 
    {
        float lastPrecent = healPrecent;

        healPrecent = (currentHeal * 100) / maxHeal;

        //проверяем процент полученного урона, если больше определенных порогов, то играем анимацию
        float difference = lastPrecent - healPrecent;
        if (difference > 0)
        {
            if (difference > hardDamagePrecentValue)
                _playerData.PlayerAnimator.SetTrigger("HDamage");
            else
                if (difference > lightDamagePrecentValue)
                    _playerData.PlayerAnimator.SetTrigger("LDamage");
        }

        if (healPrecent < vingetteStartHealPrecent)
        {
            float vengettePrecent = healPrecent * 100 / vingetteStartHealPrecent; 
            
            if (vengettePrecent > (100 - vignetteMaxIntensity * 100))
                LevelManager.PostProcessManager.VignetteActivate(true, (100 - vengettePrecent) * 0.01f);
        }
        else
            LevelManager.PostProcessManager.VignetteActivate(false);
    }

    public float GetCurrentHeal()
    {
        return currentHeal;
    }

    public float GetHealPrecent()
    {
        return healPrecent;
    }

    public override void SetModuleActivityType(bool _modulIsActive)
    {
        base.SetModuleActivityType(_modulIsActive);
    }
}
