using UnityEngine;
using UnityEngine.SceneManagement;
using SojaExiles;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game status")]
    public bool isGamePaused = false;
    public bool isGameOver = false;

    [Header("Scene settings")]
    public string mainMenuScene = "MainMenu";
    public string gameScene = "GameScene";

    private MouseLook mouseLook;
    private PlayerMovement playerMovement;

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

    void Start()
    {
        FindPlayerControls();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerControls();

        if (!isGameOver)
        {
            EnablePlayerControls();

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResetAllPanels();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePause();
        }
    }

    void FindPlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player == null)
        {
            player = GameObject.Find("First Person Player");
        }

        if (player != null)
        {
            mouseLook = camera.GetComponent<MouseLook>();
            playerMovement = player.GetComponent<PlayerMovement>();

            if (mouseLook == null)
                Debug.LogWarning("Not found MouseLook component");
            if (playerMovement == null)
                Debug.LogWarning("Not found PlayerMovement component");
        }
        else
        {
            Debug.LogWarning("Not found player component");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);
        isGameOver = false;
        isGamePaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindPlayerControls();
        EnablePlayerControls();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        DisablePlayerControls();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.GameOver();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void DisablePlayerControls()
    {
        if (mouseLook != null)
        {
            mouseLook.DisableLook();
            Debug.Log("Ban MouseLook");
        }

        if (playerMovement != null)
        {
            playerMovement.DisableMovement();
            Debug.Log("Ban PlayerMovement");
        }

        if (mouseLook == null || playerMovement == null)
        {
            FindPlayerControls();
            if (mouseLook != null) mouseLook.DisableLook();
            if (playerMovement != null) playerMovement.DisableMovement();
        }
    }

    void EnablePlayerControls()
    {
        if (mouseLook != null)
        {
            mouseLook.EnableLook();
        }

        if (playerMovement != null)
        {
            playerMovement.EnableMovement();
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        isGamePaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1f;
        isGameOver = false;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TogglePause()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsMenuOpen())
            return;

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (mouseLook != null) mouseLook.DisableLook();
            if (playerMovement != null) playerMovement.DisableMovement();
            if (UIManager.Instance != null)
                UIManager.Instance.ShowMainMenu();
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (mouseLook != null) mouseLook.EnableLook();
            if (playerMovement != null) playerMovement.EnableMovement();
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}