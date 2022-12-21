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
    Transform _lead;
    const float _LERP = 0.25f; // how fast does the camera lerp towards Lead
    const float _HIGH_ANGLE = 50f; // what's our highest angle that the camera cant rotate past
    const float _LOW_ANGLE = 115f; // what's our lowest angle that the camera cant rotate past
    const float _CORRECTION = 0.1f; // if we're at the angle max, how much do we correct the camera's y by?
    const float _CHARACTER_HEIGHT = 2.3f; // not a great solution, but it works. // + Vector3.up * _CHARACTER_HEIGHT

    const int _LAYER_MASK = ~(1 << 2); // look this up if you wanna know what this does :P

    Vector3 prior_pos = Vector3.zero;


    protected override void AnimateCamera()
    {
        // pass
    }

    // If clicked, reorient the camera to behind Two
    protected override void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            // orients the Lead right behind player, and faces towards them instantly
            _lead.position = GetAdjustedPosition() - _DISTANCE * _focusTarget.forward + Vector3.up * _CHARACTER_HEIGHT;
            _lead.LookAt(_focusTarget);
        }
    }

    // on Delta, move the LEAD around the focus position, keeping it within bounds of the angles
    protected override void OnMouseDelta(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            Vector3 cache_pos = GetAdjustedPosition();

            // apply the x and y rots separately because only the y is limited.
            // RotateAround is what gives LEAD its spherical rotation. Look at the documentation to learn more about how it does this.
            _lead.RotateAround(cache_pos, Vector3.up, delta.x * _SENSITIVITY * Time.deltaTime);

            float angle = Vector3.Angle(Vector3.up, _lead.position - cache_pos); // calculate angle between World Up and vector from focus to lead.

            // if angle is bad, correct lead's position
            if (angle < _HIGH_ANGLE || angle > _LOW_ANGLE)
            {
                Vector3 l = _lead.transform.position;
                l.y -= angle > _LOW_ANGLE ? -_CORRECTION : _CORRECTION; // this is bad and jittery, fix later.

                _lead.transform.position = l;
            }
            else // otherwise just rotate around
            {
                _lead.RotateAround(cache_pos, _lead.right, delta.y * _SENSITIVITY * Time.deltaTime); 
            }
        }
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

    // Returns the adjusted focusTarget position with character height added. This makes the
    // camera not focus on Two's feet :>
    // might also cause bugs? kinda hard to tell.
    Vector3 GetAdjustedPosition()
    {
        return _focusTarget.position + Vector3.up * _CHARACTER_HEIGHT;
    }

    // Here it is- the big one.
    protected override void MaintainDistance()
    {
        //--- This block ensures the lead always moves with the player
        Vector3 cache_pos = GetAdjustedPosition();

        _lead.position += cache_pos - prior_pos; // hacky solution but it works
                                                 // moves the lead by the delta of the focus, to keep it aligned.
        prior_pos = cache_pos;
        //---

        //--- This block is the primary behavior that retains distance in 3 cases
        Vector3 t_pos = _lead.position;

        float dist = Vector3.Distance(_lead.position, cache_pos);

        if (Physics.Raycast(cache_pos, t_pos - cache_pos, out _info, _DISTANCE, _LAYER_MASK)) // if camera view of Two is obstructed...
        {
            _lead.position = Vector3.Lerp(t_pos, _info.point, 0.5f); // lerp to the point that the obstruction starts from
        }
        
        else if (dist < _DISTANCE) // else if lead is too close...
        {
            _lead.position = Vector3.Lerp(t_pos, t_pos - _lead.forward * _DISTANCE, _MDIST_LERP * 4f); // lerp towards the proper distance away
        }
        
        else // else (too far)
        {
            // exact opposite of above
            // however, strength of lerp intensifies based on distance away
            _lead.position = Vector3.Lerp(t_pos, cache_pos - _lead.forward * _DISTANCE, _MDIST_LERP * Mathf.Clamp(dist, 0f, 40f));
        }

        _lead.LookAt(GetAdjustedPosition()); // orient LEAD
    }

    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor, basic camera stuff

        PersistentHook();

        GameObject g = GameObject.FindGameObjectWithTag("Player");

        SetFocusTarget(g.transform); // setting the initial focus

        // creation of LEAD object
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
        //
        // SLerp and Lerp have a very important difference. Look up the documentation to understand why SLerp is used here.
        // It has something to do with LEAD's RotateAround() usage.
        transform.SetPositionAndRotation(
            Vector3.Slerp(transform.position, _lead.position, _LERP), 
            Quaternion.Lerp(transform.rotation, _lead.rotation, _LERP));
    }
}