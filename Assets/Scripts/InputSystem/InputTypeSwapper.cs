using System;
using UnityEngine;

public class InputTypeSwapper : MonoBehaviour
{
    [SerializeField] private ControlType _controlType = ControlType.Touch;

    [SerializeField] private GameObject[] _touchControllers = new GameObject[0];
    [SerializeField] private GameObject[] _keybardControllers = new GameObject[0];

    private void Awake()
    {
        switch (_controlType)
        { 
            case ControlType.Touch:
                OnControlType(_touchControllers, true);
                OnControlType(_keybardControllers, false);
                break;
            case ControlType.Keyboard:
                OnControlType(_keybardControllers, true);
                OnControlType(_touchControllers, false);
                break;
            default:
                OnControlType(_touchControllers, true);
                OnControlType(_keybardControllers, false);
                break;
        }
    }

    void OnControlType(GameObject[] array, bool activate)
    { 
        for (int i = 0; i < array.Length; i++)
            array[i].SetActive(activate);
    }
}

[Serializable]
public enum ControlType
{ 
    Empty,
    Keyboard,
    Touch,
}
