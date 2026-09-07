using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;      // the leaf to spawn
    public float spawnRadius;     // radius around the centre point
    public Vector3 spawnCenter;        // centre of the spawn area, e.g. the treetop

    public float spawnInterval ;   // seconds between leaves

    void Start()
    {
        InvokeRepeating(nameof(SpawnLeaf), 0, spawnInterval);
    }

    void SpawnLeaf()
    {
        // Random position inside a sphere.
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnPosition = transform.position + spawnCenter + randomOffset;

        // Random rotation, so the leaves do not all look alike.
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        GameObject leaf = Instantiate(leafPrefab, spawnPosition, randomRotation, transform);

        // Slightly varied scale.
        float randomScale = Random.Range(0.8f, 1.2f);
        leaf.transform.localScale *= randomScale;
    }
}