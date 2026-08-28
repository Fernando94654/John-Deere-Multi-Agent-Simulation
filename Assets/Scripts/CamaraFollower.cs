using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target; // El objeto que la cámara va a seguir
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Distancia entre la cámara y el objeto
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    void LateUpdate()
    {
        // Calcula la posición deseada
        Vector3 desiredPosition = target.position + offset;
        
        // Suaviza el movimiento usando Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Aplica la nueva posición a la cámara
        transform.position = smoothedPosition;

        // Hace que la cámara mire al objeto
        transform.LookAt(target);
    }
}