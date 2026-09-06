using UnityEngine;
using URandom = UnityEngine.Random;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Spawner spawner;
    public GameObject[] SpawnableObjects;
    public GameObject siloPrefab;
    public GameObject wheatPrefab;
    public float wheatSpacing = 0.1f;
    private float obstaclePercentage = 0.05f;
    private List<GameObject> wheatList = new List<GameObject>();

    void Awake()
    {
        spawner = this;
    }

    // Se conserva para pruebas sin servidor conectado: elige obstáculos al
    // azar sobre el arreglo de fields recibido.
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

            availableFields.RemoveAt(fieldIndex);

            fieldsWithObstacle.Add(selectedField);
        }

        SpawnFields(fields, fieldsWithObstacle);
    }

    // Punto de integración con el servidor: en vez de elegir al azar, recibe
    // las posiciones exactas de obstáculo (row, column) y las traduce al
    // índice del arreglo "fields" con la misma fórmula que usa
    // FieldGridGenerator (index = row * columns + col), así que el orden en
    // que se generó el grid debe coincidir con el que se usa aquí.
    public void SpawnFromServer(GameObject[] fields, int columns, ObstacleData[] obstaclePositions, SiloData[] siloPositions)
    {
        HashSet<GameObject> fieldsWithObstacle = new HashSet<GameObject>();

        foreach (ObstacleData obstacle in obstaclePositions)
        {
            int index = obstacle.row * columns + obstacle.column;

            if (index >= 0 && index < fields.Length)
            {
                fieldsWithObstacle.Add(fields[index]);
            }
        }

        foreach (SiloData silo in siloPositions)
        {
            int index = silo.row * columns + silo.column;

            if (index >= 0 && index < fields.Length)
            {
                GameObject field = fields[index];
                Vector3 position = field.transform.position;
                position.y += 0.5f; // Ajusta la altura según sea necesario
                Instantiate(siloPrefab, position, Quaternion.identity);
            }
        }

        SpawnFields(fields, fieldsWithObstacle);
    }

    private void SpawnFields(GameObject[] fields, HashSet<GameObject> fieldsWithObstacle)
    {
        foreach (GameObject field in fields)
        {
            if (fieldsWithObstacle.Contains(field))
            {
                SpawnObject(field);
            }
            else
            {
                //SpawnWheat(field);
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

    // Límite defensivo: si fieldPrefab no está a la escala esperada (por
    // ejemplo, un Plane sin reescalar mide 10x10 en vez de 1x1), este
    // cálculo puede arrojar miles de columnas/filas y congelar o tirar el
    // Editor al intentar instanciar todos esos trigos de golpe. Este límite
    // no soluciona el problema de fondo (verificar la escala real de
    // fieldPrefab), pero evita que un descuido de escala cause un crash.
    private const int MaxWheatPerAxis = 30;

    private void SpawnWheat(GameObject field)
    {
        Collider fieldCollider = field.GetComponent<Collider>();
        Bounds fieldBounds = fieldCollider.bounds;

        Renderer wheatRenderer = wheatPrefab.GetComponentInChildren<Renderer>();
        float wheatSizeX = wheatRenderer.bounds.size.x;
        float wheatSizeZ = wheatRenderer.bounds.size.z;

        float stepX = wheatSizeX + wheatSpacing;
        float stepZ = wheatSizeZ + wheatSpacing;

        int columns = Mathf.FloorToInt((fieldBounds.size.x + wheatSpacing) / stepX);
        int rows = Mathf.FloorToInt((fieldBounds.size.z + wheatSpacing) / stepZ);

        if (columns > MaxWheatPerAxis || rows > MaxWheatPerAxis)
        {
            Debug.LogWarning(
                $"{field.name}: se calcularon {columns}x{rows} trigos, lo cual excede " +
                $"el límite de seguridad ({MaxWheatPerAxis}). Revisa la escala real de " +
                "fieldPrefab (probablemente mide más de 1x1 unidad). Se recorta el " +
                "conteo para evitar un cuelgue del Editor."
            );

            columns = Mathf.Min(columns, MaxWheatPerAxis);
            rows = Mathf.Min(rows, MaxWheatPerAxis);
        }

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