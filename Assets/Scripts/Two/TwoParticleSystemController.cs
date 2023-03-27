using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwoSMScript))]
public class TwoParticleSystemController : MonoBehaviour
{
    public ParticleSystem OnLand, OnRun, OnJump;

    private ParticleSystem Emitting 
    {
        get
        {
            return _emitting;
        }
        set
        {
            _emitting = value;

            _emitting.Play();
        }
    }

    private ParticleSystem _emitting;

    private void OnEnable()
    {
        TwoSMScript tsm = GetComponent<TwoSMScript>();

        tsm.OnStateChanged += ChangeSystem;

        _emitting = OnLand;
    }

    private void OnDisable()
    {
        TwoSMScript tsm = GetComponent<TwoSMScript>();

        tsm.OnStateChanged -= ChangeSystem;
    }

    void ChangeSystem(TwoBaseState state)
    {
        Emitting.Stop();

        if(state.PastState == TwoState.Falling && (state.StateType == TwoState.Idle || state.StateType == TwoState.Move || state.StateType == TwoState.Running))
        {
            Emitting = OnLand;
        }

        else if (state.StateType == TwoState.Running)
        {
            Emitting = OnRun;
        }

        else if (state.StateType == TwoState.Rising)
        {
            Emitting = OnJump;
        }
    }
}
