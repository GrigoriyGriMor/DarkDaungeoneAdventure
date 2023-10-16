using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControllers
{
    public abstract class AbstractModul : MonoBehaviour
    {
        internal PlayerData _playerData;

        private AbstractModul _abstractMethod;
        internal AbstractModul AbstractMethod { get => _abstractMethod; }

        void Start()
        {
            _abstractMethod = this;
        }

        public void Init(PlayerData playerData)
        {
            _playerData = playerData;
        }
    }
}