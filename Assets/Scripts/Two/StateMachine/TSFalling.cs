using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSFalling : TwoBaseState
{
    public TSFalling(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) 
    {
        StateType = TwoState.Falling;
    }

    // TODO: slower air movement???

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
        // sets the yvelo to either Max Fall Speed, or that monstrosity.
        // lol it's not even that bad, just adding a bit of gravity + increasing it by a multiplier.
        _yvelo = Mathf.Max(_yvelo + _FALL_MULTIPLIER * _GRAVITY * Time.deltaTime, _MAX_FALL_SPEED);  // deltaTime is already done in ProcessMovement

        if (_cc.isGrounded)
        {
            // do we hit the ground idle or running? Depends on the input in the air.
            ChangeState(_currentInput == Vector2.zero ? TwoState.Idle : TwoState.Move);

            return;
        }

        ProcessMovement();
    }

    public override void UpdateState(TwoBaseState b)
    {
        base.UpdateState(b);

        _running = b.IsRunning();
    }

    public override void StateStart()
    {
        // pass
    }

    protected override void StateEnd()
    {
        // pass
    }
}
