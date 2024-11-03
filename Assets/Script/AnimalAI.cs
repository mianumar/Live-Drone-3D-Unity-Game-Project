using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    public float wanderRadius = 20f;   // How far the animal can wander
    public float wanderTimer = 5f;     // Time to wait before moving again

    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator;         // Reference to Animator

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();  // Get Animator component
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        // Trigger animations based on movement
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);  // Trigger walking animation
        }
        else
        {
            animator.SetBool("isWalking", false); // Trigger idle animation
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
