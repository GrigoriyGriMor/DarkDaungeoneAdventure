using BaseClasses;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace PlayerControllers
{
    public class AnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;

        private IEnumerator Start()
        {
            while (_playerController == null || !LevelManager.Instance)
            {
                if (LevelManager.Instance?.PlayerController != null)
                    _playerController = LevelManager.Instance.PlayerController;

                yield return null;
            }
        }

        public void OnAttackAnimationEvent()
        {
            if (_playerController != null)
            {
                _playerController.OnAttackAnimEvent();
            }
        }
    }
} 