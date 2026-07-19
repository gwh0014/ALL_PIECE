using UnityEngine;
using System.Collections;

public class TreasureChest : MonoBehaviour, IInteractable
{
    public int goldValue = 1;
    public string prompt = "Open Treasure Chest";
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject treasureVisual;

    private bool isOpened = false;

    public string InteractionPrompt => isOpened ? string.Empty : prompt;

    private void Awake()
    {
        // Auto-detect animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        // Auto-detect the golden treasure inside the chest so we can hide it when opened
        if (treasureVisual == null)
        {
            Transform t = transform.Find("ChestVisualModel/DO_NOT_TOUCH/Mesh/chest_base_g/chest_treasure");
            if (t != null)
            {
                treasureVisual = t.gameObject;
            }
            else
            {
                // Fallback: search all descendants for any GameObject containing "treasure" in its name
                var renderers = GetComponentsInChildren<MeshRenderer>(true);
                foreach (var r in renderers)
                {
                    if (r.name.ToLower().Contains("treasure"))
                    {
                        treasureVisual = r.gameObject;
                        break;
                    }
                }
            }
        }
    }

    public void OnInteract(GameObject instigator)
    {
        if (isOpened) return;
        isOpened = true;

        // Add score to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTreasure(goldValue);
        }

        // Disable collider to prevent further interactions
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Trigger opening animation
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        // Start coroutine to hide the treasure visual after opening
        StartCoroutine(HideTreasureRoutine());
    }

    private IEnumerator HideTreasureRoutine()
    {
        // Wait for a short duration while chest is opening (0.5 seconds is perfect for the animation)
        yield return new WaitForSeconds(0.5f);

        if (treasureVisual != null)
        {
            treasureVisual.SetActive(false);
        }
    }
}
