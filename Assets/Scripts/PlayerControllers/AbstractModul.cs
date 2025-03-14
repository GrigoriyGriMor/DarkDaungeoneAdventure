using UnityEngine;

namespace PlayerControllers
{
    public abstract class AbstractModul : MonoBehaviour
    {
        internal PlayerData _playerData;
        internal PlayerController _playerController;
        internal bool _isGround;

        private AbstractModul _abstractMethod;
        internal AbstractModul AbstractMethod { get => _abstractMethod; }

        internal bool moduleIsActive = false;

        void Start()
        {
            _abstractMethod = this;
        }

        public virtual void Init(PlayerData playerData, PlayerController _player)
        {
            _playerData = playerData;
            _playerController = _player;

            moduleIsActive = true;

            _player.IsGround += OnGround;
        }

        void OnGround(bool isGroundSet)
        {
            _isGround = isGroundSet;
        }

        public virtual void SetModuleActivityType(bool _modulIsActive)
        {
            moduleIsActive = _modulIsActive;
        }
    }
}