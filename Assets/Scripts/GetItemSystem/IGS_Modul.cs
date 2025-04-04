using PlayerControllers;
using System.Collections;
using UnityEngine;

public class IGS_Modul : AbstractModul
{
    [SerializeField] private GameObject _cutch;
    [SerializeField] private Transform _itemInCutchPos;

    [Header("")]
    [SerializeField] private float dropItemForce = 50f;

    private GameObject _getItemBtn;
    private GameObject _dropItemBtn;

    private IGS_Item _itemCanBeGetting;
    private IGS_Item _itemInCutchGO;

    private IEnumerator Start()
    {
        while (!_inputSystemMN)
            yield return new WaitForFixedUpdate();

        while (_getItemBtn == null || _dropItemBtn == null)
        {
            _getItemBtn = _inputSystemMN.GetVisual(InputControllerType.ItemGet);
            _dropItemBtn = _inputSystemMN.GetVisual(InputControllerType.ItemPut);
            yield return null;
        }

        _getItemBtn.gameObject.SetActive(false);
        _dropItemBtn.gameObject.SetActive(false);

        _inputSystemMN._getItemAction._clickAction += GetItem;
        _inputSystemMN._putItemAction._clickAction += DropItem;

        _cutch.SetActive(false);
    }

    public override void Init(PlayerData playerData, PlayerController _player)
    {
        base.Init(playerData, _player);

        _playerController.OnTriggerEnterAction += TriggerEnter;
        _playerController.OnTriggerExitAction += TriggerExit;
    }

    private void TriggerEnter(Collider other)
    {
        if (_itemInCutchGO != null) return;

        if (other.TryGetComponent(out IGS_Item item))
        {
            if (_itemCanBeGetting != null && _itemCanBeGetting != item)
                _itemCanBeGetting.ItemTriggerComeIn(false);

            _itemCanBeGetting = item;
            _itemCanBeGetting.ItemTriggerComeIn(true);
            _getItemBtn.gameObject.SetActive(true);
        }
    }

    private void TriggerExit(Collider other)
    {
        if (_itemCanBeGetting == null) return;

        if (other.GetComponent<IGS_Item>() == _itemCanBeGetting)
        {
            _itemCanBeGetting.ItemTriggerComeIn(false);
            _itemCanBeGetting = null;
            _getItemBtn.gameObject.SetActive(false);
        }
    }

    void DropItem()
    {
        if (_itemInCutchGO == null) return;

        _dropItemBtn.gameObject.SetActive(false);
        _cutch.gameObject.SetActive(false);

        Vector3 dropVec = _playerData.PlayerVisual.transform.forward * dropItemForce;
        _itemInCutchGO.ItemWasDroped(new Vector3(dropVec.x, dropVec.y + dropItemForce, dropVec.z));
        _itemInCutchGO = null;
    }

    void GetItem()
    {
        if (_itemCanBeGetting == null || _playerDead) return;

        _itemInCutchGO = _itemCanBeGetting;
        _itemCanBeGetting = null;
        _getItemBtn.gameObject.SetActive(false);

        _cutch.gameObject.SetActive(true);
        _itemInCutchGO.ItemWasGetting(_itemInCutchPos);

        _dropItemBtn.gameObject.SetActive(true);
    }

    public override void SetModuleActivityType(bool _modulIsActive)
    {
        base.SetModuleActivityType(_modulIsActive);
    }

    private void OnDisable()
    {
        _inputSystemMN._getItemAction._clickAction -= GetItem;
        _inputSystemMN._putItemAction._clickAction -= DropItem;
    }
}
