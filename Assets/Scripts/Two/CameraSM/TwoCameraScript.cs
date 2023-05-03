using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// The big idea with this script is that the camera isn't directly controlled by MouseDelta. Rather, the position of a gameobject
/// called the LEAD (or _lead) is modified. The camera simply always lerps towards the LEAD, resulting in smooth camera movement.
/// 
/// This class is due to be reworked because of many logical simplications there are.
/// </summary>
public class TwoCameraScript : CameraScript
{
    ACameraState currentState;

    Dictionary<TwoState, ACameraState> map;

    TwoSMScript tsm;

    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor, basic camera stuff

        map = new Dictionary<TwoState, ACameraState>();

        tsm = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<TwoSMScript>();

        ACameraState bs = new BasicCamera(ChangeState);
        tsm.OnStateChanged += bs.ChangeCameraState;

        ACameraState sc = new StanceCamera(ChangeState);
        tsm.OnStateChanged += sc.ChangeCameraState;
        
        map.Add(TwoState.Idle, bs);
        map.Add(TwoState.Move, bs);
        map.Add(TwoState.Stance, sc);

        currentState = map[TwoState.Idle];
        currentState.StateStart();
    }
    
    protected override void OnDisable()
    {
        #if UNITY_EDITOR
        if (INS == null)
        {
            Debug.LogWarning("If you see this message when not exiting playmode, then you are missing an INS!");

            return;
        }

        Debug.Log(string.Format("Links for {0} were decoupled w/o race condition error.", name));
        #endif

        Free();

        ACameraState bs = map[TwoState.Idle];
        tsm.OnStateChanged -= bs.ChangeCameraState;

        ACameraState sc = map[TwoState.Stance];
        tsm.OnStateChanged -= sc.ChangeCameraState;
    }

    public void ChangeState(TwoState state)
    {
        if (state != TwoState.Idle
            && state != TwoState.Move
            && state != TwoState.Stance)
        {
            return;
        }

        currentState = map[state];

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
