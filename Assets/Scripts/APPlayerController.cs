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

    [Header("Attack Setting")]
    private APWarriorController target;
    [SerializeField] private APPlayerAttackTargetController TargetSelectController;
    [SerializeField] private float attackDistance = 1;
    [SerializeField] private float attakCooldown = 1;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private string[] animTiggerAsset = new string[3];
    private bool doAttack = false;

    [Header("Skill_1")]
    [SerializeField] private UnityEngine.UI.Image cooldownVisual;
    [SerializeField] private float skill_1_Cooldown = 5;
    private bool canUseSkill_1 = true;
    [SerializeField] private int skill_1_Damage = 25;
    [SerializeField] private float maxCastDistance = 5;
    [SerializeField] private float minCastDistance = 2;
    [SerializeField] private ParticleSystem skill_1_particle;
    [SerializeField] private ParticleSystem skill_1_explosion_particle;

    private void Start()
    {
        skill_1_particle.gameObject.SetActive(false);
        visualPlayer.SetActive(true);

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
                //if (target == null) 
                    target = TargetSelectController.GetTarget();
                if (target != null) Attack();
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

    private void Attack()
    {
        if (target.warriorDie)
        {
            target = TargetSelectController.GetTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackDistance)
        {
            if (doAttack) return;

            if (canUseSkill_1 && (distance < maxCastDistance && distance > minCastDistance))
            {
                if (!target.warriorDie) StartCoroutine(FireBall_Skill_1_Attack());
                return;
            }

            Vector3 vec = target.transform.position - transform.position;
            _rb.velocity = vec.normalized * moveSpeed;
            visualPlayer.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            if (!anim.GetBool("Run")) anim.SetBool("Run", true);
        }
        else
        {
            if (doAttack) return;

            visualPlayer.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            if ((JoystickStickk.Instance.HorizontalAxis() == 0 && JoystickStickk.Instance.VerticalAxis() == 0) && anim.GetBool("Run")) anim.SetBool("Run", false);
            _rb.velocity = new Vector3(0, 0, 0);
            if (!target.warriorDie) StartCoroutine(PanchAttack());
        }
    }

    [SerializeField] private GameObject visualP;
    private IEnumerator FireBall_Skill_1_Attack()
    {
        canUseSkill_1 = false;
        doAttack = true;

        visualPlayer.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        yield return new WaitForSeconds(0.1f);

        anim.SetTrigger(animTiggerAsset[animTiggerAsset.Length - 1]);
        yield return new WaitForFixedUpdate();

        visualP.SetActive(false);
        skill_1_particle.gameObject.SetActive(true);

        while (Vector3.Distance(transform.position, target.transform.position) > attackDistance)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > maxCastDistance + 0.1f)
            {
                canUseSkill_1 = true;
                doAttack = false;
                visualP.SetActive(true);
                skill_1_particle.gameObject.SetActive(false);

                yield break;
            } 

            Vector3 vec = target.transform.position - transform.position;

            _rb.velocity = vec.normalized * (moveSpeed * 1.5f);
            yield return new WaitForFixedUpdate();
        }

        anim.SetTrigger(animTiggerAsset[animTiggerAsset.Length - 1]);

        if (skill_1_explosion_particle != null) skill_1_explosion_particle.Play();
        _rb.velocity = new Vector3(_rb.velocity.x * 0.8f, _rb.velocity.y, _rb.velocity.z * 0.8f);

        visualP.SetActive(true);
        skill_1_particle.gameObject.SetActive(false);
        target.DamageIn(skill_1_Damage);

        StartCoroutine(Skill_1_Cooldown());

        yield return new WaitForSeconds(attakCooldown);
        doAttack = false;
    }

    private IEnumerator Skill_1_Cooldown()
    {
        float _cd = 0;

        Color color_1 = cooldownVisual.color;
        cooldownVisual.color = Color.red;

        while (_cd < skill_1_Cooldown)
        {
            yield return new WaitForFixedUpdate();

            _cd += Time.deltaTime;
            cooldownVisual.fillAmount = ((_cd * 100) / skill_1_Cooldown) / 100;
        }

        cooldownVisual.color = color_1;
        cooldownVisual.rectTransform.sizeDelta = new Vector2(cooldownVisual.rectTransform.sizeDelta.x + 0.25f, cooldownVisual.rectTransform.sizeDelta.y + 0.25f);

        yield return new WaitForSeconds(0.1f);

        cooldownVisual.rectTransform.sizeDelta = new Vector2(cooldownVisual.rectTransform.sizeDelta.x - 0.25f, cooldownVisual.rectTransform.sizeDelta.y - 0.25f);

        yield return new WaitForFixedUpdate();
        cooldownVisual.fillAmount = 0;

        canUseSkill_1 = true;
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
        visualP.SetActive(true);
        skill_1_particle.gameObject.SetActive(false);
        canUseSkill_1 = true;
        doAttack = false;

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
