using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoCameraScript : CameraScript
{
    Transform _lead;
    const float _LERP = 0.0125f;
    const float _ACCURACY = 0.95f;

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

            _lead.RotateAround(_focusTarget.position, Vector3.up, delta.x * _SENSITIVITY * Time.deltaTime);
            _lead.RotateAround(_focusTarget.position, _lead.right, delta.y * _SENSITIVITY  * Time.deltaTime * GetScale());

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

        GameObject g2 = new GameObject();
        g2.name = "Camera Lead";
        _lead = g2.transform;
        _lead.SetPositionAndRotation(transform.position, transform.rotation);
    }


    protected override void Update()
    {
        base.Update();

        if (true) // not always, do Dist or Dot or something
        {
            transform.position = Vector3.Lerp
                (transform.position,
                _lead.position,
                _LERP);

            transform.rotation = Quaternion.Lerp(transform.rotation, _lead.rotation, _LERP);

            transform.LookAt(_focusTarget);
        }
    }
}
