using UnityEngine;

public class AICarController : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints
    public float speed = 10f;     // Speed of the car
    public float turnSpeed = 5f;  // Turning speed
    public float waypointThreshold = 1f; // Distance to consider reaching a waypoint

    private int currentWaypointIndex = 0; // Index of the current waypoint

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Get the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move towards the waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Check if the car is close to the waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop through waypoints
        }
    }
}
