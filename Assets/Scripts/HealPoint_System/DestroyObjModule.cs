using System.Collections;
using UnityEngine;

public class DestroyObjModule : MonoBehaviour
{
    [Header("Swap Obj")]
    [SerializeField] private GameObject destroyVisual;

    [Header("Origin Obj")]
    [SerializeField] private Transform setRoot; 
    [SerializeField] private GameObject hideVisual;
    private Transform destroyGO;
    Coroutine _hideTimerCoroutine;

    [Header("Hide Time"), SerializeField] private float hideTinme = 10f;

    public void SwapObj()
    { 
        hideVisual.SetActive(false);

        destroyGO = Instantiate(destroyVisual, setRoot.position, setRoot.rotation, setRoot).transform;

        if (_hideTimerCoroutine != null)
            StopCoroutine(_hideTimerCoroutine);

        _hideTimerCoroutine = StartCoroutine(HideTimer());
    }

    IEnumerator HideTimer()
    { 
        yield return new WaitForSeconds(hideTinme);

        for (int i = 0; i < destroyGO.childCount; i++)
            if (destroyGO.GetChild(i).TryGetComponent(out Collider collider))
                collider.isTrigger = true;

        yield return new WaitForSeconds(1);

        Destroy(destroyGO.gameObject);
        _hideTimerCoroutine = null;
    }

    public void ResetGO()
    {
        if (_hideTimerCoroutine != null)
            StopCoroutine(_hideTimerCoroutine);

        if (destroyGO != null) 
            Destroy(destroyGO.gameObject);
        hideVisual.SetActive(true);
    }
}
