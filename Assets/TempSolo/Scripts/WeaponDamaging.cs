using UnityEngine;

public class WeaponDamaging : MonoBehaviour {
   
    [SerializeField] private float _damage = 10;
    [SerializeField] private Collider colliderObject;

    public float GetDamage() {
        colliderObject.enabled = false;
        return _damage;
    }
}
