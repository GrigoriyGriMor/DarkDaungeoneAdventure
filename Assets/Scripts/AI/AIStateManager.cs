using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class AIStateManager : MonoBehaviour
{
    [Header("Base Component")]
    [SerializeField] private NavMeshAgent _agentAI;

    [Header("Modules")]
    [SerializeField] private AIMoveModule _moveModule;
    [SerializeField] private AIHealModule _healModule;

    [Header("States")]
    [SerializeField] private StatePatrolling _statePatroll;
    [SerializeField] private StateFight _stateFight;
    [SerializeField] private StateDefeet _stateDefeet;

    private AbstractState _currentState;

    private void Start()
    {
        _currentState = _statePatroll;
    }

    public void Update()
    {
        _currentState.DoStateLogic();
    }

    private float ThresholdMotivationMakingDecision = 0f;

    private void SwapState(AbstractState state)
    {
        if (state == _currentState)
            return;

        switch (state)
        {
            case StatePatrolling:
                _currentState = state;
                break;
            case StateFight:
                _currentState = state;
                break;
            case StateDefeet:
                _currentState = state;
                break;
            default:
                break;
        }
    }
}