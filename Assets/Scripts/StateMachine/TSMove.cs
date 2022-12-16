using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSMove : TwoBaseState
{
    float t = 0f;

    public TSMove(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a) { }

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

    // returns true if we are on a slope with an angle greater than the slope limit.
    bool OnIllegalSlope()
    {
        return !(Vector3.Angle(Vector3.up, r.normal) < slopeLimit);
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
        Vector3 input_v = new Vector3(_currentInput.x, 0, _currentInput.y); // input vector
        Vector3 v = _cameraTransform.TransformDirection(input_v); // align with camera
        v.y = 0f; // remove any y that comes from the alignment

        // re-orient dir vector to the normal of the slope so we dont bounce
        if (CastRay())
        {
            Quaternion dir_rotation = Quaternion.FromToRotation(Vector3.up, r.normal); // Quat stuff

            Vector3 v2 = dir_rotation * v; // reorient the vector

            v = v2.y < 0f ? v2 : v; // only if we're going down a slope tho; up is perfectly fine
        }

        v.y += _yvelo; // add "gravity" and any rising "force" we add
        
        _cc.Move(_MOVE_SPEED * Time.deltaTime * v);

        _cc.transform.rotation = Quaternion.LookRotation(input_v); // look in the direction of movement
    }

    public override void InUpdate()
    {
        if (!_cc.isGrounded) // tick the coyote timer if we aren't on the ground
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

        if (r.collider != null)
        {
            onMovingPlatform = r.transform.CompareTag("MovingObject"); // hey everybody we on a moving plat now lol
        }

        if (onMovingPlatform) // see TSIdle
        {
            moving_body = r.transform;

            if (!_cc.transform.IsChildOf(moving_body))
            {
                _cc.transform.SetParent(moving_body, true);
            }
        }

        else
        {
            if (moving_body != null)
            {
                moving_body.DetachChildren();

                moving_body = null;
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
        slopeLimit = _cc.slopeLimit;
    }

    protected override void StateEnd()
    {
        // pass
    }
}
