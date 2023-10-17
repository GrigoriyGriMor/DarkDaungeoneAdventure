using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraIdlePos;
    [SerializeField] private Transform cameraMovePos;

    [SerializeField] private Transform target;

    [SerializeField] private float moveSpeed = 5;
    private float moveS = 0;
    [SerializeField] private float rotateSpeed = 5;

    private void Start()
    {
        transform.SetParent(cameraIdlePos);
        moveS = moveSpeed / 5;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, moveS * Time.deltaTime);
        transform.LookAt(target);
    }

    private Coroutine coroutine;

    public void StartMove()
    {
        if (transform.parent == cameraMovePos) return;

        transform.SetParent(cameraMovePos);

        if (coroutine == null)
            coroutine = StartCoroutine(SpeedUp());
    }

    private IEnumerator SpeedUp()
    {
        while (moveS < moveSpeed)
        {
            moveS += 1 * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    public void StopMove()
    {
        if (transform.parent == cameraIdlePos) return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
        transform.SetParent(cameraIdlePos);
        moveS = moveSpeed / 5;
    }
}
