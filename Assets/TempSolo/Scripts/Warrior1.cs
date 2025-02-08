using System.Collections;
using UnityEngine;

public class Warrior1 : MonoBehaviour {
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private ZoneDamage1 zoneDamage1;
    [SerializeField] private float radiusPatrol = 20.0f;    
    [SerializeField] private float minDetectionRange;
    [SerializeField] private int angleDetection = 45;
    [SerializeField] private float delayAttack = 2.0f;
    [SerializeField] private float delayCast = 1.0f;
    [SerializeField] private float prediction = 0.0f;
    
    private StateWarrior _stateWarrior;
    private Transform _thisTransform;
    private Vector3 _startPosition;
    private Vector3 _direction;
    private float _distance;
    private float _detectionRange;
    private float _dot;
    private float _angleRadians;
    private float _angleDeg;
    
    void Start() {
        _detectionRange = minDetectionRange * minDetectionRange;
        zoneDamage1.Initialize();
        _thisTransform = transform;
        _startPosition = _thisTransform.position;
        StartCoroutine(Patrol());
    }
    
    private IEnumerator Patrol() {
        Vector3 point = Vector3.zero;
        _stateWarrior = StateWarrior.Patrol;
        float minDistanceToPoint = 0.1f;
        float speed = 1;
        
        while (true) {
            point = GetPointFollow();
            float distance = (_thisTransform.position - point).sqrMagnitude;;
            while (distance > minDistanceToPoint) {
                Debug.DrawLine(_thisTransform.position, point, Color.green);
                _thisTransform.LookAt(point);
                _thisTransform.position =
                    Vector3.MoveTowards(_thisTransform.position, point, speed * Time.deltaTime);
                distance = (_thisTransform.position - point).sqrMagnitude;
                
                if (CheckPlayer())
                {
                    StartCoroutine(Attack());
                    yield break;
                }
                
                yield return null;
            }

          
            
            
            yield return new WaitForSeconds(0.1f);
            
        }
    }

    private Vector3 GetPointFollow() {
        Vector3 direction = Vector3.zero;
        Vector3 center = _startPosition;
        Vector2 randomDirection = Random.insideUnitCircle;
        direction.x = randomDirection.x;
        direction.z = randomDirection.y;
        float radius = radiusPatrol;
        Vector3 pointFollow = center + (radius * direction);
        return pointFollow;
    }
  
    
    
    private IEnumerator Attack(){
        _stateWarrior = StateWarrior.Attack;
        Vector3 positionPlayer = targetPlayer.position + targetPlayer.forward * prediction;
        zoneDamage1.SetPosition(positionPlayer);
        zoneDamage1.Show();
        
        yield return new WaitForSeconds(delayAttack);
        
        // int delayAttack = this.delayAttack;
        // const int tick = 1; 
        // while (delayAttack > 0) {
        //     yield return new WaitForSeconds(tick);
        //     delayAttack--;
        // }
        
        zoneDamage1.Damage();
        yield return new WaitForSeconds(delayCast);
        zoneDamage1.Hide();
        StartCoroutine(Patrol());
    }
  
    private bool CheckPlayer() {
        _direction = targetPlayer.position - _thisTransform.position;
        _distance = _direction.sqrMagnitude; 
        if (_distance > _detectionRange) return false;
        _dot = Vector3.Dot(_thisTransform.forward, _direction.normalized);
        _angleRadians = Mathf.Acos(_dot);
        _angleDeg = _angleRadians * Mathf.Rad2Deg;
        return _angleDeg < angleDetection;
    }
    
    private void OnDrawGizmos() {
        Vector3 center = transform.position;
        float radius = radiusPatrol;
        DrawWireDisk(center, radius, Color.red);
    }
    
    public void DrawWireDisk(Vector3 center, float radius, Color color) {
        float gizmoDiskThickness = 0.01f;
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(1, gizmoDiskThickness, 1));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }
}

enum StateWarrior {
    Idle,
    Attack,
    Run,
    Wait,
    Patrol
}