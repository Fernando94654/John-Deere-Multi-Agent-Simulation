using System.Collections.Generic;
using UnityEngine;

// Este script es el "pegamento" entre las tres piezas: recibe el estado
// desde WebSocketManager, genera el field una sola vez con FieldGridGenerator,
// y luego instancia o mueve los prefabs de tractor/harvester usando
// GetCellCenter() para saber a qué punto del mundo deben dirigirse.
public class VehicleManager : MonoBehaviour
{
    [Header("Referencias")]
    public WebSocketManager webSocketManager;
    public FieldGridGenerator fieldGridGenerator;

    [Header("Prefabs")]
    public GameObject tractorPrefab;
    public GameObject harvesterPrefab;

    // Se guarda cada vehículo instanciado por su id para poder moverlo o
    // eliminarlo en actualizaciones posteriores en vez de recrearlo.
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
        // El field solo se construye la primera vez que llega el tamaño de
        // grid; los mensajes siguientes solo actualizan posiciones.
        if (!fieldGridGenerator.IsGenerated)
        {
            fieldGridGenerator.GenerateGrid(state.grid.rows, state.grid.columns);
            PlaceObstacles(state.obstacles, state.silos);
        }

        SyncVehicles(state.tractors, tractorObjects, tractorPrefab);
        SyncVehicles(state.harvesters, harvesterObjects, harvesterPrefab);
    }

    void SyncVehicles(VehicleData[] vehicles, Dictionary<string, GameObject> objects, GameObject prefab)
    {
        foreach (VehicleData vehicle in vehicles)
        {
            Vector3 targetPosition = fieldGridGenerator.GetCellCenter(vehicle.row, vehicle.column);

            if (!objects.ContainsKey(vehicle.id))
            {
                GameObject newVehicle = Instantiate(prefab, targetPosition, Quaternion.identity);
                newVehicle.name = vehicle.id;
                objects.Add(vehicle.id, newVehicle);
            }
            else
            {
                GridMove mover = objects[vehicle.id].GetComponent<GridMove>();

                if (mover != null)
                {
                    mover.SetTarget(targetPosition);
                }
                else
                {
                    // Si el prefab no tiene GridMover, se hace el
                    // desplazamiento inmediato como respaldo.
                    objects[vehicle.id].transform.position = targetPosition;
                }
            }
        }
    }

    // Marca visualmente las celdas de obstáculo. Aquí solo se desactiva el
    // renderer del cuadrito correspondiente como ejemplo mínimo; puede
    // sustituirse por instanciar un prefab distinto sobre esa celda.
    void PlaceObstacles(ObstacleInfo obstacles, SiloInfo silos)
    {
        // Defensivo: el contenedor o sus posiciones pueden llegar como null.
        if (obstacles == null || obstacles.positions == null)
        {
            return;
        }

        Spawner.spawner.SpawnFromServer(
            fieldGridGenerator.fields,
            fieldGridGenerator.Columns,
            obstacles.positions, silos.positions
        );
    }

    // Método genérico para no repetir la misma lógica entre tractores y
    // harvesters: crea el GameObject si el id es nuevo, o actualiza su
    // destino si ya existía.
}