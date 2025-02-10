using UnityEngine;

public class WeaponDamaging : MonoBehaviour {
   
    [SerializeField] private float _damage = 10;

    public float GetDamage() {
        return _damage;
    }
}
