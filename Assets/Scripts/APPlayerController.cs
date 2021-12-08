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
            anim.SetBool("Run", false);
            _rb.velocity = new Vector3(0, 0, 0);

            if (!dieActive) _camera.StopMove();

            return;
        }

        if (!dieActive) _camera.StartMove();

        float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
        visualPlayer.transform.rotation = Quaternion.Euler(0, angle, 0);

        _rb.velocity = new Vector3(horizMove * moveSpeed, 0, verticalMove * moveSpeed);

        anim.SetBool("Run", true);
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

    public Vector3 CurrentSpawnPos()
    {
        return SpawnPos;
    }

    public void SetNewSpawnPos(Vector3 pos)
    {
        SpawnPos = pos;
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
