using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class StanceCamera : ACameraState
{
    SpearControlBehavior spear;
    Transform player;
    Transform _focusTarget;
    Transform this_camera;

    public StanceCamera(Action<TwoState> f) : base(f)
    {
        GameObject spr = GameObject.FindGameObjectWithTag("TwoSpear");
        spear = spr.GetComponent<SpearControlBehavior>();
        this_camera = Camera.main.transform;
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
            Vector2 delta = context.ReadValue<Vector2>();

            this_camera.RotateAround(player.position, Vector3.up, delta.x);
            this_camera.RotateAround(this_camera.position, this_camera.right, -delta.y);
        }
    }

    public override void StateLateUpdate()
    {
        // pass
    }

    public override void StateStart()
    {
        this_camera.position = player.position + player.forward + Vector3.up * 2f;
        CameraFocusTarget(spear.gameObject.transform);
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
        // rotate around Two's shoulder area.
    }
}
