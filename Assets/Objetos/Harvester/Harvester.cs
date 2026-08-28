using UnityEngine;

public class Harvester : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float legDuration = 1.8f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // gira 90° al completar cada lado
        if (timer >= legDuration)
        {
            transform.Rotate(0f, 90f, 0f);
            timer -= legDuration;
        }

        transform.Translate(0f, 0f, moveSpeed * Time.deltaTime);
    }
}
