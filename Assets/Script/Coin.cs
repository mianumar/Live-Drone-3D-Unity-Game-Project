using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 10; // Her coin kitne points de ga
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
            ScoreManager.instance.AddScore(coinValue);
            Destroy(gameObject); // Coin ko destroy kar de ta ke dobara na mile
        }
    }
}
