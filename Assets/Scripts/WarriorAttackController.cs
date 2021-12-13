using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAttackController : MonoBehaviour
{
    [SerializeField] private APWarriorController warriorMainController;

    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float castAttackTime = 5;
    [SerializeField] private float cooldownTime = 0.1f;

    [SerializeField] private Transform target;

    [SerializeField] private Animator attackVisual;
    [SerializeField] private GameObject attackModule;


    private void Start()
    {
        attackModule.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<APPlayerController>())
            target = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<APPlayerController>())
        {
            target = other.transform;
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
           transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    private IEnumerator Attack()
    {
        warriorMainController.attackMode = true;

        //transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

       // attackVisual.SetTrigger("Attack");
        warriorMainController.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(castAttackTime);

        //attackModule.SetActive(true);
        warriorMainController.attackMode = false;
 
        yield return new WaitForSeconds(cooldownTime);
      //  attackModule.SetActive(false);
    }



}
