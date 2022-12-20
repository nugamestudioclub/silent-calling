using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimHandlerScript : MonoBehaviour
{
    Animator _animator;
    int _parameterHash;

    int _currentState = 0;
    private int CurrentState
    {
        set
        {
            if (value != _currentState)
            {
                _currentState = value;
                _animator.SetInteger(_parameterHash, value);
            }
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _parameterHash = Animator.StringToHash("State");

        CurrentState = 0;
    }

    void OnEnable()
    {
        TwoSMScript tsms = GetComponent<TwoSMScript>();

        tsms.OnStateChanged += UpdateAnimator;
    }

    void OnDisable()
    {
        TwoSMScript tsms = GetComponent<TwoSMScript>();

        tsms.OnStateChanged -= UpdateAnimator;
    }

    void UpdateAnimator(TwoBaseState t)
    {
        CurrentState = (int)t.StateType;
    }

}
