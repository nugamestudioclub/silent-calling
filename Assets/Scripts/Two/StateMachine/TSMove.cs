using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSMove : TwoBaseState
{
    float t = 0f;

    // TODO
    // Slide-state for when on an illegal slope and not moving
    // Fix Airborne State transition when walking into an illegal slope

    public TSMove(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) 
    {
        StateType = TwoState.Move;
    }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        // basic read-value stuff

        if (c.performed)
        {
            _currentInput = c.ReadValue<Vector2>();
        }

        else if (c.canceled)
        {
            _currentInput = Vector2.zero;

            ChangeState(TwoState.Idle); // if no input, therefore idle.
        }
    }

    public override void HandleButton1(InputAction.CallbackContext c)
    {
        if (c.started && !OnIllegalSlope()) // if input was started, and we are on a surface that we can jump on, jump
        {
            _yvelo = _JUMP_FORCE;

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

    // hoo boy
    protected override void ProcessMovement()
    {
        Vector3 v = new Vector3(_currentInput.x, 0, _currentInput.y); // input vector
        v = _cameraTransform.TransformDirection(v); // re-orient input vector to Camera space
        v.y = 0f; // remove any y that comes from the alignment

        // unsmoothened variant
        // _cc.transform.rotation = Quaternion.LookRotation(v); // look in the direction of movement

        // smoothened
        // look in the direction of movement, but smoothly. Can result in intermittent angles if control
        // is released before the Lerp completes
        _cc.transform.rotation = Quaternion.Lerp( 
            _cc.transform.rotation, 
            Quaternion.LookRotation(v),
            _ROTATION_LERP * Time.deltaTime);

        // re-orient dir vector to the normal of the slope so we dont bounce
        if (CastRay())
        {
            Quaternion dir_rotation = Quaternion.FromToRotation(Vector3.up, _raycastInfo.normal); // Quat stuff

            Vector3 v2 = dir_rotation * v; // reorient the vector

            v = v2.y < 0f ? v2 : v; // only if we're going down a slope tho; up is perfectly fine
        }

        v.y += _yvelo; // add "gravity" and any rising "force" we add
        
        _cc.Move(_MOVE_SPEED * Time.deltaTime * v);
    }

    public override void InUpdate()
    {
        // tick the coyote timer if we aren't on the ground, but also if we aren't on an illegal slope
        // The reason for the latter is bc isGrounded is false on an illegal slope, but we are grounded there.
        // We just can't jump or move up the slope bc we aren't supposed to be on it.
        if (!_cc.isGrounded && !OnIllegalSlope())
        {
            t++;
        }

        if (t > _COYOTE_TIME) // if timer is up, fall
        {
            ChangeState(TwoState.Falling);

            return;
        }

        // there's a bug with this that occurs when you hang between the side of a moving object and a static one
        // like there's a gap between them that's big enough to fit in but not big enough to fall completely through
        // you go in the gap and try to jump back onto the platform and it errors here @ line 105
        //
        //      if (!_cc.transform.IsChildOf(moving_body))
        //          >> ArgumentNullException: Value cannot be null.
        //          >> Parameter name: parent
        //
        // you have to touch the platform first
        // is this ignorable?

        if (_raycastInfo.collider != null)
        {
            _onMovingPlatform = _raycastInfo.transform.CompareTag("MovingObject"); // hey everybody we on a moving plat now lol
        }

        if (_onMovingPlatform) // see TSIdle
        {
            _movingBody = _raycastInfo.transform;

            if (!_cc.transform.IsChildOf(_movingBody))
            {
                _cc.transform.SetParent(_movingBody, true);
            }
        }

        else
        {
            if (_movingBody != null)
            {
                _movingBody.DetachChildren();

                _movingBody = null;
            }
        }

        if (_currentInput != Vector2.zero) // if we are moving, do move stuff
        {
            _yvelo = _GRAVITY * Time.deltaTime; // add gravity. :( why do i have to do this

            ProcessMovement();
        }

        else
        {
            ChangeState(TwoState.Idle); // just realized Im doubling up on this from Handle2DMovement
                                        // sucks, I guess.
        }
    }

    public override void StateStart()
    {
        _yvelo = 0f;
        t = 0f;
    }

    protected override void StateEnd()
    {
        // pass
    }
}
