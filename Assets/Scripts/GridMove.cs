using UnityEngine;

// Spreads one cell of travel over the server's tick interval, on the vehicle prefabs.
public class GridMove : MonoBehaviour
{
    // Fallback only, for when the server reports no tick interval.
    public float moveSpeed = 2f;

    // How fast the vehicle turns to face its heading. Higher is snappier.
    public float rotationSpeed = 10f;

    private Vector3 targetPosition;
    private Vector3 facing;
    private float speed;

    void Awake()
    {
        targetPosition = transform.position;
        facing = transform.forward;
        speed = moveSpeed;
    }

    void Update()
    {
        RotateTowardsFacing();

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    void RotateTowardsFacing()
    {
        // Heading arrives even on a still tick, so a spout rotation turns in place.
        if (facing.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(facing.normalized, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Distance over time-until-next-state: one step takes one tick, and lag self-corrects.
    public void SetTarget(Vector3 newTarget, float tickInterval)
    {
        targetPosition = newTarget;

        if (tickInterval <= 0f)
        {
            speed = moveSpeed;
            return;
        }

        speed = Vector3.Distance(transform.position, newTarget) / tickInterval;
    }

    public void SetFacing(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        facing = direction;
    }
}
