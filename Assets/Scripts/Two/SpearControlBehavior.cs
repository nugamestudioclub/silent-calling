using UnityEngine;

public class SpearControlBehavior : MonoBehaviour
{
    [SerializeField] [Range(5f, 50f)] float MAX_RANGE_FROM_CAMERA = 25f;
    [SerializeField] [Range(0f, 5f)] float MIN_RANGE_FROM_CAMERA = 2f;
    [SerializeField] float LERP_SPEED = 0.0125f;
    [SerializeField] float LERP_ACCURACY = 0.25f;
    [SerializeField] float ANIMATION_SPEED = 50f;
    [SerializeField] float OFFSET_HEIGHT = 2.5f;
    [SerializeField] Transform SPEAR;

    const int bitmask = ~(1 << 2);

    Transform main_camera;
    Transform player;

    float distance_from_camera;

    bool in_stance = false;
    RaycastHit data;

    private void Start()
    {
        main_camera = Camera.main.transform;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (!p)
        {
            throw new MissingReferenceException("Player not found in scene!");
        }

        player = p.transform;

        Vector3 pos = SPEAR.position;
        pos.y = OFFSET_HEIGHT;
        SPEAR.position = pos;
    }

    private void Update() // late update?
    {
        if (in_stance)
        {
            Vector3 dest = GetPositionFromCamera() - Vector3.up;

            if (ShootRay())
            {
                dest.y = data.point.y + OFFSET_HEIGHT;
            }

            LerpToTarget(dest, 0f); // TODO this will have to be more complicated
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

    public void MoveSpear(Vector2 delta)
    {
        distance_from_camera = Mathf.Clamp(distance_from_camera + delta.y * 10f, MIN_RANGE_FROM_CAMERA, MAX_RANGE_FROM_CAMERA);

        HandleLineOfSight();
    }

    void HandleLineOfSight()
    {
        if (data.collider != null)
        {
            Debug.Log("Hit " + data.collider.name);

            Vanish();
        }
    }

    bool ShootRay()
    {
        Vector3 offset = transform.position;
        offset.y += OFFSET_HEIGHT;

        return Physics.Raycast(main_camera.position, offset - main_camera.position, out data, MAX_RANGE_FROM_CAMERA, bitmask);  
    }

    void Vanish()
    {
        distance_from_camera = MIN_RANGE_FROM_CAMERA;
        transform.position = main_camera.position + distance_from_camera * main_camera.forward;
    }

    void LerpToTarget(Vector3 target, float target_offset)
    {
        float accuracy = LERP_ACCURACY + target_offset;

        if ((target - transform.position).sqrMagnitude > (accuracy * accuracy))
        {
            transform.position = Vector3.Slerp(transform.position, target, LERP_SPEED * Time.deltaTime);
        }
    }

    Vector3 GetPositionFromCamera()
    {
        return player.position + main_camera.forward * distance_from_camera;
    }

}
