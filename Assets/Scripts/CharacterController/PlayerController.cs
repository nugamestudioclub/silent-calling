using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform m_pov;

    [SerializeField]
    private float m_playerSpeed = 10f;

    [SerializeField]
    private float m_drag = 0.3f;

    private bool m_isMovePressed;
    private Vector3 m_currentInput;

    private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        if (!m_pov)
        {
            m_pov = Camera.main.transform;
        }
    }

    public void GatherLateral(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        m_currentInput.x = input.x;
        m_currentInput.z = input.y;

        m_isMovePressed = !context.canceled;
    }

    private void FixedUpdate()
    {
        if (m_isMovePressed)
        {
            HandlePlayerMovement();
        }

        ApplyArtificialDrag();
    }

    private void HandlePlayerMovement()
    {
        Vector3 direction = m_pov.TransformDirection(m_currentInput);
        direction.y = 0f;
        direction.Normalize();


        if (m_rigidbody.velocity.sqrMagnitude <= m_playerSpeed * m_playerSpeed)
        {
            m_rigidbody.AddForce(direction * m_playerSpeed, ForceMode.VelocityChange);
        }
    }

    private void ApplyArtificialDrag()
    {
        m_rigidbody.AddForce(-m_rigidbody.velocity * m_drag, ForceMode.VelocityChange);
    }

}
