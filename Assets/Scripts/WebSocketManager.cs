using UnityEngine;
using System;
using NativeWebSocket;

[Serializable]
public class GridInfo
{
    public int rows;
    public int columns;
}
 
[Serializable]
public class ObstacleData
{
    public int row;
    public int column;
}

[Serializable]
public class ObstacleInfo
{
    public int count;
    public ObstacleData[] positions;
}

[Serializable]
public class SiloData
{
    public int row;
    public int column;
}

[Serializable]
public class SiloInfo
{
    public int count;
    public SiloData[] positions;
}
 
// Agent facing as a (row, column) step: (0,1) looks east.
[Serializable]
public class HeadingData
{
    public int row;
    public int column;
}

[Serializable]
public class VehicleData
{
    public string id;
    public int row;
    public int column;
    public ObstacleData[] route;

    // Engine-side state: "harvesting", "waiting cart", "to farm", "transferring".
    public string state;
    public int load;
    public int capacity;
    public HeadingData heading;
}

// The field size Unity asks the server for.
[Serializable]
public class FieldRequest
{
    public int rows;
    public int columns;
}
 
// JsonUtility takes no nested arrays, so terrain arrives flat: index = row * columns + col.
[Serializable]
public class CropInfo
{
    public int[] cells;
}

[Serializable]
public class MetricsInfo
{
    public int harvested;
    public int delivered;
    public int inTransit;
    public int distance;
    public float fuel;
    public float co2;
}

[Serializable]
public class SimulationState
{
    public int tick;

    // Seconds until the next state; GridMove spreads one cell of travel over it.
    public float tickInterval;

    public GridInfo grid;
    public CropInfo crop;
    public ObstacleInfo obstacles;
    public SiloInfo silos;
    public VehicleData[] tractors;
    public VehicleData[] harvesters;
    public MetricsInfo metrics;
}
public class WebSocketManager : MonoBehaviour
{
    public event Action<SimulationState> OnStateUpdated;
    public SimulationState LatestState { get; private set; }

    [Header("Conexión")]
    public string serverUrl = "ws://localhost:8765";

    // The server idles until the UI button sends the field size, not the connection.
    public bool Conectado { get; private set; }
    public bool ConfiguracionEnviada { get; private set; }

    private WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Conectado = true;
            Debug.Log("Conectado al servidor. Esperando a que se inicie la simulación.");
        };
 
        websocket.OnError += (error) =>
        {
            Debug.LogError("Error WebSocket: " + error);
        };
 
        websocket.OnClose += (closeCode) =>
        {
            Debug.Log("Conexión cerrada");
        };
 
        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            HandleMessage(message);
        };
 
        await websocket.Connect();
    }
 
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        // NativeWebSocket needs pumping inside Unity's loop or OnMessage never fires.
        websocket?.DispatchMessageQueue();
#endif
    }
 
    // Called by the UI button; the server builds the run, so only the first call counts.
    public async void EnviarConfiguracion(int rows, int columns)
    {
        if (!Conectado)
        {
            Debug.LogWarning("Todavía no hay conexión con el servidor.");
            return;
        }

        if (ConfiguracionEnviada)
        {
            Debug.LogWarning("La simulación ya arrancó; para cambiar el tamaño hay que reiniciar el Play.");
            return;
        }

        ConfiguracionEnviada = true;

        FieldRequest request = new FieldRequest { rows = rows, columns = columns };
        Debug.Log("Pidiendo campo de " + rows + "x" + columns);
        await websocket.SendText(JsonUtility.ToJson(request));
    }

    void HandleMessage(string message)
    {
        SimulationState state = JsonUtility.FromJson<SimulationState>(message);
 
        // Dropped rather than passed on: JsonUtility nulls any object the JSON omits.
        if (state == null || state.grid == null || state.obstacles == null
            || state.silos == null || state.tractors == null || state.harvesters == null)
        {
            Debug.LogWarning("Mensaje recibido con formato inesperado: " + message);
            return;
        }
 
        LatestState = state;
        OnStateUpdated?.Invoke(state);
    }
 
    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}