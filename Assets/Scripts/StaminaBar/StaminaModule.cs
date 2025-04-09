using Game.Core;
using PlayerControllers;
using System.Collections;
using UnityEngine;

public class StaminaModule : AbstractModul
{
    [SerializeField] private float _maxStamina = 100;
    private float _currentStamina = 0;

    private float _staminaPrecent = 100;

    [Header("Stamina Bar Controller"), SerializeField]
    private StaminaBarController _staminaBarController;

    private bool _minStaminaLevelReady = true;
    [HideInInspector] public bool MinStaminaLevelReady => _minStaminaLevelReady;

    [Header("Stamina Regen")]
    [SerializeField] private float _waitTimeForStaminaRegen = 1.5f;
    [SerializeField] private float _staminaRegenSpeed = 15f;

    private Coroutine _staminaRegenCoroutine;


    private IEnumerator Start()
    {
        _currentStamina = _maxStamina;
        InGameHUD inGameWindow = null;

        while (!_staminaBarController)
        {
            inGameWindow = GameManager.Instance.GetManager<WindowsManager>().GetCurrentWindowIfType(SupportClasses.WindowName.InGameHUD) as InGameHUD;

            if (inGameWindow != null)
                _staminaBarController = inGameWindow.StaminaBarController;

            yield return new WaitForFixedUpdate();
        }

        UpdatePrecentValue();
        //загрузка из системы сохранения
    }

    public bool UseStamina(float gettingStamina)
    {
        if (_currentStamina <= 0 || _playerDead || gettingStamina > _currentStamina)
            return false;

        _currentStamina -= gettingStamina;

        if (_currentStamina <= 0)
            _currentStamina = 0;

        UpdatePrecentValue();

        if (_staminaRegenCoroutine != null)
            StopCoroutine(_staminaRegenCoroutine);

        _staminaRegenCoroutine = StartCoroutine(StaminaRegen());

        return true;
    
    }

    void UpdatePrecentValue()
    {
        float lastPrecent = _staminaPrecent;

        _staminaPrecent = (_currentStamina * 100) / _maxStamina;

        //проверяем процент полученного урона, если больше определенных порогов, то играем анимацию
        float difference = lastPrecent - _staminaPrecent;
        if (difference > 0)
            _staminaBarController.UpdateHealValue(_staminaPrecent);
        else
            _staminaBarController.UpdateHealValue(_staminaPrecent, true);
    }

    IEnumerator StaminaRegen()
    {
        yield return new WaitForSeconds(_waitTimeForStaminaRegen);

        while (_currentStamina < _maxStamina)
        {
            _currentStamina += _staminaRegenSpeed * Time.deltaTime;
            UpdatePrecentValue();
            yield return null;
        }

        if (_currentStamina > _maxStamina)
            _currentStamina = _maxStamina;

        _staminaRegenCoroutine = null;
    }
}
