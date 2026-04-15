using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text finalTimeText;

    [Header("Game Setting")]
    public float gameStartTime = 0f;

    private int currentScore = 0;
    private float currentTime;
    private bool isGameRunning = true;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        StartCoroutine(DelayedFindUIReferences());

        currentTime = gameStartTime;
        UpdateScoreUI();
        UpdateTimerUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFindUIReferences());
    }

    IEnumerator DelayedFindUIReferences()
    {
        yield return null;
        yield return null;

        AutoFindUIReferences();

        UpdateScoreUI();
        UpdateTimerUI();
    }

    void AutoFindUIReferences()
    {
        Canvas canvas = GameObject.Find("Canvas_Second")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas == null)
        {
            return;
        }

        gameOverPanel = FindChildByName(canvas.transform, "GameOverPanel");
        scoreText = FindTextByName(canvas.transform, "ScoreText");
        timerText = FindTextByName(canvas.transform, "InventoryTitle");
        finalScoreText = FindTextByName(canvas.transform, "FinalScoreText");
        finalTimeText = FindTextByName(canvas.transform, "FinalTimeText");

    }

    GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    TMP_Text FindTextByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<TMP_Text>() : null;
    }

    void Update()
    {
        if (isGameRunning && !gameEnded)
        {
            currentTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void AddScore(int points)
    {
        if (!gameEnded)
        {
            currentScore += points;
            UpdateScoreUI();
        }
    }

    public int GetItemScore(ItemName itemName)
    {
        switch (itemName)
        {
            case ItemName.Battery:
                return 50;
            case ItemName.Key:
                return 30;
            case ItemName.TimeWatch:
                return 100;
            default:
                return 20;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        isGameRunning = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {currentScore}";

            if (finalTimeText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                finalTimeText.text = string.Format("Game Time: {0:00}:{1:00}", minutes, seconds);
            }
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void SetScore(int score)
    {
        currentScore = score;
        UpdateScoreUI();
    }

    public void SetTime(float time)
    {
        currentTime = time;
        UpdateTimerUI();
    }

    public void ResetGame()
    {
        currentScore = 0;
        currentTime = gameStartTime;
        gameEnded = false;
        isGameRunning = true;
        UpdateScoreUI();
        UpdateTimerUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}