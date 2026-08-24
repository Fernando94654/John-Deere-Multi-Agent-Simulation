using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject prefab;

    public int columns = 76;
    public int rows = 98 ;

    public float xOffset = -70f;
    public float zOffset = -85f;

    public float spacingX = 1.5f;
    public float spacingZ = 1.5f;

    void Start()
    {
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector3 position = new Vector3(
                    row * spacingX + xOffset,
                    0,
                    column * spacingZ + zOffset
                );

                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}