using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public float smoothing = 2.0f;

    private Vector2 mouseLook;
    private Vector2 smoothV;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse input
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Adjust the mouse input sensitivity
        mouseInput = Vector2.Scale(mouseInput, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        // Interpolate the mouse input to make the movement smoother
        smoothV.x = Mathf.Lerp(smoothV.x, mouseInput.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseInput.y, 1f / smoothing);
        mouseLook += smoothV;

        // Clamp the vertical rotation to prevent flipping the camera
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        // Rotate the camera and player
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
    }
}
