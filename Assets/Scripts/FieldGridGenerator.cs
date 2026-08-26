using UnityEngine;

/// <summary>
/// Genera una cuadrícula de "fields" (plataformas) a partir de un prefab.
/// Cada field ocupa una celda de tamaño unitario (1x1) en el plano XZ.
/// El arreglo resultante puede ser consumido directamente por
/// FieldController / PlatformSpawner para el spawneo de obstáculos.
/// </summary>
public class FieldGridGenerator : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public GameObject fieldPrefab;
    public float spacing = 1f;
    public GameObject[] fields;

    void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        fields = new GameObject[rows*columns];
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 localPosition = new Vector3(col * spacing, 0f, row * spacing);

                GameObject field = Instantiate(fieldPrefab,transform.position + localPosition,Quaternion.identity,transform);

                field.name = $"Field_{row}_{col}";
                fields[index] = field;
                index++;
            }
        }
    }
}