using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSIdle : TwoBaseState
{
    public TSIdle(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) { }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        if (c.started) // if pressed, go to Move
        {
            ChangeState(TwoState.Move);
        }
    }

    public override void HandleButton1(InputAction.CallbackContext c)
    {
        if (c.started) // if pressed, add a "force" then go to Rising
        {
            _yvelo = _JUMP_FORCE;

            ProcessMovement(); // this "should" be removed because it's doubling a calculation
                               // this might make jumping from Idle slightly higher than from Move, but whatever.

            ChangeState(TwoState.Rising);
        }
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
        if (CastRay()) // get Ray info
        {
            onMovingPlatform = r.collider.tag == "MovingObject"; // are we on a moving object?

            if (onMovingPlatform) // if yes, stick to it
            {
                moving_body = r.transform;

                if (!_cc.transform.IsChildOf(moving_body))
                {
                    _cc.transform.SetParent(moving_body, true); // people say this is bad; why?
                }
            }

            else // if no, then remove all children from the moving object (remove the player, essentially)
            {
                if (moving_body != null)
                {
                    moving_body.DetachChildren(); // this could come back to bite me if we add some weird level decals

                    moving_body = null;
                }
            }
        }
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
