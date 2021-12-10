using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class APWarriorController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent AI;

    [Header("Move Setting")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float newMovePointTime = 5;
    private Vector3 startPos;
    [SerializeField] private float newMovePointRadius = 1;

    [Header("Attack Setting")]
    [SerializeField] private float AttackDamage = 50;
    [SerializeField] private float reloadTime = 5;

    [Header("")]
    [SerializeField] private float healPoint = 50;
    [SerializeField] private ParticleSystem dieParticle; 

    [SerializeField] private Animator anim; 

    private Vector3 target;

    private bool attackMode = false;

    private void Start()
    {
        startPos = transform.position;

        AI = GetComponent<NavMeshAgent>();
        AI.speed = 0;
        AI.destination = transform.position;
        attackMode = false;

        moveCoroutine = null;
        moveCoroutine = StartCoroutine(GetNewPoint());
    }

    private void FixedUpdate()
    {
        if (healPoint <= 0) return;

        if (!attackMode && (AI.velocity.x != 0 && AI.velocity.z != 0))
            anim.SetBool("Run", true);
        else
            anim.SetBool("Run", false);
    }

    private Coroutine moveCoroutine;
    private IEnumerator GetNewPoint()
    {
        yield return new WaitForSeconds(newMovePointTime);

        target = new Vector3(startPos.x + Random.Range(-newMovePointRadius, newMovePointRadius), startPos.y, startPos.z + Random.Range(-newMovePointRadius, newMovePointRadius));
        RaycastHit hit; 
        Physics.Raycast(transform.position, target - transform.position, out hit);

        if (hit.collider == null)
            AI.destination = target;
        else
            AI.destination = hit.point;

        AI.speed = moveSpeed;

        moveCoroutine = null;
        moveCoroutine = StartCoroutine(GetNewPoint());
    }

    public void DamageIn(int damage)
    {
        if (healPoint <= 0) return;


        healPoint -= damage;

        if (healPoint <= 0)
            StartCoroutine(DieWarrior());
        else
            anim.SetTrigger("Damage");
    }

    public bool warriorDie = false;
    private IEnumerator DieWarrior()
    {
        warriorDie = true;
        StopCoroutine(moveCoroutine);
        AI.speed = 0;
        AI.enabled = false;

        if (dieParticle != null) dieParticle.Play();

        anim.SetTrigger("Die");
        yield return new WaitForSeconds(4);

        while (transform.position.y > -1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1 * Time.deltaTime, transform.position.z);
            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }

//    bool t = false;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(newMovePointRadius * 2, 1, newMovePointRadius * 2)); //Рисуем куб
    }
#endif
}
