using Engine.GameManagers;
using SupportSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class GameManager : SingeltonManager<GameManager>
    {
        [SerializeField]
        private List<ManagerStruct> _managersStruct;
        
        private const string errorTxt = "GameManager havn't link. You need Add this link to stability work system";

        readonly List<AbstractManager> _managerInitList = new();

        bool _initReady = false;
        public bool InitReady { get => _initReady; }

        public T GetManager<T>() where T : AbstractManager
        {
            var managerType = typeof(T);
            T castedManager = null;
            
            foreach (var managersStruct in _managersStruct)
            {
                if(managerType.IsInstanceOfType(managersStruct.Manager)) 
                    castedManager = managersStruct.Manager as T;
            }

            if (castedManager == null)
            {
                Instance.GetManager(managerType);
                Debug.LogError($"Менеджер {managerType} не обнаружен в _managersStruct листе, попытка найти его на сцене..");
            }

            return castedManager;
        }

        public override void Awake()
        {
            base.Awake();
            
            #region Managers
            
            _managersStruct.Sort((x, y) =>
            {
                int result = x.InitOrder.CompareTo(y.InitOrder);
                return result;
            });

            for (int i = 0; i < _managersStruct.Count; i++)
            {
                if(_managersStruct[i].DisableDefaultInit) continue;
                TryInit(_managersStruct[i].Manager);
            }
            #endregion
        }
        
        private IEnumerator Start()
        {            
            DontDestroyOnLoad(gameObject);

            while (!_initReady)
            {
                foreach (AbstractManager mn in _managerInitList)
                {
                    _initReady = mn.IsReady;
                    if (!_initReady)
                        break;
                }

                yield return new WaitForFixedUpdate();
            }

            Debug.Log("GameManager Init");
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void TryInit(AbstractManager manager)
        {
            if (manager != null)
            {
                //print($"{manager.ToString()} is LOADED!");
                manager.Init();
                _managerInitList.Add(manager);
            }    
            else
            {
                Debug.LogError(errorTxt);
            }
        }
    }
}