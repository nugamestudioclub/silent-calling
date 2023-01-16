using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSRising : TSBaseAirborneState
{
    float timeStarted;

    public TSRising(CharacterController c, Transform t, Action<TwoState> a, Action<Vector3, float, bool> f) : base(c, t, a, f)
    {
        StateType = TwoState.Rising;
    }

    public override void PhysicsProcess()
    {
        _yvelo += _GRAVITY * Time.deltaTime * timeStarted; // "exponentially" increase the gravity so
                                                // we fall faster. This feels better to use. MAKE SURE THIS IS UNSCALED WITH FRAMERATE
                                                // deltaTime is done in ProcessMovement

        if (_yvelo < 0f)
        {
            ChangeState(TwoState.Falling);

            return;
        }

        timeStarted += timeStarted * _DECEL * Time.deltaTime * 50f; // see TwoSMScript

        ProcessMovement();
    }

    public override void StateStart()
    {
        timeStarted = 1f;
    }
}
