using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APInteractbleObjController : MonoBehaviour
{
    [Header("Анимация Рычага")]
    [SerializeField] private Animator objAnim;

    [Header("Задержка")]
    [SerializeField] private float waitTime = 1;

    [Header("Анимация активного обьекта")]
    [SerializeField] private Animator useObjAnim;
    [SerializeField] private ParticleSystem particle;

    [SerializeField] private bool oneShot = false;
    private bool oneShotActive = false;

    public void UseObject()
    {
        StartCoroutine(UseObj());    
    }


    private IEnumerator UseObj()
    {
        if (oneShot)
        {
            if (oneShotActive) yield break;

            oneShotActive = true;

            if (objAnim != null)
                objAnim.SetTrigger("Start");

            yield return new WaitForSeconds(waitTime);

            if (useObjAnim != null)
                useObjAnim.SetTrigger("Start");

            if (particle != null)
                particle.Play();
        }
        else
        {
            if (objAnim != null)
                objAnim.SetTrigger("Start");

            yield return new WaitForSeconds(waitTime);

            if (useObjAnim != null)
                useObjAnim.SetTrigger("Start");

            if (particle != null)
                particle.Play();
        }
    }
}
