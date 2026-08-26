using UnityEngine;

public class FieldBehaviour : MonoBehaviour
{
    public string wheatTag = "Wheat";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tractor")|| other.CompareTag("harvester"))
        {
            Debug.Log($"FieldBehaviour: OnTriggerEnter with {other.gameObject.name} (tag: {other.gameObject.tag})");
            Destroy(gameObject);
        }
    }
}