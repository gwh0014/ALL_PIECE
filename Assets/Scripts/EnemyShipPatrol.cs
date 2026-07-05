using UnityEngine;

public class EnemyShipPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float patrolSpeed = 1.6f;
    public float chaseSpeed = 2.6f;
    public float targetReachThreshold = 1.0f;

    [Header("AI Settings")]
    public float detectionRadius = 8f;
    public float damageToPlayer = 20f;
    public float damageCooldown = 1.5f;

    private int currentWaypointIndex = 0;
    private Transform playerTransform;
    private float lastDamageTime = 0f;

    private void Start()
    {
        // Find player in the scene
        if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
        {
            playerTransform = GameManager.Instance.playerShip.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
            return;

        if (playerTransform == null)
        {
            // Try to find player if not assigned yet
            if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
            {
                playerTransform = GameManager.Instance.playerShip.transform;
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPos = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        
        MoveTowards(targetPos, patrolSpeed);

        if (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(targetPos.x, 0f, targetPos.z)) < targetReachThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector3 targetPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        MoveTowards(targetPos, chaseSpeed);
    }

    private void MoveTowards(Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f; // Keep on flat water plane

        if (direction != Vector3.zero)
        {
            // Rotate towards target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3.5f);
        }

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TakeDamage(damageToPlayer);
                    lastDamageTime = Time.time;

                    // Apply a physical bounce-back force to the player
                    Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
                    if (playerRb != null)
                    {
                        Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                        pushDirection.y = 0f;
                        playerRb.AddForce(pushDirection * 10f, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
