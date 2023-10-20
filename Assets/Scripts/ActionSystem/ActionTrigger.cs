using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    [SerializeField] private int _triggerID = -1;

    public int GetTriggerID()
    {
        return _triggerID;
    }
}
