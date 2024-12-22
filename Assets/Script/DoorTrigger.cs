using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Transform door;       // Door GameObject ka reference
    public float moveSpeed = 2f; // Door ki movement ki speed
    public Vector3 doorOpenPosition;  // Door ki neeche ki position
    public Vector3 doorClosePosition; // Door ki upar ki position

    private void Start()
    {
        // Initial position set karna
        doorClosePosition = door.position;
        doorOpenPosition = doorClosePosition - new Vector3(0, 16f, 0); // 3 units neeche
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();  // Purani movement ko roko
            StartCoroutine(MoveDoor(doorOpenPosition)); // Door neeche hato
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();  // Purani movement ko roko
            StartCoroutine(MoveDoor(doorClosePosition)); // Door wapas upar jao
        }
    }

    System.Collections.IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(door.position, targetPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
