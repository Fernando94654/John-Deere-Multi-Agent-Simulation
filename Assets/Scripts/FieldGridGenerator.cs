using UnityEngine;
using TMPro;
public class FieldGridGenerator : MonoBehaviour
{
    public GameObject fieldPrefab;
    public float spacing = 1f;
    public GameObject[] fields;


    public void GenerateGrid(int rows, int columns)
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