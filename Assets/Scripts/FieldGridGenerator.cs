using UnityEngine;

public class FieldGridGenerator : MonoBehaviour
{
    public GameObject fieldPrefab;
    public GameObject floorPrefab;

    // World size of one floorPrefab tile (Campo is a 10-unit plane scaled x30 = 300).
    public float floorTileSize = 300f;

    public float spacing = 1f;
    public GameObject[] fields;

    // Kept so GetCellCenter() can index and bounds-check without re-receiving them.
    private int gridRows;
    private int gridColumns;
    public int Columns => gridColumns;

    // Stops the field being rebuilt on every message that repeats the grid size.
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
                // Columns straddle the object: half toward +X, half toward -X.
                float xOffset = (col - (columns - 1) / 2f) * spacing;

                // Rows subtract, so row 0 sits on the object and the rest run toward -Z.
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
        GenerateFloor();

        IsGenerated = true;
    }

    // Tiles the ground under the whole grid. The cells are centred on X at
    // transform.position.x and run from transform.position.z back toward -Z, so
    // the floor has to be aligned to those bounds - not around the world origin,
    // which is what left the -Z end uncovered.
    private void GenerateFloor()
    {
        if (floorPrefab == null || floorTileSize <= 0f)
        {
            return;
        }

        // Span the cells occupy in world units, plus one cell of headland margin.
        float gridWidth = (gridColumns - 1) * spacing + spacing;
        float gridDepth = (gridRows - 1) * spacing + spacing;

        int tilesX = Mathf.Max(1, Mathf.CeilToInt(gridWidth / floorTileSize));
        int tilesZ = Mathf.Max(1, Mathf.CeilToInt(gridDepth / floorTileSize));

        // Grid centre in world space.
        float centerX = transform.position.x;
        float centerZ = transform.position.z - (gridRows - 1) * spacing / 2f;

        // Top-left tile centre, so the block stays centred on the grid.
        float startX = centerX - (tilesX - 1) * floorTileSize / 2f;
        float startZ = centerZ + (tilesZ - 1) * floorTileSize / 2f;

        for (int iz = 0; iz < tilesZ; iz++)
        {
            for (int ix = 0; ix < tilesX; ix++)
            {
                Vector3 position = new Vector3(
                    startX + ix * floorTileSize,
                    transform.position.y,
                    startZ - iz * floorTileSize
                );

                GameObject floor = Instantiate(
                    floorPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                floor.name = $"Floor_{ix}_{iz}";
            }
        }
    }

    // World position of cell (row, col); assumes fieldPrefab's pivot is cell-centred.
    public Vector3 GetCellCenter(int row, int col)
    {
        if (!IsGenerated)
        {
            Debug.LogWarning("A cell was requested before the grid was generated.");
            return transform.position;
        }

        if (row < 0 || row >= gridRows || col < 0 || col >= gridColumns)
        {
            Debug.LogWarning($"Cell out of range: ({row}, {col})");
            return transform.position;
        }

        int index = row * gridColumns + col;
        return fields[index].transform.position;
    }
}