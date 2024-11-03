using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public float healthValue = 20f; // The amount of health this item restores
    public float rotationSpeed = 100f; // The speed at which the health item rotates

    private void Update()
    {
        // Rotate the health item around the z-axis
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.instance.IncreaseHealth(healthValue);
            Destroy(gameObject); // Destroy the health item after pickup
        }
    }
}
