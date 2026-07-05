using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractionDetector : MonoBehaviour
{
    public float interactionRadius = 2.5f;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable closestInteractable;
    private InputAction interactAction;

    private void Start()
    {
        if (InputSystem.actions != null)
        {
            // Set interactAction to Player/Jump (Spacebar)
            interactAction = InputSystem.actions.FindAction("Player/Jump");
        }
    }

    private void Update()
    {
        FindClosestInteractable();

        // Check for interact key press
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (closestInteractable != null)
            {
                closestInteractable.OnInteract(gameObject);
                FindClosestInteractable(); // Refresh after interaction
            }
        }
    }

    private void FindClosestInteractable()
    {
        // Clean up any destroyed objects
        nearbyInteractables.RemoveAll(x => x == null || (x as MonoBehaviour) == null);

        if (nearbyInteractables.Count == 0)
        {
            closestInteractable = null;
            return;
        }

        IInteractable closest = null;
        float minDistance = float.MaxValue;

        foreach (var interactable in nearbyInteractables)
        {
            MonoBehaviour mb = interactable as MonoBehaviour;
            if (mb == null) continue;

            float dist = Vector3.Distance(transform.position, mb.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = interactable;
            }
        }

        closestInteractable = closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            if (!nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
        }
    }

    public IInteractable GetClosestInteractable()
    {
        return closestInteractable;
    }

    public string GetCurrentPrompt()
    {
        if (closestInteractable != null)
        {
            return closestInteractable.InteractionPrompt;
        }
        return string.Empty;
    }
}
