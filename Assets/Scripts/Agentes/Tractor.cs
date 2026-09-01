using UnityEngine;

public class Tractor : MonoBehaviour
{
    public static Tractor tractor;
    public float moveSpeed;
    public float timer = 0f;
    private bool hasTurned = false;
    private Vector3 posInitial;
    private Quaternion rotInitial;
    private Quaternion targetRotation;

    void Awake()
    {
        tractor = this;
    }

    void Start()
    {
        posInitial = transform.position;
        rotInitial = transform.rotation;
    }
    public void Move()
    {
        timer += Time.deltaTime;

        if (timer <= 3f)
        {
            transform.position += -transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (!hasTurned)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0f, -90f, 0f);
            hasTurned = true;
        }
        else if (timer <= 5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

        else if (timer <= 7f)
        {
            transform.position += -transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
        ResetMovement();
        }
        // después del segundo 5 ya no hace nada: se detiene
        
    }

    public void ResetMovement()
    {
        transform.position = posInitial;
        transform.rotation = rotInitial;
        timer = 0f;
        UIController.controller.timer = 0f;
        hasTurned = false;
        Spawner.spawner.ReactivateWheat();
    }
}