using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForce : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform TargetPoint;
    [SerializeField] private float Force = 150f;

    [SerializeField] private float distance = 15;

    private void Update()
    {
        if (Vector3.Distance(TargetPoint.position, transform.position) > distance)
        {
            Debug.LogError("add force");
            _rb.AddForce((TargetPoint.position - transform.position).normalized * Force, ForceMode.Force);
        }
    }
}
