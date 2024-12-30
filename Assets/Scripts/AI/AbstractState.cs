using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AbstractState : MonoBehaviour
{
    public bool StateIsActive { get => _stateIsActive; }

    public BaseAIParametrs _aiParametrs;

    [SerializeField] private float _thresholdMotivationMakingDecision = 5f;
    [HideInInspector] public float ThresholdMotivationMakingDecision { get => _thresholdMotivationMakingDecision; }
    [SerializeField] private List<UsedModuleForThreshold> _usedModuleForThresholdList = new List<UsedModuleForThreshold>();

    private bool _stateIsActive = false;

    public virtual void Init(BaseAIParametrs _param)
    { 
        _aiParametrs = _param;
        _stateIsActive = true;
    }

    public virtual void DoStateLogic()
    {
        //State logic
    }
}

[System.Serializable]
public class BaseAIParametrs
{
    [HideInInspector] public NavMeshAgent AgentAI;
    [HideInInspector] public Animator Animator;
}

[System.Serializable]
public class UsedModuleForThreshold
{
    [SerializeField] private AbstractAIModule module;
    [SerializeField] private float thresholdMultiplay;
}