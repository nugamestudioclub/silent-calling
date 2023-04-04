using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ACameraState
{
    protected abstract void CameraAnimation();

    public abstract void CameraMouseClicked(InputAction.CallbackContext context);

    public abstract void CameraMouseDelta(InputAction.CallbackContext context);

    public abstract void CameraFocusTarget(Transform t);

    protected abstract void CameraMaintainDistance();

    public abstract void StateStart();

    public abstract void StateUpdate();

    public abstract void StateLateUpdate();
}
