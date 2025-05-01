using Game.Core;
using PlayerControllers;
using SupportSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseClasses
{
    public class LevelManager : SingeltonManager<LevelManager>
    {
        [SerializeField]
        private PostProcessingManager _postProcessManager = null;

        [SerializeField]
        private SupportClasses.WindowName _mainWindowAtScene = SupportClasses.WindowName.InGameHUD;
        
        static public PostProcessingManager PostProcessManager { get => Instance.GetManager(Instance._postProcessManager); }

        #region UI Managment
        private List<MonoBehaviour> _uiComponentStruct = new List<MonoBehaviour>();

        [SerializeField] private PlayerController _playerController;
        public PlayerController PlayerController { get => _playerController; }

        private IEnumerator Start()
        {
            while (!GameManager.Instance.InitReady)
                yield return null;

            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(_mainWindowAtScene);
        }

        public void RegisterUIComponent(MonoBehaviour component)
        {
            if (_uiComponentStruct.Contains(component))
                return;

            _uiComponentStruct.Add(component);
        }

        public void RegisterPlayerController(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public T GetUIComponent<T>() where T : MonoBehaviour
        {
            var componentType = typeof(T);
            T castedComponent = null;

            foreach (var compomemt in _uiComponentStruct)
            {
                if (componentType.IsInstanceOfType(compomemt))
                    castedComponent = compomemt as T;
            }

            if (castedComponent == null)
            {
                Instance.GetManager(componentType);
                Debug.LogError($"Компонент {componentType} не найден");
            }

            return castedComponent;
        }

        #endregion
    }
}

[Serializable]
public class UIElementType
{
    public MonoBehaviour UIComponent;
}