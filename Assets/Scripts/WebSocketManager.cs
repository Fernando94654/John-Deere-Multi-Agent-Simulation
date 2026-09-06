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
 
[Serializable]
public class VehicleData
{
    public string id;
    public int row;
    public int column;
    public ObstacleData[] route;
}
 
[Serializable]
public class SimulationState
{
    public GridInfo grid;
    public ObstacleInfo obstacles;
    public SiloInfo silos;
    public VehicleData[] tractors;
    public VehicleData[] harvesters;
}
public class WebSocketManager : MonoBehaviour
{
    public event Action<SimulationState> OnStateUpdated;
    public SimulationState LatestState { get; private set; }

    [Header("Conexión")]
    public string serverUrl = "ws://localhost:8765";
 
    private WebSocket websocket;
 
    async void Start()
    {
        websocket = new WebSocket(serverUrl);
 
        websocket.OnOpen += () =>
        {
            Debug.Log("Conectado al servidor");
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
        // NativeWebSocket necesita procesar su cola de mensajes dentro del
        // ciclo de Unity; sin esta llamada, OnMessage nunca se dispara.
        websocket?.DispatchMessageQueue();
#endif
    }
 
    void HandleMessage(string message)
    {
        SimulationState state = JsonUtility.FromJson<SimulationState>(message);
 
        // Un mensaje mal formado o incompleto se descarta en vez de
        // propagarse a los suscriptores, para no obligar a cada uno de
        // ellos a validar null por separado.
        if (state == null || state.grid == null)
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