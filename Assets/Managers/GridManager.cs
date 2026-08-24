using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject prefab;

    public int rows = 82;
    public int columns = 95;

    public float xOffset = -70f;
    public float zOffset = -85f;

    public float spacingX = 1.5f;
    public float spacingZ = 1.5f;

    void Start()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(
                    column * spacingX + xOffset,
                    0,
                    row * spacingZ + zOffset
                );

                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}