using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoCameraScript : CameraScript
{
    Transform _lead;
    const float _LERP = 0.05f;
    const float _ACCURACY = 0.95f;
    const float _HIGH_ANGLE = 50f;
    const float _LOW_ANGLE = 115f;
    const float _CORRECTION = 0.1f;

    Vector3 prior_pos = Vector3.zero;

    protected override void AnimateCamera()
    {
        // pass
    }

    protected override void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _lead.position = _focusTarget.position - _DISTANCE * _focusTarget.forward + Vector3.up * 1.5f;
            _lead.LookAt(_focusTarget);
        }
    }

    protected override void OnMouseDelta(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            _lead.RotateAround(_focusTarget.position, Vector3.up, delta.x * _SENSITIVITY * Time.deltaTime);

            float angle = Vector3.Angle(Vector3.up, _lead.position - _focusTarget.position);

            if (angle < _HIGH_ANGLE || angle > _LOW_ANGLE)
            {
                Vector3 l = _lead.transform.position;
                l.y -= angle > _LOW_ANGLE ? -_CORRECTION : _CORRECTION; // this is bad and jittery, fix later.

                _lead.transform.position = l;
            }

            else
            {
                _lead.RotateAround(_focusTarget.position, _lead.right, delta.y * _SENSITIVITY * Time.deltaTime); // * GetScale()
            }

        }
    }

    /*
    float GetScale() // 50 and 115
    {
        //var c = Vector3.Angle(_focusTarget.up, transform.position - _focusTarget.position);

        //return Mathf.Pow(Mathf.Clamp((c - 50f) / (115f - 50f), 0f, 1f), 2f);

        return Mathf.Pow(1f - Mathf.Abs(Vector3.Dot(_focusTarget.up, (transform.position - _focusTarget.position).normalized)), 2f);
    }
    */

    // idk if this even needs to be a thing i can override
    // maybe add a lerp in here?
    protected override void SetFocusTarget(Transform t)
    {
        var cache = _focusTarget;

        _focusTarget = t == null ? _priorTarget : t;

        _priorTarget = cache;

        transform.LookAt(_focusTarget);
    }

    protected override void MaintainDistance()
    {
        _lead.position += _focusTarget.position - prior_pos; // hacky solution but it works

        prior_pos = _focusTarget.position;


        Vector3 t_pos = _lead.position;

        float dist = Vector3.Distance(_lead.position, _focusTarget.position);

        if (Physics.Raycast(_focusTarget.position, t_pos - _focusTarget.position, out _info, _DISTANCE)) // if obstructed
        {
            _lead.position = Vector3.Lerp(t_pos, _info.point, 0.5f);
        }

        else if (dist < _DISTANCE) // else if too close
        {
            _lead.position = Vector3.Lerp(t_pos, t_pos - _lead.forward * _DISTANCE, _MDIST_LERP * 4f);
        }

        else // else (too far)
        {
            _lead.position = Vector3.Lerp(t_pos, _focusTarget.position - _lead.forward * _DISTANCE, _MDIST_LERP * Mathf.Clamp(dist, 0f, 40f));
        }

        _lead.LookAt(_focusTarget);
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

        prior_pos = g.transform.position;
    }


    protected override void Update()
    {
        base.Update();

        if (true) // not always, do Dist or Dot or something
        {
            transform.position = Vector3.Slerp
                (transform.position,
                _lead.position,
                _LERP);

            transform.rotation = Quaternion.Lerp(transform.rotation, _lead.rotation, _LERP);
        }
    }
}
