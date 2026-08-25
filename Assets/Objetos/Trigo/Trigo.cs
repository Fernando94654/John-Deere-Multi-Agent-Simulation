using UnityEngine;

public class Trigo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {   
        // Rotate the object by 20 degrees
        gameObject.transform.Rotate(0, 20, 0);
        // Destroy the object
        Destroy(gameObject);
    }
}
