using BaseClasses;
using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIHealModule : AbstractAIModule
{
    [SerializeField] private float maxHeal = 100;
    private float currentHeal = 0;

    private float healPrecent = 100;

    private float lightDamagePrecentValue = 5f;
    private float hardDamagePrecentValue = 10f;

    [Header("Heal Bar Controller"), SerializeField]
    private HealBarController _healBarController;

    [Header("")]
    public UnityEvent _die = new UnityEvent();

    private IEnumerator Start()
    {
        currentHeal = maxHeal;

        while (!_healBarController)
        {
            GameManager.Instance.GetManager<WindowsManager>().GetCurrentWindowIfType(SupportClasses.WindowName.InGameHUD);
            yield return new WaitForFixedUpdate();
        }

        UpdatePrecentValue();
        //загрузка из системы сохранения
    }

    public void SetDamage(float _damage)
    {
        if (currentHeal <= 0) return;

        //запрос в систему блока, есть ли блок, если есть то режем урон, если нет, то передаем весь урон
        currentHeal -= _damage;

        if (currentHeal > maxHeal)
            currentHeal = maxHeal;

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
                _aiParametrs.Animator.SetTrigger("HDamage");
            else
                if (difference > lightDamagePrecentValue)
                _aiParametrs.Animator.SetTrigger("LDamage");

            _healBarController.UpdateHealValue(healPrecent);
        }
        else
            _healBarController.UpdateHealValue(healPrecent, true);
    }

    public float GetCurrentHeal()
    {
        return currentHeal;
    }

    public float GetHealPrecent()
    {
        return healPrecent;
    }
}
