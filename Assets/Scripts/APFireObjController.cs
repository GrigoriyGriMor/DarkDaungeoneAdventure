using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class APFireObjController : MonoBehaviour
{
    [SerializeField] private GameObject fireObj;
    [SerializeField] private float flyTime = 1.5f;
    [SerializeField] private float fireSpeed = 5;

    [SerializeField] private float reloadTime = 5;

    [SerializeField] private Transform[] fireStartPos = new Transform[3];

    private List<GameObject> createObj = new List<GameObject>();

    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;

    private Coroutine coroutine;

    private void Start()
    {
        //source = GetComponent<AudioSource>();

        for (int i = 0; i < fireStartPos.Length; i++)
        {
            GameObject go = Instantiate(fireObj, fireStartPos[i].position, fireStartPos[i].rotation, fireStartPos[i]);
            createObj.Add(go);
        }

        if (coroutine == null)
            coroutine = StartCoroutine(Fire());
    }

    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(Fire());
    }
    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator Fire()
    {
        for (int i = 0; i < createObj.Count; i++)
        {
            createObj[i].transform.position = fireStartPos[i].position;
            createObj[i].transform.rotation = fireStartPos[i].rotation;
        }

        yield return new WaitForSeconds(reloadTime);

       // if (clip != null && SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(clip, source);

        float time = 0;

        while (time < flyTime)
        {
            time += Time.deltaTime;
            for (int i = 0; i < createObj.Count; i++)
                createObj[i].transform.Translate(Vector3.forward * fireSpeed);// = new Vector3(createObj[i].transform.position.x + fireSpeed, createObj[i].transform.position.y, createObj[i].transform.position.z);

            yield return new WaitForFixedUpdate();
        }

        coroutine = null;
        coroutine = StartCoroutine(Fire());
    }
}
