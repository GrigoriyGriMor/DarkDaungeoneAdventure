using UnityEngine;

public class HookObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _connectPoint;
    [SerializeField] private float _objMass = 250f;

    [Header(""), SerializeField] private ParticleSystem _connectParticle;
    [Header(""), SerializeField] private Transform _visual;

    private FixedJoint _joint;

    private void Start()
    {
        GetComponent<Rigidbody>().mass = _objMass;

        if (_joint != null)
            Destroy(_joint);

        _connectPoint.gameObject.SetActive(false);
        if (_visual != null)
            _visual.gameObject.SetActive(false);
    }

    public Rigidbody SetPointConnect(Vector3 _position, Vector3 _rotation)
    {
        _connectPoint.gameObject.SetActive(true);

        _connectPoint.transform.position = _position;
        _connectPoint.transform.up = _rotation;

        if (_visual != null)
        {
            _visual.gameObject.SetActive(true);
            _visual.transform.position = _position;
            _visual.transform.up = _rotation;
        }

        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = _connectPoint;

        if (_connectParticle != null) _connectParticle.Play();

        return _connectPoint;
    }

    public void BreakConnect()
    {
        if (_joint != null) Destroy(_joint);

        _connectPoint.gameObject.SetActive(false);
        _visual.gameObject.SetActive(false);
    }


}
