using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The big idea with this script is that the camera isn't directly controlled by MouseDelta. Rather, the position of a gameobject
/// called the LEAD (or _lead) is modified. The camera simply always lerps towards the LEAD, resulting in smooth camera movement.
/// 
/// This class is due to be reworked because of many logical simplications there are.
/// </summary>
public class TwoCameraScript : CameraScript
{
    ACameraState currentState;

    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor, basic camera stuff

        PersistentHook();

        currentState = new BasicCamera();
        currentState.StateStart();
    }

    // If clicked, reorient the camera to behind Two
    protected override void OnMouseClick(InputAction.CallbackContext context)
    {
        currentState.CameraMouseClicked(context);
    }

    // on Delta, move the LEAD around the focus position, keeping it within bounds of the angles
    protected override void OnMouseDelta(InputAction.CallbackContext context)
    {
        currentState.CameraMouseDelta(context);
    }

    // idk if this even needs to be a thing i can override
    // maybe add a lerp in here?
    protected override void SetFocusTarget(Transform t)
    {
        currentState.CameraFocusTarget(t);
    }

    protected override void Update()
    {
        currentState.StateUpdate();
    }

    void LateUpdate() // changed from Update bc of camera snapping
    {
        currentState.StateLateUpdate();
    }
 
}
