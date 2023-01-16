using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that adds an impact to a charactercontroller because bruhh we have to do that
/// this should be consolidated into TwoSM when reworked because it doesnt need to be an extra behavior.
/// </summary>
public class CharacterControllerForceScript : MonoBehaviour
{
    Vector3 impact = Vector3.zero;
    private CharacterController character;
 
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    public void AddImpact(Vector3 dir, float force, bool additive)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground

        impact = dir.normalized * force + (additive ? impact : Vector3.zero);
    }

    void FixedUpdate()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2) character.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}
