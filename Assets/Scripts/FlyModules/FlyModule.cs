using Game.Core;
using PlayerControllers;
using System.Collections;
using UnityEngine;

public class FlyModule : AbstractModul
{
    [SerializeField] private ParticleSystem _flyEffect;
    public bool IsFly = false;

    [Header("")]
    [SerializeField] private float _maxMagicPool = 100f;
    [SerializeField] private float _currentMagicPool = 0f;
    [SerializeField] private float _enegyDropCount = 15f;

    [Header("")]
    [SerializeField] private float _moveSpeed = 7;
    private float _mSpeed = 0;

    InputSystemManager _inputSystemMN;

    public IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return new WaitForFixedUpdate();

        _inputSystemMN = GameManager.Instance.GetManager<InputSystemManager>();

        _mSpeed = _moveSpeed;
        _currentMagicPool = _maxMagicPool;
    }

    private void Update()
    {
        if (!moduleIsActive || _inputSystemMN == null) return;

        if (IsFly)
        {
            Fly();
            _currentMagicPool -= _enegyDropCount * Time.deltaTime;

            if (_currentMagicPool <= 0)
            {
                _currentMagicPool = 0;
                IsFly = false;
                FlyEnable(IsFly);
            }
        }
        else
        {
            if (_currentMagicPool < _maxMagicPool)
                _currentMagicPool += _enegyDropCount * Time.deltaTime;
            else
                _currentMagicPool = _maxMagicPool;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentMagicPool < 50)
                IsFly = false;
            else
                IsFly = !IsFly;

            FlyEnable(IsFly);
        }
    }

    private void FlyEnable(bool fly)
    {
        _flyEffect.gameObject.SetActive(fly);
        _playerData.PlayerVisual.gameObject.SetActive(!fly);

        if (fly)
        {
            _playerData.PlayerBase.localEulerAngles = _playerData.CameraControlBlock.eulerAngles;
            _playerData.CameraControlBlock.localEulerAngles = 
                new Vector3(_playerData.CameraControlBlock.localEulerAngles.x, 0, _playerData.CameraControlBlock.localEulerAngles.z);
        }
        else
            _playerData.PlayerBase.localEulerAngles = new Vector3(0, _playerData.PlayerBase.localEulerAngles.y, 0);
    }

    public override void SetModuleActivityType(bool _modulIsActive)
    {
        base.SetModuleActivityType(_modulIsActive);

        if (!_modulIsActive)
            ResetAllParam();
    }

    void ResetAllParam()
    {
        _mSpeed = 0;
    }

    void Fly()
    {
        float horizMove = _inputSystemMN.Move().x;
        float verticalMove = _inputSystemMN.Move().y;

        _playerData.PlayerBase.localEulerAngles =
            new Vector3(_playerData.PlayerBase.localEulerAngles.x + (-verticalMove * 0.25f), _playerData.PlayerBase.localEulerAngles.y + horizMove * 0.25f, _playerData.PlayerBase.localEulerAngles.z);

        Vector3 vec = new Vector3(0, 0, _mSpeed);
        _playerData.PlayerRB.linearVelocity = transform.TransformVector(vec);

        _playerData.PlayerMainCamera.StartMove();
    }
}
