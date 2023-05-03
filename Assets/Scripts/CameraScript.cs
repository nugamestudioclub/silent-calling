using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CameraScript : PossessableObject
{
    // hey dustin use this thing

    protected RaycastHit _info; // raycast information about the ray from focus to camera

    protected Transform _focusTarget, _priorTarget; // who the camera is focusing on
    protected bool _isCameraLocked = false; // is the camera in a locked position?
    [SerializeField] protected const float _DISTANCE = 10f; // how far should the camera be from the focus?
    protected const float _MDIST_LERP = 0.0125f; // how much the Lead lerps to obey distance
    [SerializeField] protected readonly float _SENSITIVITY =  20f; // brub
                                                                   // protected ??? _cameraState = ???;

    #region Basic Hooks

    protected override void Free()
    {
        Debug.Log("free: " + this.name);
        INS.MouseClickBind -= OnMouseClick;
        INS.MouseDeltaBind -= OnMouseDelta;
    }

    protected override void Hook()
    {
        Debug.Log("hook: " + this.name);
        INS.MouseClickBind += OnMouseClick;
        INS.MouseDeltaBind += OnMouseDelta;
    }
    #endregion

    // declaring abstracts

    // Sets the focus to input, but also can do other things (such as lerp to the new focus)
    protected abstract void SetFocusTarget(Transform t);
    // bruh
    protected abstract void OnMouseDelta(InputAction.CallbackContext context);
    // bruh
    protected abstract void OnMouseClick(InputAction.CallbackContext context);

    protected abstract void Start();

    protected abstract void Update();

}
