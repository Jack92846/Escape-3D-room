using UnityEngine;
using System;
using SojaExiles;

[Serializable]
public class GameSettings
{
    [Header("Picture Settings")]
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public bool fullscreen = true;
    public int qualityLevel = 2;

    [Header("Volume Settings")]
    public float masterVolume = 0.8f;
    public float sfxVolume = 0.8f;
    public float musicVolume = 0.5f;

    [Header("Control Settings")]
    public float mouseSensitivity = 100f;
    public bool invertY = false;

    [Header("Game Settings")]
    public bool showHints = true;
    public float difficultyLevel = 1f;

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt("ResolutionWidth", resolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", resolutionHeight);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
        PlayerPrefs.SetInt("ShowHints", showHints ? 1 : 0);
        PlayerPrefs.SetFloat("Difficulty", difficultyLevel);
        PlayerPrefs.Save();
    }

    public void LoadFromPlayerPrefs()
    {
        resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
        invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
        showHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
        difficultyLevel = PlayerPrefs.GetFloat("Difficulty", 1f);
    }

    public void ApplySettings()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, fullscreen);

        QualitySettings.SetQualityLevel(qualityLevel);

        AudioListener.volume = masterVolume;

        MouseLook mouseLook = GameObject.FindObjectOfType<MouseLook>();
        if (mouseLook != null)
        {
            mouseLook.mouseXSensitivity = mouseSensitivity;
            mouseLook.mouseYSensitivity = mouseSensitivity;
        }
    }
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public GameSettings currentSettings = new GameSettings();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplySettings();
    }

    public void LoadSettings()
    {
        currentSettings.LoadFromPlayerPrefs();
    }

    public void SaveSettings()
    {
        currentSettings.SaveToPlayerPrefs();
    }

    public void ApplySettings()
    {
        currentSettings.ApplySettings();
    }

    public void ResetToDefaults()
    {
        currentSettings = new GameSettings();
        SaveSettings();
        ApplySettings();
    }
}