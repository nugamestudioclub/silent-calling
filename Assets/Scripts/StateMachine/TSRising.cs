using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSRising : TwoBaseState
{
    float timeStarted;

    public TSRising(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a){ }

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
        // pass
    }

    public override void InUpdate()
    {
        _yvelo += _GRAVITY * Time.deltaTime * (1 + timeStarted); // "exponentially" increase the gravity so
                                                                 // we fall faster. This feels better to use.

        if (_yvelo < 0f)
        {
            ChangeState(TwoState.Falling);

            return;
        }

        timeStarted += _DECEL; // see TwoSMScript

        ProcessMovement();
    }

    public override void StateStart()
    {
        timeStarted = 0f;
    }

    protected override void StateEnd()
    {
        // pass
    }
}
