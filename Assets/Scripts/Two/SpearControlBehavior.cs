using UnityEngine;

public class SpearControlBehavior : MonoBehaviour
{
    [SerializeField] [Range(1f, 50f)] float MAX_RANGE_FROM_CAMERA = 25f;
    [SerializeField] float LERP_SPEED = 0.0125f;
    [SerializeField] float LERP_ACCURACY = 0.25f;
    [SerializeField] float ANIMATION_SPEED = 50f;
    [SerializeField] Transform SPEAR;

    Transform main_camera;
    Transform player;

    bool in_stance = false;

    private void Start()
    {
        main_camera = Camera.main.transform;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (!p)
        {
            throw new MissingReferenceException("Player not found in scene!");
        }

        player = p.transform;
    }

    private void Update() // late update?
    {
        if (in_stance)
        {
            LerpToTarget(main_camera.position, 0f); // TODO this will have to be more complicated
        } 

        else
        {
            LerpToTarget(player.position, 1f);
        }

        SPEAR.RotateAround(SPEAR.position, Vector3.up, ANIMATION_SPEED * 2f * Time.deltaTime);
        SPEAR.position += Mathf.Sin(Time.time * ANIMATION_SPEED * 0.1f) * Time.deltaTime * Vector3.up;
    }

    public void ToggleStance(bool value)
    {
        in_stance = value;
    }

    public void DoBehaviorWithValue(float value)
    {
        if (value < 0)
        {
            Debug.Log("Spin");
        }

        else
        {
            Debug.Log("Burst");
        }
    }

    void LerpToTarget(Vector3 target, float target_offset)
    {
        float accuracy = LERP_ACCURACY + target_offset;

        if ((target - transform.position).sqrMagnitude > (accuracy * accuracy))
        {
            transform.position = Vector3.Lerp(transform.position, target, LERP_SPEED * Time.deltaTime);
        }
    }

}
