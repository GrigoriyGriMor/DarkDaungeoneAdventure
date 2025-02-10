using UnityEngine;

public class TempPlayerAttack : MonoBehaviour {
    [SerializeField] private Animator animator;  

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            animator.SetTrigger("Attack1");
        }
    }
    
}
