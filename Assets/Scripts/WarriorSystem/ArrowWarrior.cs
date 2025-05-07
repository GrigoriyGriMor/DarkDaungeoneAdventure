using BaseClasses;
using UnityEngine;

public class ArrowWarrior : EnemyBase
{
    private void FixedUpdate()
    {
        if (LevelManager.Instance == null && LevelManager.Instance.PlayerController == null) return;

        Vector3 directionToTarget = (LevelManager.Instance.PlayerController.transform.position - transform.position).normalized;
        float targetYRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        transform.rotation = targetRotation;
    }
}
