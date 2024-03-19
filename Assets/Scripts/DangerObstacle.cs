using UnityEngine;

public class DangerObstacle : MonoBehaviour
{
    [SerializeField] private float _damage = 10000;

    public float GetDamage()
    {
        return _damage;
    }
}
