using UnityEngine;
using URandom = UnityEngine.Random;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Spawner spawner;
    public GameObject[] SpawnableObjects;
    public GameObject wheatPrefab;
    public float wheatSpacing = 0.1f;
    private float obstaclePercentage = 0.05f;
    private List<GameObject> wheatList = new List<GameObject>();

    void Awake()
    {
        spawner = this;
    }

    public void SpawnObstacles(GameObject[] fields)
    {
        int obstacleCount = Mathf.RoundToInt(fields.Length * obstaclePercentage);

        List<GameObject> availableFields = new List<GameObject>(fields);
        HashSet<GameObject> fieldsWithObstacle = new HashSet<GameObject>();

        for (int i = 0; i < obstacleCount; i++)
        {
            if (availableFields.Count == 0)
                break;

            int fieldIndex = URandom.Range(0, availableFields.Count);
            GameObject selectedField = availableFields[fieldIndex];

            // Lo elimina para evitar que vuelva a seleccionars
            availableFields.RemoveAt(fieldIndex);

            fieldsWithObstacle.Add(selectedField);
        }

        // Recorrido único de todos los fields: aquí se asigna directamente
        // obstáculo o trigo mediante el if / else
        foreach (GameObject field in fields)
        {
            if (fieldsWithObstacle.Contains(field))
            {
                SpawnObject(field);
            }
            else
            {
                SpawnWheat(field);
            }
        }
    }

    private void SpawnObject(GameObject field)
    {
        Collider fieldCollider = field.GetComponent<Collider>();
        Bounds bounds = fieldCollider.bounds;

        int objectIndex = URandom.Range(0, SpawnableObjects.Length);
        GameObject selectedObject = SpawnableObjects[objectIndex];

        Renderer objectRenderer = selectedObject.GetComponentInChildren<Renderer>();
        float y = bounds.max.y + objectRenderer.bounds.extents.y;

        Vector3 spawnPosition = new Vector3(bounds.center.x, y, bounds.center.z);
        Instantiate(selectedObject, spawnPosition, Quaternion.identity);
    }

    private void SpawnWheat(GameObject field)
    {
        Collider fieldCollider = field.GetComponent<Collider>();
        Bounds fieldBounds = fieldCollider.bounds;

        Renderer wheatRenderer = wheatPrefab.GetComponentInChildren<Renderer>();
        float wheatSizeX = wheatRenderer.bounds.size.x;
        float wheatSizeZ = wheatRenderer.bounds.size.z;

        float stepX = wheatSizeX + wheatSpacing;
        float stepZ = wheatSizeZ + wheatSpacing;

        // Número de trigos que caben por eje, considerando su tamaño más la separación
        int columns = Mathf.FloorToInt((fieldBounds.size.x + wheatSpacing) / stepX);
        int rows = Mathf.FloorToInt((fieldBounds.size.z + wheatSpacing) / stepZ);

        // Punto de partida: esquina inferior del field, desplazado medio trigo
        // para que la cuadrícula de trigos quede centrada dentro de la casilla
        float usedWidth = columns * stepX - wheatSpacing;
        float usedDepth = rows * stepZ - wheatSpacing;

        float startX = fieldBounds.center.x - usedWidth * 0.5f + wheatSizeX * 0.5f;
        float startZ = fieldBounds.center.z - usedDepth * 0.5f + wheatSizeZ * 0.5f;
        float y = fieldBounds.max.y + wheatRenderer.bounds.extents.y;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = new Vector3(
                    startX + col * stepX,
                    y,
                    startZ + row * stepZ
                );

                GameObject wheat = Instantiate(
    wheatPrefab,
    spawnPosition,
    Quaternion.identity
);

wheatList.Add(wheat);
            }
        }
    }


public void ReactivateWheat()
{
    foreach (GameObject wheat in wheatList)
    {
        if (wheat != null && !wheat.activeSelf)
        {
            wheat.SetActive(true);
        }
    }
}
}