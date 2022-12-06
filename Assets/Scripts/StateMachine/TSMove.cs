using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSMove : TwoBaseState
{
    public TSMove(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) { }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _currentInput = c.ReadValue<Vector2>();
        }

        else if (c.canceled)
        {
            _currentInput = Vector2.zero;

            ChangeState(TwoState.Idle);
        }
    }

    public override void HandleButton1(InputAction.CallbackContext c)
    {
        Debug.Log("jump");
    }

    public override void HandleButton2(InputAction.CallbackContext c)
    {
        Debug.Log("use");
    }

    public override void HandleButton3(InputAction.CallbackContext c)
    {
        Debug.Log("extra");
    }

    public override void InUpdate()
    {
        if (_currentInput != Vector2.zero)
        {
            Vector3 v = new Vector3(_currentInput.x, 0, _currentInput.y);

            v = _cameraTransform.TransformDirection(v);

            v.y = 0f;
            Debug.Log(v);
            _cc.SimpleMove(v.normalized * 5f);
        }
    }

    protected override void StateEnd()
    {
        // pass
    }

    protected override void StateStart()
    {
        // pass
    }
}
