using System;
using UnityEngine;

namespace AttackSystem
{
    public interface IAttackable
    {
        event Action<Transform> DeadEvent;

        void TakeDamage(float damage);
        Transform GetTransform();
        bool IsAlive();
    }
} 