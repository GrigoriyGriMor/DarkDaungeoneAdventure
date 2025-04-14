using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActionActivator))]
public class IGS_ItemEndPos : MonoBehaviour
{
    [SerializeField] private Transform _endTransform;
    [SerializeField] private bool _useEndPointScale = false;
    [SerializeField] private float _moveSpeed = 25f;

    [SerializeField] private ParticleSystem _setItemParticle;

    Coroutine _sitepCoroutine;

    private void Start()
    {
        GetComponent<ActionActivator>()._activateWithGOEvent.AddListener((value) => SetItemToEndPos(value));
    }

    void SetItemToEndPos(Transform _obj)
    { 
        if (_sitepCoroutine != null)
            StopCoroutine(_sitepCoroutine);

        _sitepCoroutine = StartCoroutine(ItemToEndPosCoroutine(_obj));
    }

    IEnumerator ItemToEndPosCoroutine(Transform _obj)
    {
        _obj.GetComponent<Rigidbody>().isKinematic = true;

        while (Vector3.Distance(_obj.position, _endTransform.position) > 0.25f)
        {
            _obj.position = Vector3.Lerp(_obj.position, _endTransform.position, _moveSpeed * Time.deltaTime);
            _obj.rotation = Quaternion.Lerp(_obj.localRotation, _endTransform.rotation, _moveSpeed * Time.deltaTime);

            if (_useEndPointScale)
                _obj.transform.localScale = Vector3.Lerp(_endTransform.localScale, _endTransform.localScale, (_moveSpeed / 2) * Time.deltaTime); ;

            yield return new WaitForEndOfFrame();
        }

        _obj.position = _endTransform.position;
        _obj.rotation = _endTransform.rotation;

        _obj.GetComponent<IGS_Item>().DeactiveComponents();

        if (_setItemParticle != null)
            _setItemParticle.Play();

        _obj.parent = _endTransform;
    }
}
