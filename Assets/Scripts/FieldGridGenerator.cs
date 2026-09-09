using UnityEngine;
using System;

public class FieldGridGenerator : MonoBehaviour
{
    public GameObject fieldPrefab;
    public GameObject floorPrefab;
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

        // Generator of floor
     
            int extraFloorSides = (gridColumns > 15) ? (int)Math.Ceiling((gridColumns / 2f) / 15f) * 2 : 1;
            int extraFloorDown  = (gridRows > 22)    ? (int)Math.Ceiling((gridRows+7) / 15f) : 1;


            int begginingX = -300 * extraFloorSides/2 + 150;
            int begginingZ = 0;

            for(int jz = 0; jz<extraFloorDown; jz++){
                for(int ix=0; ix<extraFloorSides; ix++){

                    int posZ = begginingZ - (jz*300);
                    int posX = begginingX + (ix*300);
                    Vector3 posicionEspecifica = new Vector3(posX, 0f, posZ);

                    GameObject floor = Instantiate (
                        floorPrefab,
                        posicionEspecifica,
                        Quaternion.identity,
                        transform
                    );

                    floor.name = $"Floor_{ix}_{jz}";
                }
            }

        IsGenerated = true;

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