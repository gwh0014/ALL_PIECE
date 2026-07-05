using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PirateShipController : MonoBehaviour
{
    [Header("Sailing Physics")]
    public float maxForwardSpeed = 12f;
    public float forwardAcceleration = 8f;
    public float turnSpeed = 120f;
    public float waterDrag = 1.5f;
    public float waterAngularDrag = 2.5f;

    [Header("Wind Boost")]
    public float boostMultiplier = 1.8f;
    public float boostDuration = 1.0f;
    public float boostCooldown = 3.0f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private float currentSpeed;
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isBoosting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = waterDrag;
        rb.angularDamping = waterAngularDrag;

        // Find actions from the project-wide InputSystem actions
        if (InputSystem.actions != null)
        {
            moveAction = InputSystem.actions.FindAction("Player/Move");
            // Set jumpAction (Wind Boost) to Player/Interact (E)
            jumpAction = InputSystem.actions.FindAction("Player/Interact");
        }
        else
        {
            Debug.LogWarning("InputSystem.actions is null! Make sure the Input System package is active.");
        }
    }

    private void Update()
    {
        // Handle Cooldown and Boost timers
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Jump/Action triggers Wind Boost
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && cooldownTimer <= 0f && !isBoosting)
        {
            TriggerWindBoost();
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = Vector2.zero;
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }

        // 1. Handling Rotation / Steering
        // A/D or Left/Right steers (input.x)
        if (Mathf.Abs(input.x) > 0.05f)
        {
            float rotationAmount = input.x * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // 2. Handling Forward / Sail Power
        // W adds forward sail power (input.y > 0)
        // S brakes/drops sail (input.y < 0)
        float forwardInput = Mathf.Clamp01(input.y); // Only sail forward or stop, sailing backwards in a pirate ship is slow
        float backwardInput = Mathf.Clamp01(-input.y); // Use S for braking / slow reverse

        float targetSpeed = forwardInput * maxForwardSpeed;
        if (isBoosting)
        {
            targetSpeed *= boostMultiplier;
        }

        if (backwardInput > 0.05f)
        {
            // Apply braking force
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * waterDrag * 3f);
        }
        else if (forwardInput > 0.05f)
        {
            // Apply forward force in direction of facing
            Vector3 forwardForce = transform.forward * forwardAcceleration * forwardInput;
            if (isBoosting)
            {
                forwardForce *= boostMultiplier;
            }

            // Cap the max velocity on the horizontal plane
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude < targetSpeed)
            {
                rb.AddForce(forwardForce, ForceMode.Acceleration);
            }
        }

        // Ensure we don't fly off the water level (keep ship Y coordinate stable, but let physics handle collisions)
        Vector3 pos = transform.position;
        if (pos.y < -0.1f || pos.y > 0.1f)
        {
            pos.y = Mathf.Lerp(pos.y, 0f, Time.fixedDeltaTime * 10f);
            transform.position = pos;
        }
    }

    private void TriggerWindBoost()
    {
        isBoosting = true;
        boostTimer = boostDuration;
        cooldownTimer = boostCooldown;

        // Play SFX
        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlayRepair(); // plays a high pitch swoosh as boost
        }

        // Add an instant forward impulse
        rb.AddForce(transform.forward * maxForwardSpeed * 0.5f, ForceMode.VelocityChange);
    }

    public bool IsBoosting() => isBoosting;
    public float GetBoostCooldownPercentage()
    {
        if (boostCooldown <= 0) return 0f;
        return Mathf.Clamp01(cooldownTimer / boostCooldown);
    }
}
