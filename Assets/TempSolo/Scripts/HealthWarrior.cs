using System;
using UnityEngine;
using Game.Core;
using SoundSystem;

public class HealthWarrior : MonoBehaviour {
    [SerializeField] private Collider colliderObject;
    [SerializeField] private float healthMax = 100;
    [SerializeField] private float health = 0;
    public event Action<float> DamageEvent;
    public event Action DeathEvent;

    private void Awake() { health = healthMax; }
    public void TakeDamage(float damage) {
        health -= damage;
        if (health > 0) {
            DamageEvent?.Invoke(damage);
            return;
        }
        Dead();
    }

    private void Dead() {
        colliderObject.enabled = false;
        DeathEvent?.Invoke();
    } 
}
