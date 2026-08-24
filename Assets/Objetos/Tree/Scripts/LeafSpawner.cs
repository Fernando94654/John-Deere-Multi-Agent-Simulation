using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;      // Prefab de la hoja
    public float spawnRadius;     // Radio alrededor del punto central
    public Vector3 spawnCenter;        // Centro del área de generación (ej. copa del árbol)

    public float spawnInterval ;   // Segundos entre cada hoja generada

    void Start()
    {
        InvokeRepeating(nameof(SpawnLeaf), 0, spawnInterval);
    }

    void SpawnLeaf()
    {
        // Posición aleatoria dentro de una esfera
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnPosition = transform.position + spawnCenter + randomOffset;

        // Rotación aleatoria para variedad visual
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        GameObject leaf = Instantiate(leafPrefab, spawnPosition, randomRotation, transform);

        // Escala ligeramente variable
        float randomScale = Random.Range(0.8f, 1.2f);
        leaf.transform.localScale *= randomScale;
    }
}