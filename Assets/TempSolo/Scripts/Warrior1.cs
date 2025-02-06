using System.Collections;
using UnityEngine;

public class Warrior1 : MonoBehaviour {
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private ZoneDamage1 prefabZoneDamage1;
    [SerializeField] private float minDetectionRange;
    [SerializeField] private int angleDetection;
    [SerializeField] private int delayAttack;
    [SerializeField] private float prediction;
    
    [SerializeField] private StateWarrior stateWarrior;
    [SerializeField] private float detectionRange;
    private Transform _thisTransform;
    private ZoneDamage1 _zoneDamage1;
    
    void Start() {
        _zoneDamage1 = GetZoneDamage();
        _zoneDamage1.Initialize();
        _thisTransform = transform;
        StartCoroutine(Patrol());
    }

    private ZoneDamage1 GetZoneDamage() {
        return Instantiate(prefabZoneDamage1);
    }
    
    private IEnumerator Patrol() {
        stateWarrior = StateWarrior.Patrol;
        while (true) {
            if (CheckPlayer()) {
                StartCoroutine(Attack());
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator Attack(){
        stateWarrior = StateWarrior.Attack;
        Vector3 positionPlayer = targetPlayer.position + targetPlayer.forward * prediction;
        _zoneDamage1.SetPosition(positionPlayer);
        _zoneDamage1.Show();
        int delayAttack = this.delayAttack;
        
        while (delayAttack > 0) {
            yield return new WaitForSeconds(1.0f);
            delayAttack--;
        }
        
        _zoneDamage1.Damage();
        yield return new WaitForSeconds(1.0f);
        _zoneDamage1.Hide();
        StartCoroutine(Patrol());
    }
  
    private bool CheckPlayer() {
        detectionRange = Vector3.Distance(_thisTransform.position, targetPlayer.position);
        return detectionRange < minDetectionRange;
    }
    
}

enum StateWarrior {
    Idle,
    Attack,
    Run,
    Wait,
    Patrol
}