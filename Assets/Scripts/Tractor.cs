using UnityEngine;

public class Tractor : MonoBehaviour
{
    public float moveSpeed = 2f;

    private float timer = 0f;
    private bool hasTurned = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer <= 3f)
        {
            transform.position += -transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (!hasTurned)
        {
            transform.Rotate(0f, -90f, 0f);
            hasTurned = true;
        }
        else if (timer <= 5f)
        {
            transform.position += -transform.forward * moveSpeed * Time.deltaTime;
        }
        // después del segundo 5 ya no hace nada: se detiene
    }
}