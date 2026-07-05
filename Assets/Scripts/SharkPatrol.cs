using UnityEngine;

public class SharkPatrol : MonoBehaviour
{
    [Header("Circular Patrol")]
    public Vector3 centerPoint;
    public float radius = 5f;
    public float speed = 2f;
    public bool clockwise = true;

    [Header("Damage Settings")]
    public float damageToPlayer = 15f;
    public float damageCooldown = 1.5f;

    private float angle = 0f;
    private float lastDamageTime = 0f;

    private void Start()
    {
        // If centerPoint wasn't customized, default to starting position
        if (centerPoint == Vector3.zero)
        {
            centerPoint = transform.position;
        }

        // Calculate initial angle based on starting position relative to centerPoint
        Vector3 offset = transform.position - centerPoint;
        angle = Mathf.Atan2(offset.z, offset.x);
        radius = new Vector2(offset.x, offset.z).magnitude;
        if (radius < 1f) radius = 5f;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
            return;

        // Calculate next angular position
        float directionMultiplier = clockwise ? -1f : 1f;
        angle += directionMultiplier * speed * Time.deltaTime / radius;

        // Position on circle
        float x = centerPoint.x + Mathf.Cos(angle) * radius;
        float z = centerPoint.z + Mathf.Sin(angle) * radius;
        Vector3 newPos = new Vector3(x, transform.position.y, z);

        // Calculate direction vector to face forward along the path
        Vector3 moveDirection = (newPos - transform.position).normalized;
        moveDirection.y = 0f;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        transform.position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DealDamage(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DealDamage(other.gameObject);
        }
    }

    private void DealDamage(GameObject player)
    {
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(damageToPlayer);
                lastDamageTime = Time.time;

                // Simple knockback
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDirection = (player.transform.position - transform.position).normalized;
                    pushDirection.y = 0f;
                    playerRb.AddForce(pushDirection * 8f, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 drawCenter = (centerPoint == Vector3.zero) ? transform.position : centerPoint;
        Gizmos.DrawWireSphere(drawCenter, radius);
    }
}
