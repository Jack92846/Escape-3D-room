using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panel reference")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject menuPanel;
    public GameObject settingsPanel;

    [Header("Button reference")]
    public Button inGameMenuButton;
    public Button restartButton;
    public Button gameleaveButton;

    [Header("Main panel button")]
    public Button settingsBtn;
    public Button saveBtn;
    public Button loadBtn;
    public Button quitBtn;
    public Button backBtn;
    public Button resumeBtn;

    [Header("Setting panel")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public Slider mouseSensitivitySlider;
    public TMP_Text sensitivityValueText;
    public Button applySettingsBtn;
    public Button closeSettingsBtn;

    [Header("Hint message")]
    public GameObject messagePanel;
    public TMP_Text messageText;
    public float messageDuration = 2f;

    private bool isMenuOpen = false;
    private bool wasGamePaused = false;
    private bool isEscHandled = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel == null)
        {
            AutoFindReferences();
        }

        ResetAllPanels();

        BindButtons();

        LoadSettingsToUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void AutoFindReferences()
    {
        Canvas canvas = GameObject.Find("Canvas_Second")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError("Not found Canvas！");
            return;
        }

        gameOverPanel = FindChildByName(canvas.transform, "GameOverPanel");
        menuPanel = FindChildByName(canvas.transform, "MenuPanel");
        settingsPanel = FindChildByName(canvas.transform, "SettingsPanel");
        pausePanel = FindChildByName(canvas.transform, "MenuPanel");
        messagePanel = FindChildByName(canvas.transform, "HintPanel");

        inGameMenuButton = FindButtonByName(canvas.transform, "MenuButton");
        restartButton = FindButtonByName(canvas.transform, "RestartButton");
        gameleaveButton = FindButtonByName(canvas.transform, "GameleaveButton");
        settingsBtn = FindButtonByName(canvas.transform, "Setting");
        saveBtn = FindButtonByName(canvas.transform, "Save");
        loadBtn = FindButtonByName(canvas.transform, "Read");
        quitBtn = FindButtonByName(canvas.transform, "Exit");
        backBtn = FindButtonByName(canvas.transform, "Return");
        resumeBtn = FindButtonByName(canvas.transform, "Return");

        masterVolumeSlider = FindSliderByName(canvas.transform, "MASlider");
        sfxVolumeSlider = FindSliderByName(canvas.transform, "SVSlider");
        musicVolumeSlider = FindSliderByName(canvas.transform, "VSlider");
        fullscreenToggle = FindToggleByName(canvas.transform, "Toggle");
        qualityDropdown = FindDropdownByName(canvas.transform, "Dropdown");
        mouseSensitivitySlider = FindSliderByName(canvas.transform, "MSlider");
        applySettingsBtn = FindButtonByName(canvas.transform, "Apply");
        closeSettingsBtn = FindButtonByName(canvas.transform, "Cancel");

    }

    private GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    private Button FindButtonByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<Button>() : null;
    }

    private Slider FindSliderByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<Slider>() : null;
    }

    private Toggle FindToggleByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<Toggle>() : null;
    }

    private TMP_Dropdown FindDropdownByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<TMP_Dropdown>() : null;
    }

    private TMP_Text FindTextByName(Transform parent, string name)
    {
        GameObject obj = FindChildByName(parent, name);
        return obj != null ? obj.GetComponent<TMP_Text>() : null;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetAllPanels();

        StartCoroutine(DelayedFindReferences());
        
        isMenuOpen = false;
        wasGamePaused = false;
        Time.timeScale = 1f;
    }


    IEnumerator DelayedFindReferences()
    {
        yield return null;
        yield return null;

        AutoFindReferences();

        BindButtons();

        LoadSettingsToUI();
    }



    IEnumerator RebindButtonsAfterLoad()
    {
        yield return null;
        BindButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isEscHandled)
            {
                isEscHandled = true;
                HandleEscapeKey();
                StartCoroutine(ResetEscHandled());
            }
        }
        else
        {
            isEscHandled = false;
        }
    }

    IEnumerator ResetEscHandled()
    {
        yield return new WaitForSeconds(0.2f);
        isEscHandled = false;
    }

    void HandleEscapeKey()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            if (isMenuOpen)
            {
                HideMenuPanel();
            }
            return;
        }

        if (!isMenuOpen)
        {
            ShowMainMenu();
        }
        else
        {
            ResumeGame();
        }
    }

    void BindButtons()
    {
        if (inGameMenuButton != null)
        {
            inGameMenuButton.onClick.RemoveAllListeners();
            inGameMenuButton.onClick.AddListener(ToggleInGameMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        if (gameleaveButton != null)
        {
            gameleaveButton.onClick.RemoveAllListeners();
            gameleaveButton.onClick.AddListener(LeaveGame);
        }

        if (settingsBtn != null)
        {
            settingsBtn.onClick.RemoveAllListeners();
            settingsBtn.onClick.AddListener(ShowSettings);
        }

        if (saveBtn != null)
        {
            saveBtn.onClick.RemoveAllListeners();
            saveBtn.onClick.AddListener(SaveGame);
        }

        if (loadBtn != null)
        {
            loadBtn.onClick.RemoveAllListeners();
            loadBtn.onClick.AddListener(LoadGame);
        }

        if (quitBtn != null)
        {
            quitBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.AddListener(QuitGame);
        }

        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(HideMenuPanel);
        }

        if (resumeBtn != null)
        {
            resumeBtn.onClick.RemoveAllListeners();
            resumeBtn.onClick.AddListener(ResumeGame);
        }

        if (applySettingsBtn != null)
        {
            applySettingsBtn.onClick.RemoveAllListeners();
            applySettingsBtn.onClick.AddListener(ApplySettings);
        }

        if (closeSettingsBtn != null)
        {
            closeSettingsBtn.onClick.RemoveAllListeners();
            closeSettingsBtn.onClick.AddListener(CloseSettings);
        }

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    public void LeaveGame()
    {
        ShowMessage("Leaving Game ...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ResetAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (messagePanel != null) messagePanel.SetActive(false);

        isMenuOpen = false;
        wasGamePaused = false;

        Time.timeScale = 1f;

        if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    #region game menu function

    public void ToggleInGameMenu()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        if (isMenuOpen)
            ResumeGame();
        else
            ShowMainMenu();
    }

    public void ResumeGame()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isMenuOpen = false;

        if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        wasGamePaused = false;
    }

    #endregion

    #region game ending function

    public void ShowGameOverPanel()
    {
        ResetAllPanels();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetGame();
        }

        ResetAllPanels();

        Time.timeScale = 1f;

        StartCoroutine(DelayedRestart());
     }

     private IEnumerator DelayedRestart()
     {
         yield return null;

         if (GameManager.Instance != null)
         {
             GameManager.Instance.RestartGame();
         }
         else
         {
             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         }

         yield return null;

         ResetAllPanels();
         isMenuOpen = false;
         wasGamePaused = false;
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;

         StartCoroutine(DelayedFindReferences());
     }

    #endregion

    #region menu function

    public void ShowMainMenu()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            isMenuOpen = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        wasGamePaused = true;
    }

    public void HideMenuPanel()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isMenuOpen = false;

        if (GameManager.Instance != null && !GameManager.Instance.isGameOver && wasGamePaused)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            wasGamePaused = false;
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame();
        ShowMessage("Save Game！");
    }

    public void LoadGame()
    {
        ResetAllPanels();

        if (SaveSystem.LoadGame())
        {
            ShowMessage("Already read Game！");
            HideMenuPanel();
        }
        else
        {
            ShowMessage("Not found saving file");
        }
    }

    public void QuitGame()
    {
        ShowMessage("EXIT GAME NOW...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion

    #region settings function

    public void ShowSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadSettingsToUI();
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void LoadSettingsToUI()
    {
        if (SettingsManager.Instance != null)
        {
            GameSettings settings = SettingsManager.Instance.currentSettings;

            if (masterVolumeSlider != null)
                masterVolumeSlider.value = settings.masterVolume;

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = settings.sfxVolume;

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = settings.musicVolume;

            if (fullscreenToggle != null)
                fullscreenToggle.isOn = settings.fullscreen;

            if (qualityDropdown != null)
                qualityDropdown.value = settings.qualityLevel;

            if (mouseSensitivitySlider != null)
            {
                mouseSensitivitySlider.value = settings.mouseSensitivity;
                if (sensitivityValueText != null)
                    sensitivityValueText.text = Mathf.RoundToInt(settings.mouseSensitivity).ToString();
            }
        }
    }

    void ApplySettings()
    {
        if (SettingsManager.Instance != null)
        {
            GameSettings settings = SettingsManager.Instance.currentSettings;

            if (masterVolumeSlider != null)
                settings.masterVolume = masterVolumeSlider.value;

            if (sfxVolumeSlider != null)
                settings.sfxVolume = sfxVolumeSlider.value;

            if (musicVolumeSlider != null)
                settings.musicVolume = musicVolumeSlider.value;

            if (fullscreenToggle != null)
                settings.fullscreen = fullscreenToggle.isOn;

            if (qualityDropdown != null)
                settings.qualityLevel = qualityDropdown.value;

            if (mouseSensitivitySlider != null)
                settings.mouseSensitivity = mouseSensitivitySlider.value;

            SettingsManager.Instance.SaveSettings();
            SettingsManager.Instance.ApplySettings();

            ShowMessage("Apply Settings！");
        }
    }

    void OnSensitivityChanged(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = Mathf.RoundToInt(value).ToString();
    }

    void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    #endregion

    #region help function

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }

    #endregion
}
