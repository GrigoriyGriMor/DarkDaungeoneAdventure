using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APPlayerAttackTargetController : MonoBehaviour
{
    [SerializeField]private List<APWarriorController> targets = new List<APWarriorController>();
    [SerializeField] private APPlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<APWarriorController>())
        {
            targets.Add(other.GetComponent<APWarriorController>());
            
            if (targets.Count == 1) player.CanAttack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<APWarriorController>())
        {
            targets.Remove(other.GetComponent<APWarriorController>());
            if (targets.Count <= 0) player.CanAttack = false;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < targets.Count; i++)
            if (targets[i].warriorDie) targets.Remove(targets[i]);
    }

    public APWarriorController GetTarget()
    {
        if (targets.Count <= 0)
        {
            player.CanAttack = false;
            return null;
        }

        int _targetID = 0;
        float distance = Vector3.Distance(transform.position, targets[0].transform.position);

        if (targets.Count > 1)
            for (int i = 1; i < targets.Count; i++)
            {
                if (Vector3.Distance(transform.position, targets[i].transform.position) < distance)
                    _targetID = i;
            }

        return targets[_targetID];
    }
}
