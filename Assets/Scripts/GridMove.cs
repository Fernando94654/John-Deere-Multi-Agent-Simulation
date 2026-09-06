using UnityEngine;

// Se adjunta al prefab de tractor y al de harvester. Su única función es
// desplazarse suavemente hacia la última celda objetivo que se le indique,
// en vez de saltar de golpe cada vez que llega una actualización del
// servidor (recordar que el servidor manda la posición cada ~200 ms, no
// cada fotograma).
public class GridMove : MonoBehaviour
{
    public float moveSpeed = 2f;

    // Qué tan rápido gira el vehículo para alinearse con su dirección de
    // movimiento. Un valor alto lo hace girar casi instantáneo; uno bajo
    // produce un giro más gradual y visible.
    public float rotationSpeed = 10f;

    private Vector3 targetPosition;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        RotateTowardsTarget();

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    void RotateTowardsTarget()
    {
        // Vector desde la posición actual hacia el destino. Se anula la
        // componente Y para que el vehículo no se incline hacia arriba o
        // abajo, solo gire sobre el plano horizontal.
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        // Si la distancia restante es casi cero (el vehículo ya llegó, o
        // aún no se le ha asignado un destino distinto), direction sería
        // un vector nulo y Quaternion.LookRotation lanzaría una advertencia;
        // en ese caso simplemente no se actualiza la rotación este frame.
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // Slerp en vez de asignar la rotación de golpe: así el giro se ve
        // como un movimiento continuo en vez de un salto brusco cada vez
        // que cambia la celda objetivo.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }
}