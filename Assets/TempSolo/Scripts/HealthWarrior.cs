using System;
using UnityEngine;

public class HealthWarrior : MonoBehaviour {
  [SerializeField] private Collider colliderObject;
  [SerializeField] private float healthMax = 100;
  [SerializeField] private float health = 0;
  public event Action DamageEvent, DeathEvent;
  private void Awake() { health = healthMax; }
  public void TakeDamage(float damage) {
    health -= damage;
    if (health > 0) {
      DamageEvent?.Invoke();
      return;
    }
    Dead();
  }

  private void Dead() {
    colliderObject.enabled = false;
    DeathEvent?.Invoke();
  } 
}
