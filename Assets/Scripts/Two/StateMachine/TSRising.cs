using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSRising : TwoBaseState
{
    float timeStarted;

    public TSRising(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a)
    {
        StateType = TwoState.Rising;
    }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _currentInput = c.ReadValue<Vector2>();
        }

        else if (c.canceled)
        {
            _currentInput = Vector2.zero;
        }
    }

    public override void HandleButton1(InputAction.CallbackContext c)
    {
        // pass
    }
    
    public override void HandleButton2(InputAction.CallbackContext c)
    {
        // pass
    }

    public override void HandleButton3(InputAction.CallbackContext c)
    {
        if (c.canceled)
        {
            _running = false;
        }
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

        timeStarted += timeStarted * _DECEL * Time.deltaTime * 25f; // see TwoSMScript

        ProcessMovement();
    }

    public override void UpdateState(TwoBaseState b)
    {
        base.UpdateState(b);

        _running = b.IsRunning();
    }

    public override void StateStart()
    {
        timeStarted = 1f;
    }

    protected override void StateEnd()
    {
        // pass
    }
}
