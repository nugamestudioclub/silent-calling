using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoCameraScript : CameraScript
{
    Transform _lead;
    const float _LERP = 0.25f;
    const float _HIGH_ANGLE = 50f;
    const float _LOW_ANGLE = 115f;
    const float _CORRECTION = 0.1f;
    const float _CHARACTER_HEIGHT = 2.3f; // not a great solution, but it works. // + Vector3.up * _CHARACTER_HEIGHT

    const int _LAYER_MASK = ~(1 << 2); // look this up if you wanna know what this does :P

    Vector3 prior_pos = Vector3.zero;

    // oh god im too lazy to comment this
    // dont even wanna read it either

    protected override void AnimateCamera()
    {
        // pass
    }

    protected override void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _lead.position = GetAdjustedPosition() - _DISTANCE * _focusTarget.forward + Vector3.up * 1.5f;
            _lead.LookAt(_focusTarget);
        }
    }

    protected override void OnMouseDelta(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            Vector3 cache_pos = GetAdjustedPosition();

            _lead.RotateAround(cache_pos, Vector3.up, delta.x * _SENSITIVITY * Time.deltaTime);

            float angle = Vector3.Angle(Vector3.up, _lead.position - cache_pos);

            if (angle < _HIGH_ANGLE || angle > _LOW_ANGLE)
            {
                Vector3 l = _lead.transform.position;
                l.y -= angle > _LOW_ANGLE ? -_CORRECTION : _CORRECTION; // this is bad and jittery, fix later.

                _lead.transform.position = l;
            }

            else
            {
                _lead.RotateAround(cache_pos, _lead.right, delta.y * _SENSITIVITY * Time.deltaTime); // * GetScale()
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

    Vector3 GetAdjustedPosition()
    {
        return _focusTarget.position + Vector3.up * _CHARACTER_HEIGHT;
    }

    protected override void MaintainDistance()
    {
        Vector3 cache_pos = GetAdjustedPosition();

        _lead.position += cache_pos - prior_pos; // hacky solution but it works
                                                 // moves the lead by the delta of the focus, to keep it aligned.
        prior_pos = cache_pos;


        Vector3 t_pos = _lead.position;

        float dist = Vector3.Distance(_lead.position, cache_pos);

        if (Physics.Raycast(cache_pos, t_pos - cache_pos, out _info, _DISTANCE, _LAYER_MASK)) // if obstructed
        {
            _lead.position = Vector3.Lerp(t_pos, _info.point, 0.5f);
        }
        
        else if (dist < _DISTANCE) // else if too close
        {
            _lead.position = Vector3.Lerp(t_pos, t_pos - _lead.forward * _DISTANCE, _MDIST_LERP * 4f);
        }
        
        else // else (too far)
        {
            _lead.position = Vector3.Lerp(t_pos, cache_pos - _lead.forward * _DISTANCE, _MDIST_LERP * Mathf.Clamp(dist, 0f, 40f));
        }

        _lead.LookAt(GetAdjustedPosition());
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
        MaintainDistance(); // could lighten the calls to this by hooking it into an OnMoved/OnCameraMoved event but whatevs
                            // I mean I could listen to "if the prior_pos Delta is big enough or camera moved, fire the event"
                            // seems like a bit much tho

        // used to have this in a conditional, but SLerp and Quaterion Lerp are so cheap I dont care.
        transform.SetPositionAndRotation(
            Vector3.Slerp(transform.position, _lead.position, _LERP), 
            Quaternion.Lerp(transform.rotation, _lead.rotation, _LERP));
    }
}
