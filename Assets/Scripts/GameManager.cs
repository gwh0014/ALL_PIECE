using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, GameOver, Victory }

    [Header("Player Status")]
    public float maxHullHealth = 100f;
    public float currentHullHealth = 100f;
    public int collectedTreasure = 0;
    public int targetTreasure = 3;

    [Header("Level Configurations")]
    public int currentLevel = 1;
    public GameObject level1Group;
    public GameObject level2Group;
    public GameObject level3Group;
    public Transform playerStartPointLevel1;
    public Transform playerStartPointLevel2;
    public Transform playerStartPointLevel3;

    [Header("Game State")]
    public GameState currentState = GameState.Playing;

    public GameObject playerShip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentHullHealth = maxHullHealth;
        collectedTreasure = 0;
        currentState = GameState.Playing;
        LoadLevel(currentLevel);
    }

    public void AddTreasure(int amount)
    {
        if (currentState != GameState.Playing) return;

        collectedTreasure += amount;
        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlayCollect();
        }

        Debug.Log($"Collected Treasure: {collectedTreasure}/{targetTreasure}");
    }

    public void TakeDamage(float damage)
    {
        if (currentState != GameState.Playing) return;

        currentHullHealth -= damage;
        currentHullHealth = Mathf.Clamp(currentHullHealth, 0f, maxHullHealth);

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlayDamage();
        }

        Debug.Log($"Ship took damage! Hull Health: {currentHullHealth}%");

        if (currentHullHealth <= 0f)
        {
            TriggerGameOver();
        }
    }

    public void RepairHull(float amount)
    {
        if (currentState != GameState.Playing) return;

        currentHullHealth += amount;
        currentHullHealth = Mathf.Clamp(currentHullHealth, 0f, maxHullHealth);

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlayRepair();
        }

        Debug.Log($"Ship repaired! Hull Health: {currentHullHealth}%");
    }

    public void CompleteLevel()
    {
        if (currentState != GameState.Playing) return;

        if (collectedTreasure < targetTreasure)
        {
            Debug.Log("Not enough treasure to complete the level!");
            return;
        }

        if (currentLevel < 3)
        {
            currentLevel++;
            LoadLevel(currentLevel);
            if (SimpleAudioManager.Instance != null)
            {
                SimpleAudioManager.Instance.PlayWin();
            }
        }
        else
        {
            TriggerVictory();
        }
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        collectedTreasure = 0;

        // Level setup parameters
        if (level == 1)
        {
            targetTreasure = 2; // Level 1 is easy: 2 chests
            if (level1Group != null) level1Group.SetActive(true);
            if (level2Group != null) level2Group.SetActive(false);
            if (level3Group != null) level3Group.SetActive(false);
            TeleportPlayer(playerStartPointLevel1);
        }
        else if (level == 2)
        {
            targetTreasure = 3; // Level 2: 3 chests
            if (level1Group != null) level1Group.SetActive(false);
            if (level2Group != null) level2Group.SetActive(true);
            if (level3Group != null) level3Group.SetActive(false);
            TeleportPlayer(playerStartPointLevel2);
        }
        else if (level == 3)
        {
            targetTreasure = 4; // Level 3: 4 chests, harder
            if (level1Group != null) level1Group.SetActive(false);
            if (level2Group != null) level2Group.SetActive(false);
            if (level3Group != null) level3Group.SetActive(true);
            TeleportPlayer(playerStartPointLevel3);
        }

        Debug.Log($"Loaded Level {level}. Goal: Collect {targetTreasure} treasures.");
    }

    private void TeleportPlayer(Transform startPoint)
    {
        if (playerShip != null)
        {
            Rigidbody rb = playerShip.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (startPoint != null)
            {
                playerShip.transform.position = startPoint.position;
                playerShip.transform.rotation = startPoint.rotation;
            }
            else
            {
                playerShip.transform.position = Vector3.zero;
                playerShip.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void TriggerGameOver()
    {
        currentState = GameState.GameOver;
        Debug.Log("GAME OVER: Your ship has sunk!");
    }

    private void TriggerVictory()
    {
        currentState = GameState.Victory;
        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlayWin();
        }
        Debug.Log("VICTORY! You are the Pirate King!");
    }

    public void RestartGame()
    {
        // Reload the scene to reset all destroyed/collected objects, enemies, player position, etc.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
