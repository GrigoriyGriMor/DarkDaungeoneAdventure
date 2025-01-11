using Game.Core;
using System;

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