using UnityEngine;
using AttackSystem;

namespace AttackSystem
{
    public class EnemyBase : MonoBehaviour, IAttackable
    {
        [Header("Health Settings")]
        [SerializeField] protected float _maxHealth = 100f;
        [SerializeField] protected float _currentHealth;
        [SerializeField] protected bool _isAlive = true;

        [Header("Gizmos Colors")]
        [SerializeField] protected Color _patrolZoneColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] protected Color _followZoneColor = new Color(0f, 0f, 1f, 0.3f);
        [SerializeField] protected Color _attackZoneColor = new Color(1f, 0f, 0f, 0.3f);
        [SerializeField] protected Color _detectionZoneColor = new Color(1f, 1f, 0f, 0.3f);

        protected virtual void Start()
        {
            _currentHealth = _maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (!_isAlive) return;

            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _isAlive = false;
                Die();
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        protected virtual void Die()
        {
            // Базовая реализация смерти
            gameObject.SetActive(false);
        }

        #if UNITY_EDITOR
        protected void DrawWireDisk(Vector3 center, float radius, Color color)
        {
            float gizmoDiskThickness = 0.01f;
            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(1, gizmoDiskThickness, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        protected void DrawSector(Vector3 center, float radius, float angle, Color color)
        {
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawSolidArc(center, transform.up, transform.forward, angle, radius);
        }
        #endif
    }
} 