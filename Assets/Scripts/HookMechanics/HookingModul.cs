using PlayerControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class HookingModul : AbstractModul
{
    [SerializeField] private float _hookDistance = 15f;
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Transform _hookStartPoint;
    [SerializeField] private float springDamping = 80f; 
    private Rigidbody _hookConnectPoint;
    private HookObject _hookObj;
    private float connectLineDistance = 0;

    RaycastHit hit;

    private void Start()
    {
        _lineRenderer.positionCount = 0;
        connectLineDistance = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)/*Input.GetTouch()*/)
        {
            ConnectToPoint();
        }

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

            if (Input.GetKeyDown(KeyCode.Q))
        {
            BreakConnectWithPoint();
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
            }
            else
            {
                Debug.LogError("Нужно подойти поближе");
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
    }
}
