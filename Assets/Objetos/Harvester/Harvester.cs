using UnityEngine;

public class Harvester : MonoBehaviour
{
    public float moveSpeed = 10f;

    private float timer = 0f;
    private bool hasTurned = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer <= 1.8f)
        {
            transform.Translate(0f, 0f, moveSpeed * Time.deltaTime);
        }
        else if (!hasTurned)
        {
            transform.Rotate(0f, 90f, 0f);
            hasTurned = true;
        }
        else if (timer <= 5f)
        {
            transform.Translate(0f, 0f, moveSpeed * Time.deltaTime);
        }
    }
}