using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGS_Item : MonoBehaviour
{
    [SerializeField] private ParticleSystem _triggerActiveParticle;
    [SerializeField] private ParticleSystem _itemInCutchParticle;

    [Header("")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _parableHight = 1f;
    private Coroutine _moveToPointCoroutine;

    bool InCutch = false;
    Collider _myCollider;
    Rigidbody _myRB;

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
        _myCollider.enabled = true;

        _myRB = GetComponent<Rigidbody>();
        _myRB.isKinematic = true;
    }

    public void ItemTriggerComeIn(bool active)
    {
        if (InCutch) return;

        if (_triggerActiveParticle != null)
        {
            if (active)
                _triggerActiveParticle.Play();
            else
                _triggerActiveParticle.Stop();
        }
    }

    public void ItemWasGetting(Transform parent)
    {
        if (_triggerActiveParticle != null)
           _triggerActiveParticle.Stop();

        InCutch = true;
        _myCollider.enabled = false;

        if (_moveToPointCoroutine != null)
            StopCoroutine(_moveToPointCoroutine);

        _moveToPointCoroutine = StartCoroutine(MoveToPointCoroutine(parent));
    }

    public void ItemWasDroped(Vector3 dropVector)
    {
        transform.parent = null;
        _myCollider.enabled = true;

        _myRB.isKinematic = false;
        _myRB.AddForce(dropVector, ForceMode.Impulse);

        InCutch = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.rotation = Quaternion.identity;
            _myRB.isKinematic = true;
        }
    }

    IEnumerator MoveToPointCoroutine(Transform parent)
    {
        float parableCurrentHight = _parableHight;
        float allDistance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(parent.position.x, 0, parent.position.z));

        while (Vector3.Distance(transform.position, parent.position) > 0.1f)
        {
            if (parableCurrentHight != 0 && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                new Vector3(parent.position.x, 0, parent.position.z)) < (allDistance / 4))
                    parableCurrentHight = 0;

            transform.position = Vector3.Lerp(transform.position, new Vector3(parent.position.x, parent.position.y + parableCurrentHight, parent.position.z), _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.localRotation, parent.rotation, 0.5f * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        transform.parent = parent;
        yield return new WaitForEndOfFrame();

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    
        if (_itemInCutchParticle != null)
            _itemInCutchParticle.Play();
    }

    public void DeactiveComponents()
    {
        MonoBehaviour[] _allComponents = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in _allComponents)
            Destroy(component);

        Destroy(_myCollider);
        Destroy(_myRB);
    }
}
