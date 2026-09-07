using System.Collections.Generic;
using UnityEngine;

// Glue: takes server state, builds the field once, then spawns and drives the vehicles.
public class VehicleManager : MonoBehaviour
{
    [Header("Referencias")]
    public WebSocketManager webSocketManager;
    public FieldGridGenerator fieldGridGenerator;
    public FieldPainter fieldPainter;

    [Header("Prefabs")]
    public GameObject tractorPrefab;
    public GameObject harvesterPrefab;

    [Header("Presentación")]
    [Tooltip("Spacing within a cell so vehicles do not overlap at the silo.")]
    public float separacionEnCelda = 3f;

    // Vehicles are kept by id so later updates move them instead of recreating them.
    private Dictionary<string, GameObject> tractorObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> harvesterObjects = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        webSocketManager.OnStateUpdated += HandleStateUpdated;
    }

    void OnDisable()
    {
        webSocketManager.OnStateUpdated -= HandleStateUpdated;
    }

    void HandleStateUpdated(SimulationState state)
    {
        // The field is built on the first message only; the rest just update positions.
        if (!fieldGridGenerator.IsGenerated)
        {
            fieldGridGenerator.GenerateGrid(state.grid.rows, state.grid.columns);
            PlaceObstacles(state.obstacles, state.silos);
        }

        SyncVehicles(state.tractors, tractorObjects, tractorPrefab, state.tickInterval);
        SyncVehicles(state.harvesters, harvesterObjects, harvesterPrefab, state.tickInterval);

        // The terrain arrives whole each message, so repainting needs no accumulation.
        if (fieldPainter != null && state.crop != null)
        {
            fieldPainter.Paint(fieldGridGenerator.fields, state.crop.cells);
        }
    }

    void SyncVehicles(VehicleData[] vehicles, Dictionary<string, GameObject> objects, GameObject prefab, float tickInterval)
    {
        foreach (VehicleData vehicle in vehicles)
        {
            Vector3 targetPosition = fieldGridGenerator.GetCellCenter(vehicle.row, vehicle.column)
                                     + DesfaseDe(vehicle.id);

            if (!objects.ContainsKey(vehicle.id))
            {
                GameObject newVehicle = Instantiate(prefab, targetPosition, Quaternion.identity);
                newVehicle.name = vehicle.id;
                objects.Add(vehicle.id, newVehicle);
            }

            GridMove mover = objects[vehicle.id].GetComponent<GridMove>();

            if (mover == null)
            {
                // Without GridMove, fall back to snapping straight to the cell.
                objects[vehicle.id].transform.position = targetPosition;
                continue;
            }

            mover.SetTarget(targetPosition, tickInterval);

            if (vehicle.heading != null)
            {
                mover.SetFacing(DireccionMundo(vehicle.heading));
            }
        }
    }

    // The engine thinks in (row, col); rows run toward -Z and columns toward +X.
    Vector3 DireccionMundo(HeadingData heading)
    {
        return new Vector3(heading.column, 0f, -heading.row);
    }

    // Machines may share the silo cell, so a small per-id offset keeps them apart.
    Vector3 DesfaseDe(string id)
    {
        int hash = Mathf.Abs(id.GetHashCode());
        float angulo = (hash % 360) * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo)) * separacionEnCelda;
    }

    // Marks the obstacle and silo cells on the freshly built field.
    void PlaceObstacles(ObstacleInfo obstacles, SiloInfo silos)
    {
        // Defensive: either container, or its positions, can arrive null.
        if (obstacles == null || obstacles.positions == null
            || silos == null || silos.positions == null)
        {
            return;
        }

        Spawner.spawner.SpawnFromServer(
            fieldGridGenerator.fields,
            fieldGridGenerator.Columns,
            obstacles.positions, silos.positions
        );
    }

    // Shared by tractors and harvesters: spawn on a new id, retarget on a known one.
}