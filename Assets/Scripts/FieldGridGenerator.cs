using UnityEngine;

public class FieldGridGenerator : MonoBehaviour
{
    public GameObject fieldPrefab;
    public float spacing = 1f;
    public GameObject[] fields;

    // Se guardan para poder calcular índices y validar límites en
    // GetCellCenter() sin tener que volver a recibirlos como parámetro.
    private int gridRows;
    private int gridColumns;
    public int Columns => gridColumns;

    // Evita generar el campo más de una vez si el servidor sigue mandando
    // el mismo tamaño de grid en cada mensaje (ver VehicleManager).
    public bool IsGenerated { get; private set; } = false;

    public void GenerateGrid(int rows, int columns)
    {
        gridRows = rows;
        gridColumns = columns;

        fields = new GameObject[rows * columns];
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Columnas: se centran respecto al objeto (mitad hacia +X,
                // mitad hacia -X), en vez de partir todas hacia la derecha.
                float xOffset = (col - (columns - 1) / 2f) * spacing;

                // Filas: se restan en vez de sumarse, de modo que row = 0
                // quede exactamente en el objeto (el "centro superior") y
                // las filas siguientes avancen en -Z, es decir, hacia el
                // lado contrario de donde apunta la flecha azul.
                float zOffset = -row * spacing;

                Vector3 localPosition = new Vector3(xOffset, 0f, zOffset);

                GameObject field = Instantiate(
                    fieldPrefab,
                    transform.position + localPosition,
                    Quaternion.identity,
                    transform
                );

                field.name = $"Field_{row}_{col}";
                fields[index] = field;
                index++;
            }
        }

        IsGenerated = true;
    }

    // Regresa la posición en el mundo del centro de la celda (row, col).
    // Se asume que el pivote de fieldPrefab está en el centro del cuadrito
    // (como ocurre con un Plane o Cube por defecto); si el prefab tuviera
    // el pivote en una esquina, habría que sumar (spacing / 2f) en x y z.
    public Vector3 GetCellCenter(int row, int col)
    {
        if (!IsGenerated)
        {
            Debug.LogWarning("Se pidió una celda antes de generar el grid.");
            return transform.position;
        }

        if (row < 0 || row >= gridRows || col < 0 || col >= gridColumns)
        {
            Debug.LogWarning($"Celda fuera de rango: ({row}, {col})");
            return transform.position;
        }

        int index = row * gridColumns + col;
        return fields[index].transform.position;
    }
}