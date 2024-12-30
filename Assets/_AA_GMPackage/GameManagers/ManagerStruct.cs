using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine.Serialization;

namespace Engine.GameManagers
{
    [Serializable]
    public struct ManagerStruct
    {
        public AbstractManager Manager;
        public int InitOrder;
        public bool DisableDefaultInit;
    }
}