using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoCameraScript : CameraScript
{

    protected override void AnimateCamera()
    {
        // pass
    }

    protected override void OnMouseClick(InputAction.CallbackContext context)
    {
        // TODO
    }

    protected override void OnMouseDelta(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            transform.RotateAround(_focusTarget.position, Vector3.up, delta.x * _SENSITIVITY * Time.deltaTime);
            transform.RotateAround(_focusTarget.position, transform.right, delta.y * _SENSITIVITY  * Time.deltaTime * GetScale());
            
            transform.eulerAngles = transform.eulerAngles;

            transform.LookAt(_focusTarget);
        }
    }

    float GetScale() // 50 and 115
    {
        //var c = Vector3.Angle(_focusTarget.up, transform.position - _focusTarget.position);

        //return Mathf.Pow(Mathf.Clamp((c - 50f) / (115f - 50f), 0f, 1f), 2f);

        return Mathf.Pow(1f - Mathf.Abs(Vector3.Dot(_focusTarget.up, (transform.position - _focusTarget.position).normalized)), 2f);
    }

    // idk if this even needs to be a thing i can override
    // maybe add a lerp in here?
    protected override void SetFocusTarget(Transform t)
    {
        var cache = _focusTarget;

        _focusTarget = t == null ? _priorTarget : t;

        _priorTarget = cache;

        transform.LookAt(_focusTarget);
    }

    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        PersistentHook();

        GameObject g = GameObject.FindGameObjectWithTag("Player");

        SetFocusTarget(g.transform);
    }
    
}
