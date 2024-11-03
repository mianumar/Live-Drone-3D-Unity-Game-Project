using UnityEngine;
using UnityEngine.AI;

public class CivilianAI : MonoBehaviour
{
    public float wanderRadius = 20f;
    public float wanderTimer = 10f;
    public float maxSoundRadius = 20f;
    public float walkSpeedThreshold = 1f;
    public float speedThreshold = 0.1f;
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 wanderTarget;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderTimer;
        Wander();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Wander();
            timer = 0;
        }

        float currentSpeed = agent.velocity.magnitude;
        if (currentSpeed > 0.01f)
        {
            animator.SetFloat("Forward", Mathf.Clamp(currentSpeed / agent.speed, 0f, 0.5f));
        }

        Vector3 direction = wanderTarget - transform.position;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                wanderTarget = hit.position;
                agent.SetDestination(wanderTarget);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxSoundRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wanderTarget, 0.5f);
    }
}
