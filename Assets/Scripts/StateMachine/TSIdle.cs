using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSIdle : TwoBaseState
{
    public TSIdle(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) { }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            ChangeState(TwoState.Move);
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
        // pass
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
