using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CameraScript : PossessableObject
{
    protected Transform _focusTarget, _priorTarget;
    protected bool _isCameraLocked = false;
    [SerializeField] protected const float _DISTANCE = 10f;
    [SerializeField] protected readonly float _SENSITIVITY =  40f;
    // protected ??? _cameraState = ???;


    const float _MDIST_LERP = 0.0125f;
    RaycastHit _info;

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

    /// <summary>
    /// Placed in Update, this function keeps the camera at a distance from the player
    /// unless the view is obstructed.
    /// </summary>
    protected void MaintainDistance()
    {
        Vector3 t_pos = transform.position;

        float dist = Vector3.Distance(transform.position, _focusTarget.position);

        if (Physics.Raycast(_focusTarget.position, t_pos - _focusTarget.position, out _info, _DISTANCE)) // if obstructed
        {
            /*transform.position = (_info.collider.tag != "Player") ?
                t_pos + transform.forward * _info.distance
                :
                Vector3.Lerp(t_pos, t_pos - transform.forward * (_DISTANCE - _info.distance), _MDIST_LERP);*/
            
            transform.position = Vector3.Lerp(t_pos, _info.point , _MDIST_LERP * 2f);
        }

        else if (dist < _DISTANCE) // else if too close
        {
            transform.position = Vector3.Lerp(t_pos, t_pos - transform.forward * _DISTANCE, _MDIST_LERP * 4f);
        }

        else // else (too far)
        {
            transform.position = Vector3.Lerp(t_pos, _focusTarget.position - transform.forward * _DISTANCE, _MDIST_LERP * Mathf.Clamp(dist, 0f, 40f));
        }
    }

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
