using UnityEngine;

public class Tractor : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float legDuration = 3f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // gira 90° al completar cada lado
        if (timer >= legDuration)
        {
            transform.Rotate(0f, -90f, 0f);
            timer -= legDuration;
        }

        transform.position += -transform.forward * moveSpeed * Time.deltaTime;
    }
}
