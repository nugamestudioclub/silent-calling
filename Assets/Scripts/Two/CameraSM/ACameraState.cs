using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ACameraState
{
    protected Action<TwoState> _ChangeStateAction;

    public ACameraState(Action<TwoState> action)
    {
        _ChangeStateAction = action;
    }

    protected abstract void CameraAnimation();

    public abstract void CameraMouseClicked(InputAction.CallbackContext context);

    public abstract void CameraMouseDelta(InputAction.CallbackContext context);

    public abstract void CameraFocusTarget(Transform t);

    protected abstract void CameraMaintainDistance();

    public abstract void StateStart();

    public abstract void StateUpdate();

    public abstract void StateLateUpdate();

    public void ChangeCameraState(TwoBaseState state)
    {
        _ChangeStateAction.Invoke(state.StateType);
    }
}
