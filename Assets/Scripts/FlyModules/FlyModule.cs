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
    }

    private void Update()
    {
        if (!moduleIsActive)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentMagicPool < _maxMagicPool / 2)
                ToggleFly(false);
            else
                ToggleFly(!IsFly);
        }

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
        float horizMove = _inputSystemMN.Move().x;
        float verticalMove = _inputSystemMN.Move().y;

        _playerData.PlayerBase.eulerAngles =
            new Vector3(_playerData.PlayerBase.eulerAngles.x + (-verticalMove * (_moveSpeed * _rotateMultiplay)), _playerData.PlayerBase.eulerAngles.y + horizMove * (_moveSpeed * _rotateMultiplay), _playerData.PlayerBase.eulerAngles.z);

        Vector3 vec = new Vector3(0, 0, _moveSpeed);
        _playerData.PlayerRB.linearVelocity = transform.TransformVector(vec);

        _playerData.PlayerMainCamera.StartMove();
    }

    private void ToggleFly(bool state = false)
    {
        IsFly = state;
        _flyEffect.gameObject.SetActive(IsFly);
        _playerData.PlayerVisual.gameObject.SetActive(!IsFly);

        if (!state)
        {
            _playerData.PlayerBase.eulerAngles = new Vector3(0, _playerData.PlayerBase.eulerAngles.y, 0);
        }
        else
            _playerData.PlayerBase.eulerAngles = _playerData.CameraControlBlock.eulerAngles;
    }

    private void OnDestroy()
    {
        //if (_inputSystemMN != null)
        //    _inputSystemMN._jumpAction -= ToggleFly;
    }
}