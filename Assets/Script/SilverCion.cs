using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverCion : MonoBehaviour
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
            LastLevelScoe.instance.IncreaseHealth(healthValue);
            Destroy(gameObject); // Destroy the health item after pickup
        }
    }
}
