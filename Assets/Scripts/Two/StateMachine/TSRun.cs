using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSRun : TSMove
{
    // identical to TSMove, but running
    // god i need to do a lot of refactoring
    // there is a lot of repeat code

    public TSRun(CharacterController c, Transform t, Action<TwoState> a) : base(c, t, a)
    {
        StateType = TwoState.Running;
    }

    public override void HandleButton3(InputAction.CallbackContext c)
    {
        if (c.canceled)
        {
            ChangeState(TwoState.Move);

            _running = false;
        }
    }

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

        _cc.Move(_MOVE_SPEED * Time.deltaTime * v * _RUN_SPEED_MULTIPLIER);
    }

    public override void StateStart()
    {
        _running = true;
    }
}
