using UnityEngine;

public class FieldController : MonoBehaviour
{
    public FieldGridGenerator gridGenerator;
    public Spawner spawner;

    void Start()
    {
        spawner.SpawnObstacles(gridGenerator.fields);
    }
}