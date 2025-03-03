using Game.Core;
using PlayerControllers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HookingModul : AbstractModul
{
    [SerializeField] private float _hookDistance = 15f;
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Transform _hookStartPoint;
    [SerializeField] private float springDamping = 80f;

    private GameObject _breakHookBtnVisual; 

    private Rigidbody _hookConnectPoint;
    private HookObject _hookObj;
    private float connectLineDistance = 0;

    RaycastHit hit;

    InputSystemManager _inputSystemMN;

    private IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return new WaitForFixedUpdate();

        _inputSystemMN = GameManager.Instance.GetManager<InputSystemManager>();

        _lineRenderer.positionCount = 0;
        connectLineDistance = 0;

        _inputSystemMN._hookAction += BreakConnectWithPoint;

        while (_breakHookBtnVisual == null)
        { 
            _breakHookBtnVisual = _inputSystemMN.GetVisual(InputControllerType.HookBreak);
            yield return null;
        }
        
        _breakHookBtnVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!moduleIsActive) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            ConnectToPoint();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            BreakConnectWithPoint();
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            ConnectToPoint();
#endif

        if (_lineRenderer.positionCount == 2 && _hookConnectPoint != null)
        {
            _lineRenderer.SetPosition(0, _hookStartPoint.position);
            _lineRenderer.SetPosition(1, _hookConnectPoint.transform.position);

            if (Vector3.Distance(_hookConnectPoint.position, _hookStartPoint.position) > connectLineDistance)
            {
                _hookConnectPoint.AddForce((_hookStartPoint.position - _hookConnectPoint.position).normalized * springDamping, ForceMode.Force);

                if (Vector3.Distance(_hookConnectPoint.position, _hookStartPoint.position) > connectLineDistance * 1.5f)
                    _playerData.PlayerRB.AddForce((_hookConnectPoint.position - _hookStartPoint.position).normalized * springDamping * 14, ForceMode.Force);
            }
        }
    }

    void ConnectToPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit, 25);
        if (hit.collider != null && hit.collider.TryGetComponent(out HookObject hookObj))
        {
            if (Vector3.Distance(hit.point, _hookStartPoint.position) < _hookDistance)
            {
                BreakConnectWithPoint();

                //Активируем кнопку при нажатии на которую ломается связь с объектом
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, _hookStartPoint.position);
                _lineRenderer.SetPosition(1, hit.point);

                _hookObj = hookObj;
                _hookConnectPoint = hookObj.SetPointConnect(hit.point, (_hookStartPoint.position - hit.point));
                connectLineDistance = Vector3.Distance(hit.point, _hookStartPoint.position);

                _playerController.IsHooking(true);

                _breakHookBtnVisual.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("distance error");
                //отправляем сообщение в Answer контроллер
            }
        }
    }

    void BreakConnectWithPoint()
    {
        _lineRenderer.positionCount = 0;
        connectLineDistance = 0;

        if (_hookObj != null)
        {
            _hookObj.BreakConnect();
            _hookObj = null;
        }

        _playerController.IsHooking(false);
        _breakHookBtnVisual.gameObject.SetActive(false);
    }

    public override void SetModuleActivityType(bool _modulIsActive)
    {
        base.SetModuleActivityType(_modulIsActive);

        BreakConnectWithPoint();
    }
}
