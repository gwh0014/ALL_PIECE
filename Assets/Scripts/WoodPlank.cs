using UnityEngine;

public class WoodPlank : MonoBehaviour, IInteractable
{
    public float repairAmount = 25f;
    public string prompt = "Salvage Wood Planks";

    public string InteractionPrompt => prompt;

    public void OnInteract(GameObject instigator)
    {
        if (GameManager.Instance != null)
        {
            // If already at full health, we can still collect or warn
            GameManager.Instance.RepairHull(repairAmount);
        }

        // Destroy the planks
        Destroy(gameObject);
    }
}
