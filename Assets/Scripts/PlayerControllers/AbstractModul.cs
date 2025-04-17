using Game.Core;
using UnityEngine;

namespace PlayerControllers
{
    public abstract class AbstractModul : MonoBehaviour
    {
        protected PlayerData _playerData;
        protected PlayerController _playerController;
        protected InputSystemManager _inputSystemMN;
        protected bool _isGround;

        private AbstractModul _abstractMethod;
        internal AbstractModul AbstractMethod { get => _abstractMethod; }

        internal bool moduleIsActive = false;

        internal bool _playerDead = false;

        public virtual void Start()
        {
            _abstractMethod = this;
        }

        public virtual void Init(PlayerData playerData, PlayerController _player)
        {
            _playerData = playerData;
            _playerController = _player;

            moduleIsActive = true;

            _inputSystemMN = GameManager.Instance.GetManager<InputSystemManager>();

            _player.IsGround += OnGround;

            SubscribeToInput();
        }

        protected virtual void SubscribeToInput() { }

        void OnGround(bool isGroundSet)
        {
            _isGround = isGroundSet;
        }

        public virtual void SetModuleActivityType(bool _modulIsActive)
        {
            moduleIsActive = _modulIsActive;

            if (!_modulIsActive)
                StopAllCoroutines();
        }

        public virtual void OnPlayerDied()
        {
            _playerDead = true;
        }

        public virtual void OnPlayerRespawn()
        {
            _playerDead = false;
        }
    }
}