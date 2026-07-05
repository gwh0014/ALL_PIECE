using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable
{
    public int goldValue = 1;
    public string prompt = "Open Treasure Chest";

    public string InteractionPrompt => prompt;

    public void OnInteract(GameObject instigator)
    {
        // Add score to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTreasure(goldValue);
        }

        // Destroy the chest
        Destroy(gameObject);
    }
}
