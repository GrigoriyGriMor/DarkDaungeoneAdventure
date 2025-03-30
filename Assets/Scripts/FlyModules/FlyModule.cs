using PlayerControllers;
using UnityEngine;

public class FlyModule : AbstractModul
{
    [SerializeField] private ParticleSystem _flyEffect;
    public bool IsFly { get; private set; }

    [SerializeField] private float _maxMagicPool = 100f;
    private float _currentMagicPool;

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _rotateMultiplay = 0.15f;

    private Vector3 _startDirection = Vector3.zero;

    [Header("Effect and Sound")]
    [SerializeField] private AudioClip _flySound;
    [SerializeField] private AudioClip _fallSound;

    [SerializeField] private ParticleSystem _startFlyParticle;
    [SerializeField] private ParticleSystem _endFlyParticle;

    [SerializeField] private string _endFlyAnimTriggerName = "EndFly";

    protected override void SubscribeToInput()
    {
        if (_inputSystemMN == null)
            return;
            
        _inputSystemMN._jumpAction._holdStartAction += () => ToggleFly(true);
        _inputSystemMN._jumpAction._holdEndAction += () => ToggleFly(false);
    }

    private void Start()
    {
        _currentMagicPool = _maxMagicPool;
        //_playerController.OnCollisionEnterAction += BreakFly;
    }

    void BreakFly(Collision collider)
    {
        ToggleFly(false);        
    }

    private void FixedUpdate()
    {
        if (!moduleIsActive)
            return;

        if (IsFly && _currentMagicPool > 0)
        {
            _currentMagicPool -= 15f * Time.deltaTime;
            Fly();

            if (_currentMagicPool <= 0)
            {
                _currentMagicPool = 0;
                ToggleFly(false);
            }    
        }
        else 
            if (!IsFly && _currentMagicPool < _maxMagicPool)
                _currentMagicPool += 5f * Time.deltaTime;
    }

    void Fly()
    {
        // Обновляем направление движения (изменяем _currentDirection на основе ввода)
        _startDirection += _playerData.PlayerBase.TransformDirection(Vector3.zero);
        _startDirection.Normalize();

        // Рассчитываем финальный вектор движения
        Vector3 globalMovement = _startDirection * _moveSpeed;

        // Устанавливаем скорость Rigidbody
        _playerData.PlayerRB.linearVelocity = globalMovement;

        // Двигаем камеру
        _playerData.PlayerMainCamera.StartMove();
    }

    private void ToggleFly(bool state = false)
    {
        if (IsFly == state)
            return;

        IsFly = state;
        _flyEffect.gameObject.SetActive(IsFly);
        _playerData.PlayerVisual.gameObject.SetActive(!IsFly);

        if (state)
        {
            _startDirection = _playerData.CameraControlBlock.forward.normalized; // Берем начальное направление из камеры

            if (_startFlyParticle) _startFlyParticle.Play();
        }
        else
        {
            if (_endFlyParticle) _endFlyParticle.Play();
            _playerData.PlayerAnimator.SetTrigger(_endFlyAnimTriggerName);
        }
    }
}