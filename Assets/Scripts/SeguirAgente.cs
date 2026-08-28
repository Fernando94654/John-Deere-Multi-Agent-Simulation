using UnityEngine;

// Mantiene la camara sobre el agente durante el Play
public class SeguirAgente : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 14f, -22f);

    private Transform agente;

    void Start()
    {
        Harvester h = FindFirstObjectByType<Harvester>();
        if (h != null) { agente = h.transform; return; }

        Tractor t = FindFirstObjectByType<Tractor>();
        if (t != null) agente = t.transform;
    }

    void LateUpdate()
    {
        if (agente == null) return;

        transform.position = agente.position + offset;
        transform.LookAt(agente);
    }
}
