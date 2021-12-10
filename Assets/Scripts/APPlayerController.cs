using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APPlayerController : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private GameObject visualPlayer;
    [SerializeField] private Animator anim;

    private Rigidbody _rb;

    [Header("Sounds")]
    [SerializeField] private AudioClip dieAudioClip;
    [SerializeField] private AudioClip useObjAudioClip;

    [Header("Particles")]
    [SerializeField] private ParticleSystem useInteractiveParticle;

    [SerializeField] private Vector3 SpawnPos;

    private bool dieActive = false;

    [SerializeField] private APCameraController _camera;

    public bool CanAttack = false;

    private void Start()
    {
        dieActive = false;
        SpawnPos = gameObject.transform.position;

        if (GetComponent<Rigidbody>())
            _rb = GetComponent<Rigidbody>();
        else
            Debug.LogError("Not find Rigidbody Component");
    }

    private void FixedUpdate()
    {
        if (!APGameController.Instance.gameIsPlayed || dieActive)
        {
            _rb.velocity = new Vector3(0, 0, 0);
            return;
        }

        Move();
    }

    public void Move()
    {
        float horizMove = JoystickStickk.Instance.HorizontalAxis();
        float verticalMove = JoystickStickk.Instance.VerticalAxis();

        if (horizMove == 0.0f && verticalMove == 0.0f)
        {
            if (CanAttack)
            {
                if (target == null) target = TargetSelectController.GetTarget();
                Attack();
            }
            else
            {
                if (anim.GetBool("Run")) anim.SetBool("Run", false);
                _rb.velocity = new Vector3(0, 0, 0);

                if (!dieActive) _camera.StopMove();
            }

            return;
        }

        if (doAttack) return;

        if (!dieActive) _camera.StartMove();

        float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
        visualPlayer.transform.rotation = Quaternion.Euler(0, angle, 0);

        _rb.velocity = new Vector3(horizMove * moveSpeed, 0, verticalMove * moveSpeed);

        if (!anim.GetBool("Run")) anim.SetBool("Run", true);
    }

    [Header("Attack Setting")]
    private APWarriorController target;
    [SerializeField] private APPlayerAttackTargetController TargetSelectController;
    [SerializeField] private float attackDistance = 1;
    [SerializeField] private float attakCooldown = 1;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private string[] animTiggerAsset = new string[3];
    private bool doAttack = false;

    private void Attack()
    {
        if (target.warriorDie)
        {
            target = TargetSelectController.GetTarget();
            return;
        }

        if (Vector3.Distance(transform.position, target.transform.position) > attackDistance)
        {
            if (doAttack) return;

            Vector3 vec = target.transform.position - transform.position;
            _rb.velocity = vec.normalized * moveSpeed;
            visualPlayer.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            if (!anim.GetBool("Run")) anim.SetBool("Run", true);
        }
        else
        {
            if (doAttack) return;

            visualPlayer.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            if (anim.GetBool("Run")) anim.SetBool("Run", false);
            _rb.velocity = new Vector3(0, 0, 0);
            if (!target.warriorDie) StartCoroutine(PanchAttack());
        }
    }

    private IEnumerator PanchAttack()
    {
        doAttack = true;
        anim.SetTrigger(animTiggerAsset[Random.Range(0, animTiggerAsset.Length)]);

        yield return new WaitForSeconds(0.25f);
        target.DamageIn(attackDamage);

        yield return new WaitForSeconds(attakCooldown);
        doAttack = false;
    }

    public void WinGame()
    {
        anim.SetBool("Run", false);
        anim.SetBool("WinGame", true);
    }

    public void LoseGame()
    {

        anim.SetBool("Run", false);
        anim.SetBool("LoseGame", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dieActive) return;

        if (other.GetComponent<DangerObstacle>())
        {
            StartCoroutine(DieAnim());

            if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(dieAudioClip);
        }

        if (other.GetComponent<APInteractbleObjController>())
        {
            if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(useObjAudioClip);
            anim.SetTrigger("UseItem");

            other.GetComponent<APInteractbleObjController>().UseObject();
        }
    }

    public Vector3 CurrentSpawnPos()
    {
        return SpawnPos;
    }

    public void SetNewSpawnPos(Vector3 pos)
    {
        SpawnPos = pos;
    }

    private IEnumerator DieAnim()
    {
        _camera.StopMove();
        dieActive = true;

        anim.SetBool("Run", false);
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(1.75f);

        _camera.StartMove();
        transform.position = SpawnPos;
        visualPlayer.transform.rotation = Quaternion.identity;

        dieActive = false;
    }
}
