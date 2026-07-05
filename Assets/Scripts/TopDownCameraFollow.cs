using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 12f, -8.5f);
    public float smoothSpeed = 5f;

    private void Start()
    {
        // Auto-detect player if target is null
        if (target == null && GameManager.Instance != null && GameManager.Instance.playerShip != null)
        {
            target = GameManager.Instance.playerShip.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
            {
                target = GameManager.Instance.playerShip.transform;
            }
            return;
        }

        Vector3 targetPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Force a top-down angled look
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}
