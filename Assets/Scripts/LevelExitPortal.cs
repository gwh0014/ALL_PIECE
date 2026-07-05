using UnityEngine;

public class LevelExitPortal : MonoBehaviour, IInteractable
{
    public string basePrompt = "Sail to Safe Port";

    public string InteractionPrompt
    {
        get
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.collectedTreasure >= GameManager.Instance.targetTreasure)
                {
                    return basePrompt;
                }
                else
                {
                    return $"Safe Port (Need {GameManager.Instance.targetTreasure - GameManager.Instance.collectedTreasure} more Treasure!)";
                }
            }
            return basePrompt;
        }
    }

    public void OnInteract(GameObject instigator)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.collectedTreasure >= GameManager.Instance.targetTreasure)
            {
                GameManager.Instance.CompleteLevel();
            }
            else
            {
                Debug.Log("Not enough treasure to complete the level yet!");
            }
        }
    }
}
