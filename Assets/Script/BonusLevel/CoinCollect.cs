using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int coinValue = 10;
    public float rotationSpeed = 100f;

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            

            if (BonusLevel.instance != null)
            {
                BonusLevel.instance.ShowPlus10();
            }

            Destroy(gameObject);
        }
    }
}