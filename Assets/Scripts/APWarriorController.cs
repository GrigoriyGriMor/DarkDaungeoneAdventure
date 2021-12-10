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
    [SerializeField] private float newMovePointRadius = 1;

    [Header("Attack Setting")]
    [SerializeField] private float AttackDamage = 50;
    [SerializeField] private float reloadTime = 5;

    [SerializeField] private float healPoint = 50;

    private Vector3 target;

    private bool attackMode = false;

    private void Start()
    {
        AI = GetComponent<NavMeshAgent>();
        AI.speed = 0;
        AI.destination = transform.position;
        attackMode = false;
        StartCoroutine(GetNewPoint());
    }




    private IEnumerator GetNewPoint()
    {
        target = new Vector3(transform.position.x + Random.Range(-newMovePointRadius, newMovePointRadius), transform.position.y, transform.position.z + Random.Range(-newMovePointRadius, newMovePointRadius));
        AI.destination = target;
        AI.speed = moveSpeed;
        yield return new WaitForSeconds(newMovePointTime);

        StartCoroutine(GetNewPoint());
    }
}
