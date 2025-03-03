using UnityEngine;

public class DamageWarrior : MonoBehaviour {
    [SerializeField] private HealthWarrior health;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out WeaponDamaging weaponDamaging)) {
            float damage = weaponDamaging.GetDamage();
            health.TakeDamage(damage);
        }
    }
}
