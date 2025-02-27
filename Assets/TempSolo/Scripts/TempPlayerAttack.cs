using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class TempPlayerAttack : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private Collider colliderWeapon;
    [SerializeField] private float delayAfterAttack = 0.5f;
    [SerializeField] private string stateAttack = "Attack";
    
    private bool isAttack;
    private void Awake() {
        colliderWeapon.enabled = false;
    }

    void Update() {
        if (isAttack) return;
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(TimerAttack());
        }
    }

    private IEnumerator TimerAttack() {
        isAttack = true;
        animator.SetTrigger(stateAttack);
        colliderWeapon.enabled = true;
        yield return new WaitForSeconds(delayAfterAttack);
        colliderWeapon.enabled = false;
        isAttack = false;
    }
}
