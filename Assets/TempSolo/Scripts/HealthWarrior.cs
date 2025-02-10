using System.Collections;
using UnityEngine;

public class HealthWarrior : MonoBehaviour {
  [SerializeField] private MeshRenderer meshWarrior;
  [SerializeField] private float healthMax = 100;
  [SerializeField] private float health = 0;

  private Material _material;
  private Color _originalColor;

  private void Awake() {
    health = healthMax;
    _material = meshWarrior.material;
    _originalColor = _material.color;
  }

  public void TakeDamage(float damage) {
    health -= damage;
    StartCoroutine(DelayColor());
    gameObject.SetActive(health >= 0);
  }

  private IEnumerator DelayColor() {
    _material.color = Color.red;
    yield return new WaitForSeconds(0.2f);
    _material.color = _originalColor;
  } 
  
}
