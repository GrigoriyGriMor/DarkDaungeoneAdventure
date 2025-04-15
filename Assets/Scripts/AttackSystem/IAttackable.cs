using UnityEngine;

namespace AttackSystem
{
    public interface IAttackable
    {
        void TakeDamage(float damage);
        Transform GetTransform();
        bool IsAlive();
    }
} 