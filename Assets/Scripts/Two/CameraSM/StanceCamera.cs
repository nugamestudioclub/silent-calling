using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class StanceCamera : ACameraState
{
    SpearControlBehavior spear;
    Transform transform;

    Transform player;
    Transform _focusTarget;

    float angle;

    const float _SENSITIVITY = 15f;

    public StanceCamera(Action<TwoState> f) : base(f)
    {
        GameObject spr = GameObject.FindGameObjectWithTag("TwoSpear");
        spear = spr.GetComponent<SpearControlBehavior>();
        transform = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void CameraFocusTarget(Transform t)
    {
        _focusTarget = t;
    }

    public override void CameraMouseClicked(InputAction.CallbackContext context)
    {
        // pass?
    }

    public override void CameraMouseDelta(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float multiplier = _SENSITIVITY * Time.deltaTime;

            Vector2 delta = context.ReadValue<Vector2>();

            transform.RotateAround(player.position, Vector3.up, delta.x * multiplier);

            angle = Mathf.Clamp(angle - delta.y * multiplier, -70f, 70f);

            if (Mathf.Abs(angle) < 70f)
            {
                transform.RotateAround(transform.position, transform.right, -delta.y * multiplier);
            }

            // spear.HandleLineOfSight();
        }
    }


    public override void StateLateUpdate()
    {
        // pass
    }

    public override void StateStart()
    {
        transform.position = player.position + player.forward + Vector3.up * 2f;
        CameraFocusTarget(spear.gameObject.transform);

        angle = 0f;
    }

    public override void StateUpdate()
    {
        // maintain distance from Two's shoulder.
    }

    protected override void CameraAnimation()
    {
        // pass
    }

    protected override void CameraMaintainDistance()
    {
        // pass
    }
}
