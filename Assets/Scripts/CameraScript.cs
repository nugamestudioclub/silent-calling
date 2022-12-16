using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CameraScript : PossessableObject
{
    // hey dustin use this thing

    protected RaycastHit _info;

    protected Transform _focusTarget, _priorTarget;
    protected bool _isCameraLocked = false;
    [SerializeField] protected const float _DISTANCE = 10f;
    protected const float _MDIST_LERP = 0.0125f;
    [SerializeField] protected readonly float _SENSITIVITY =  20f;
    // protected ??? _cameraState = ???;

    protected override void Free()
    {
        INS.MouseClickBind -= OnMouseClick;
        INS.MouseDeltaBind -= OnMouseDelta;
    }

    protected override void FreePersistentHook()
    {
        INS.EmptyBind -= Hook;
    }

    protected override void Hook()
    {
        INS.MouseClickBind += OnMouseClick;
        INS.MouseDeltaBind += OnMouseDelta;
    }

    protected override void PersistentHook()
    {
        INS.EmptyBind += Hook;
    }

    protected abstract void MaintainDistance();

    protected abstract void SetFocusTarget(Transform t);

    protected abstract void OnMouseDelta(InputAction.CallbackContext context);

    protected abstract void OnMouseClick(InputAction.CallbackContext context);

    protected abstract void AnimateCamera();

    protected abstract void Start();

    protected virtual void Update()
    {
        MaintainDistance();
    }

}
