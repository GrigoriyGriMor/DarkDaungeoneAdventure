using PlayerControllers;
using UnityEngine;
using UnityEngine.Events;

public class HealModule : AbstractModul
{
    [SerializeField] private float maxHeal = 100;
    [SerializeField] private float currentHeal = 0;

    [SerializeField] private float healPrecent = 100;

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
        healPrecent = (currentHeal * 100) / maxHeal;
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
