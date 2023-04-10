using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Union class for Rising, Falling, and Walljump (eventually) since they share many things
/// </summary>
public abstract class TSBaseAirborneState : TwoBaseState
{
    protected Action<Vector3, float, bool> forceMethod;

    RaycastHit wall_info;

    int air_jump_count = 0;

    public TSBaseAirborneState(CharacterController c, Transform t, Action<TwoState> cf, Action<Vector3, float, bool> f) : base(c, t, cf)
    {
        forceMethod = f;
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
        if (c.started)
        {
            if (IfDoWallJump()) // not nested bc then the ray would shoot 3 times for every one click
            {
                ChangeState(TwoState.Rising); // IfDoWallJump() "resets" falling velocity by adding to it.
            }
        }
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

    public override void HandleStanceInput(InputAction.CallbackContext c)
    {
        // pass
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
        air_jump_count = 0;
    }

    protected bool IfDoWallJump()
    {
        Transform player = _cc.transform;

        bool success = Physics.Raycast(player.position + Vector3.up, player.forward, out wall_info, 2f, _LAYER_MASK);

        if (success)
        {
            air_jump_count++;

            Vector3 direction = wall_info.normal;
            direction.y = 0f; // flatten into the XZ plane

            forceMethod.Invoke(direction.normalized, _WALLKICK_FORCE * 3f, false);

            // reduce jump height over the number of jumps we made
            _yvelo += _WALLKICK_FORCE / (air_jump_count * _AIRJUMP_DRAG); 

            player.transform.LookAt(direction.normalized + player.transform.position);
        }

        return success;
    }
}
