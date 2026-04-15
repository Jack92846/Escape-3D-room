using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add TextMeshPro

public enum TimeState
{
    Past,    // past
    Present, // now
    Future   // future
}

public class WatchTool : MonoBehaviour
{
    public static WatchTool Instance; // Singleton mode for global access

    [Header("Time status")]
    public TimeState currentTime = TimeState.Present;
    public bool hasWatch = false; // Whether get the watch

    [Header("Battery upgrade")]
    public bool hasUpgraded = false; // Whether it has been upgraded to the future time and space
    public GameObject futureUIPrompt; // UI tips for unlocking future time and space

    // UI
    [Header("UI display")]
    public TextMeshProUGUI timeStateText; // Current spatiotemporal state text
    public string pastText = "Past";
    public string presentText = "Present";
    public string futureText = "Future";
    public Color pastColor = new Color(0.7f, 0.7f, 0.7f); // gray
    public Color presentColor = Color.white; // white
    public Color futureColor = new Color(0.5f, 0.8f, 1f); // Light blue

    [Header("Switching effect")]
    public float transitionTime = 2f; // Switching time
    public AudioClip timeShiftSound; // Toggle sound effects
    private AudioSource audioSource; // Add a dedicated audiosource
    public ParticleSystem timeShiftEffect; // Toggle particle effects

    [Header("Particle position settings")]
    public Transform vfxTarget; // Target followed by particles (usually camera)
    public Vector3 localOffset = new Vector3(0, 0, 2f); // Offset from target

    [Header("Particle orientation adjustment")]
    public bool fixParticleDirection = true;
    public Vector3 particleRotation = new Vector3(0, 180f, 0); // Rotate the Y axis 180 degrees

    [Header("Particle level settings")]
    public string particleLayer = "Particles";
    public int particleSortingOrder = 1000;

    [Header("UI hint")]
    public GameObject watchUI; // Watch UI
    public GameObject timeStateUI; // Current spatio-temporal status UI
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;
    public float hintDisplayTime = 2f;

    [Header("UI screen settings")]
    public Canvas overlayCanvas;
    public Image screenMask;
    public float fadeDuration = 0.5f;
    public Color maskColor = Color.white;

    [Header("Spatiotemporal objects")]
    public List<TimeObject> timeObjects = new List<TimeObject>(); // All objects affected by time and space

    private bool isTransitioning = false; // Whether switching is in progress
    private Coroutine currentHintCoroutine;

    void Awake()
    {
        // Singleton initialization
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 2f; // 2D sound effect can be heard in the whole scene
    }

    void Start()
    {
        // Create masking UI
        CreateMaskUI();
        // Configure particle render levels
        ConfigureParticleLayers();

        // Make sure the mask is initially completely transparent
        if (screenMask != null)
        {
            screenMask.color = new Color(maskColor.r, maskColor.g, maskColor.b, 0f);
            Debug.Log("Start: Make sure the mask is transparent, current alpha: " + screenMask.color.a);
        }

        // If no target is specified, the main camera is used by default
        if (vfxTarget == null && Camera.main != null)
        {
            vfxTarget = Camera.main.transform;
        }

        // Set the particle system as a sub object of the target
        if (timeShiftEffect != null && vfxTarget != null)
        {
            timeShiftEffect.transform.SetParent(vfxTarget);
            timeShiftEffect.transform.localPosition = localOffset;
            timeShiftEffect.transform.localRotation = Quaternion.identity;
            // Correct particle direction - rotate 180 degrees
            if (fixParticleDirection)
            {
                timeShiftEffect.transform.localRotation = Quaternion.Euler(particleRotation);
                Debug.Log($"Corrected particle direction, rotation: {particleRotation}");
                Debug.Log($"Particles attached to {vfxTarget.name}，deviation: {localOffset}");
            }
            else
            {
                timeShiftEffect.transform.localRotation = Quaternion.identity;
            }
        }

        // Find and set timestatetext (if not set in the inspector)
        FindAndSetTimeStateText();

        // Update all spatio-temporal objects
        UpdateAllTimeObjects();
        UpdateWatchUI();

        // Update time display
        UpdateTimeStateUI();

        // Initial check whether there is future time and space unlocking
        CheckFutureUnlockStatus();
    }

    // Find and set timestatetext
    void FindAndSetTimeStateText()
    {
        if (timeStateText == null)
        {
            // Try to find by name
            GameObject timeObject = GameObject.Find("Time");
            if (timeObject != null)
            {
                timeStateText = timeObject.GetComponent<TextMeshProUGUI>();
                if (timeStateText == null)
                {
                    // Try to find in sub objects
                    timeStateText = timeObject.GetComponentInChildren<TextMeshProUGUI>();
                }

                if (timeStateText != null)
                {
                    Debug.Log("Time textmeshpro component found successfully");
                }
                else
                {
                    Debug.LogWarning("Textmeshprougui component not found on time object");
                }
            }
            else
            {
                Debug.LogWarning("GameObject named'time'was not found in the scene");
            }
        }
    }

    void Update()
    {
        // Detect switching input (e.g. press T key)
        if (hasWatch && Input.GetKeyDown(KeyCode.T) && !isTransitioning)
        {
            if (hasUpgraded)
            {
                // Upgraded: three space-time cycles
                CycleThroughAllTimes();
            }
            else
            {
                // Not upgraded: only switch between past and present
                ToggleTime();
            }
        }

        // Detect mouse wheel toggle (optional)
        if (hasWatch && Input.GetAxis("Mouse ScrollWheel") != 0 && !isTransitioning)
        {
            if (hasUpgraded)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    ShiftToNextTime();
                else
                    ShiftToPreviousTime();
            }
            else
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    ShiftToFuture();
                else
                    ShiftToPast();
            }
        }
    }

    // Check the unlocking status of future time and space
    void CheckFutureUnlockStatus()
    {
        // If you already have a watch and it has been upgraded, display the future UI prompt
        if (hasUpgraded && futureUIPrompt != null)
        {
            futureUIPrompt.SetActive(true);
        }
    }

    // Three space-time cycles
    void CycleThroughAllTimes()
    {
        if (currentTime == TimeState.Past)
            ShiftToPresent();
        else if (currentTime == TimeState.Present)
            ShiftToFuture();
        else
            ShiftToPast();
    }

    // Switch to the next space-time (for scroll wheel forward)
    void ShiftToNextTime()
    {
        if (currentTime == TimeState.Past)
            ShiftToPresent();
        else if (currentTime == TimeState.Present)
            ShiftToFuture();
        else
            ShiftToPast();
    }

    // Switch to the previous space-time (for wheel reverse)
    void ShiftToPreviousTime()
    {
        if (currentTime == TimeState.Past)
            ShiftToFuture();
        else if (currentTime == TimeState.Present)
            ShiftToPast();
        else
            ShiftToPresent();
    }

    void CreateMaskUI()
    {
        if (overlayCanvas == null)
        {
            GameObject canvasObj = new GameObject("TimeShiftCanvas");
            overlayCanvas = canvasObj.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 900; // Below particles but above scene

            // Add canvas scaler to ensure adaptability to different resolutions
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // Add graphic raycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        if (screenMask == null)
        {
            GameObject maskObj = new GameObject("ScreenMask");
            maskObj.transform.SetParent(overlayCanvas.transform, false);

            screenMask = maskObj.AddComponent<Image>();
            screenMask.color = new Color(maskColor.r, maskColor.g, maskColor.b, 0f);

            RectTransform rt = screenMask.rectTransform;

            rt.anchorMin = Vector2.zero;    // Lower left corner (0,0)
            rt.anchorMax = Vector2.one;     // Upper right corner (1,1)
            rt.offsetMin = Vector2.zero;    // Left/bottom margin
            rt.offsetMax = Vector2.zero;    // Right/top margin

            // Make sure the UI does not block clicks
            screenMask.raycastTarget = false;

            maskObj.SetActive(true); // GameObject to activate
            screenMask.enabled = true; // Image component to enable

            Debug.Log("Create full screen mask, initial alpha:" + screenMask.color.a);
        }
    }

    void ConfigureParticleLayers()
    {
        // Set the particle system to use a special level
        if (WatchTool.Instance != null && WatchTool.Instance.timeShiftEffect != null)
        {
            ParticleSystemRenderer psRenderer =
                WatchTool.Instance.timeShiftEffect.GetComponent<ParticleSystemRenderer>();

            if (psRenderer != null)
            {
                // Set particle rendering above UI
                psRenderer.sortingLayerName = particleLayer;
                psRenderer.sortingOrder = particleSortingOrder;

                // Create a canvas dedicated to particles
                CreateParticleCanvas();
            }
        }
    }

    void CreateParticleCanvas()
    {
        // Create a canvas dedicated to rendering particles
        GameObject particleCanvasObj = new GameObject("ParticleCanvas");
        Canvas particleCanvas = particleCanvasObj.AddComponent<Canvas>();
        particleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        particleCanvas.sortingOrder = particleSortingOrder;
    }

    public IEnumerator PlayMaskEffect()
    {
        Debug.Log("Start UI shielding effect");

        // Fade mask
        yield return StartCoroutine(FadeMask(0f, 1f, fadeDuration));

        // Keep shielded (particle effects visible)
        yield return new WaitForSeconds(0.5f);

        // Fade out mask
        yield return StartCoroutine(FadeMask(1f, 0f, fadeDuration));

        Debug.Log("End of UI shielding effect");
    }

    IEnumerator FadeMask(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        Color fromColor = screenMask.color;
        Color toColor = new Color(maskColor.r, maskColor.g, maskColor.b, toAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            screenMask.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }

        screenMask.color = toColor;
    }

    // Get watch
    public void AcquireWatch()
    {
        hasWatch = true;
        ShowHint("Obtain the Time Watch! Press the T key to switch dimensions", hintDisplayTime);

        // Check whether the battery has been owned (through inventory)
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null && inventory.HasItem(ItemName.Battery))
        {
            UpgradeWatchWithBattery();
        }

        // Show UI tips
        if (watchUI != null)
            watchUI.SetActive(true);

        // Update time display
        UpdateTimeStateUI();

        // Play to get the sound/animation of the watch
        // StartCoroutine(ShowTutorial());
    }

    // Upgrade watch with battery
    public void UpgradeWatchWithBattery()
    {
        if (!hasUpgraded)
        {
            hasUpgraded = true;
            ShowHint("Time Watch Upgrade! Press the T key to switch dimensions", hintDisplayTime);


            // Play upgrade effect
            if (timeShiftSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(timeShiftSound);
            }

            // Show upgrade tips
            if (futureUIPrompt != null)
            {
                futureUIPrompt.SetActive(true);
                StartCoroutine(HideUpgradePrompt());
            }

            // Trigger particle effect
            if (timeShiftEffect != null)
            {
                //timeShiftEffect.Play();
            }

            // Update the time display (update the color if the current time and space is in the future)
            UpdateTimeStateUI();
        }
    }

    // Hide upgrade tips
    IEnumerator HideUpgradePrompt()
    {
        yield return new WaitForSeconds(3f);
        if (futureUIPrompt != null)
        {
            futureUIPrompt.SetActive(false);
        }
    }

    // Switch time and space (past/present)
    public void ToggleTime()
    {
        if (currentTime == TimeState.Past)
            ShiftToPresent();
        else
            ShiftToPast();
    }

    // Switch to the past
    public void ShiftToPast()
    {
        if (isTransitioning || currentTime == TimeState.Past) return;

        StartCoroutine(TimeShiftCoroutine(TimeState.Past));
    }

    // Switch to present
    public void ShiftToPresent()
    {
        if (isTransitioning || currentTime == TimeState.Present) return;

        StartCoroutine(TimeShiftCoroutine(TimeState.Present));
    }

    // Switch to future
    public void ShiftToFuture()
    {
        if (isTransitioning || currentTime == TimeState.Future) return;

        StartCoroutine(TimeShiftCoroutine(TimeState.Future));
    }

    // Spatiotemporal switching process
    IEnumerator TimeShiftCoroutine(TimeState newTime)
    {
        isTransitioning = true;

        // 1. Play toggle sound
        if (timeShiftSound != null && audioSource != null)
        {
            audioSource.clip = timeShiftSound;
            audioSource.Play();
            Debug.Log("Playing time shift sound: " + timeShiftSound.name);
        }
        else
        {
            Debug.LogWarning("Time shift sound or AudioSource is not set!");
        }

        // 2. Start UI shielding effect
        if (screenMask != null)
        {
            StartCoroutine(FadeMask(0f, 1f, fadeDuration / 2));
        }

        // 3. Play particle effects after a short delay
        yield return new WaitForSeconds(fadeDuration / 4);

        // 4. Play particle effects
        if (timeShiftEffect != null)
        {
            // Stop and clean up previous particles
            timeShiftEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Reset particle time (make sure to start from scratch)
            timeShiftEffect.time = 0f;

            // Play a new particle effect
            timeShiftEffect.Play();
        }

        // 5. Wait for particles to play for a while
        yield return new WaitForSeconds(0.5f);

        // 6. Fade white mask
        if (screenMask != null)
        {
            yield return StartCoroutine(FadeMask(1f, 0f, fadeDuration / 2));
        }

        // 7. Wait for the remaining transition time
        float remainingTime = transitionTime - fadeDuration - 0.5f;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // 8. Update spatio-temporal status
        currentTime = newTime;
        UpdateAllTimeObjects();

        // 9. Update UI display (key)
        UpdateTimeStateUI();

        Debug.Log("Space Shift to: " + currentTime);

        isTransitioning = false;
    }

    // Update all objects affected by time and space
    void UpdateAllTimeObjects()
    {
        // If the list is empty, the timeobject in the scene will be found automatically
        if (timeObjects.Count == 0)
        {
            TimeObject[] allTimeObjects = FindObjectsOfType<TimeObject>();
            timeObjects.AddRange(allTimeObjects);
        }

        foreach (TimeObject timeObj in timeObjects)
        {
            if (timeObj != null)
                timeObj.UpdateForTimeState(currentTime);
        }
    }

    // Register spatiotemporal objects
    public void RegisterTimeObject(TimeObject timeObject)
    {
        if (!timeObjects.Contains(timeObject))
            timeObjects.Add(timeObject);
    }

    // Cancel spatiotemporal objects
    public void UnregisterTimeObject(TimeObject timeObject)
    {
        if (timeObjects.Contains(timeObject))
            timeObjects.Remove(timeObject);
    }

    // Update watch UI display
    void UpdateWatchUI()
    {
        if (watchUI != null)
            watchUI.SetActive(hasWatch);
    }

    // Update time status UI (main method)
    void UpdateTimeStateUI()
    {
        if (timeStateText != null)
        {
            // Set text based on current time and space
            switch (currentTime)
            {
                case TimeState.Past:
                    timeStateText.text = pastText;
                    timeStateText.color = pastColor;
                    break;
                case TimeState.Present:
                    timeStateText.text = presentText;
                    timeStateText.color = presentColor;
                    break;
                case TimeState.Future:
                    timeStateText.text = futureText;
                    timeStateText.color = futureColor;
                    break;
            }

            // If the watch has not been obtained, a gray prompt is displayed
            if (!hasWatch)
            {
                timeStateText.text = "???";
                timeStateText.color = Color.gray;
            }

            Debug.Log($"Update time display: {timeStateText.text}");
        }
        else
        {
            // If not, try to find it again
            FindAndSetTimeStateText();
            if (timeStateText != null)
            {
                // Recursive call once
                UpdateTimeStateUI();
            }
        }

        // If there is an additional spatio-temporal status UI, update it as well
        if (timeStateUI != null)
        {
            // Other UI elements can be updated here
        }
    }

    // Check whether it is in the specified time and space
    public bool IsInTimeState(TimeState state)
    {
        return currentTime == state;
    }

    // Expose methods for external calls to refresh UI
    public void RefreshTimeUI()
    {
        UpdateTimeStateUI();
    }

    void ShowHint(string message, float displayTime)
    {
        if (hintText != null)
        {
            hintText.text = message;
        }

        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
        }

        if (currentHintCoroutine != null)
            StopCoroutine(currentHintCoroutine);
        currentHintCoroutine = StartCoroutine(AutoHideHint(displayTime));

    }

        IEnumerator AutoHideHint(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (hintPanel != null)
                hintPanel.SetActive(false);

            if (hintText != null)
                hintText.text = "";
        }
}