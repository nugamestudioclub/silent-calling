using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StanceCamera : ACameraState
{
    public override void CameraFocusTarget(Transform t)
    {
        // not sure what to put here.
    }

    public override void CameraMouseClicked(InputAction.CallbackContext context)
    {
        // pass?
    }

    public override void CameraMouseDelta(InputAction.CallbackContext context)
    {
        // rotate camera accordingly, spear will follow
    }

    public override void StateLateUpdate()
    {
        // pass
    }

    public override void StateStart()
    {
        // pass
    }

    public override void StateUpdate()
    {
        // maintain distance from Two's shoulder.
    }

    protected override void CameraAnimation()
    {
        // pass
    }

    protected override void CameraMaintainDistance()
    {
        // rotate around Two's shoulder area.
    }
}
