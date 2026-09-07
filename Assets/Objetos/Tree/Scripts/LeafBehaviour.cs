using UnityEngine;

public class LeafBehaviour : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeDestroy = 0f; // Retardo opcional antes de destruir

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, delayBeforeDestroy);
    }
}