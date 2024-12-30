using Game.Core;
using System.Collections;
using UnityEngine;

public class LoadScreenController : AbstractManager
{
    [SerializeField] private GameObject _loadSc;

    [SerializeField] private Animator _anim;

    [Header("Использовать DontDestroyOnLoad?")]
    [SerializeField] private bool useDontDestroyGO = true;


    private void Start()
    {
        if (useDontDestroyGO)
            DontDestroyOnLoad(gameObject);

       _loadSc.SetActive(false);
    }

    public void LoadScreenActive()
    {
        if (_loadSc != null && !_loadSc.activeInHierarchy) _loadSc.gameObject.SetActive(true);
    }

    public void LoadScreenDeactive()
    {
        StartCoroutine(Deactive());
    }

    private IEnumerator Deactive()
    {
        _anim.SetTrigger("Loaded");

        yield return new WaitForSeconds(0.5f);

        if (_loadSc != null && _loadSc.activeInHierarchy) _loadSc.gameObject.SetActive(false);
    }
}
