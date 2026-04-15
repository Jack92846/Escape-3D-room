using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeExitDoor : MonoBehaviour
{
    [Header("Space time requirement")]
    public TimeState requiredTimeState = TimeState.Past;

    [Header("Door statu")]
    public bool isLocked = true;
    public GameObject doorModel;
    public Collider doorCollider;

    [Header("Effect")]
    public Material unlockedMaterial;
    public AudioClip unlockSound;

    [Header("Game over")]
    public bool triggerGameOver = true;
    public string exitMessage = "You leave the room!";

    void Start()
    {
        UpdateDoorState();
    }

    void Update()
    {
        CheckTimeState();
    }

    void CheckTimeState()
    {
        if (WatchTool.Instance != null && WatchTool.Instance.hasWatch)
        {
            bool isCorrectTime = WatchTool.Instance.currentTime == requiredTimeState;

            if (isCorrectTime && isLocked)
            {
                UnlockDoor();
            }
            else if (!isCorrectTime && !isLocked)
            {
                LockDoor();
            }
        }
    }

    void UnlockDoor()
    {
        isLocked = false;

        if (unlockSound != null)
            AudioSource.PlayClipAtPoint(unlockSound, transform.position);

        if (doorModel != null && unlockedMaterial != null)
            doorModel.GetComponent<Renderer>().material = unlockedMaterial;

        if (doorCollider != null)
            doorCollider.isTrigger = true;

    }

    void LockDoor()
    {
        isLocked = true;

        if (doorCollider != null)
            doorCollider.isTrigger = false;
    }

    void UpdateDoorState()
    {
        if (doorCollider != null)
            doorCollider.isTrigger = !isLocked;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isLocked)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.GameOver();
            }

            DisablePlayerMovement(other.gameObject);
        }
    }
    
    void DisablePlayerMovement(GameObject player)
    {
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script.GetType().Name.Contains("Movement") ||
                script.GetType().Name.Contains("Controller"))
            {
                script.enabled = false;
            }
        }
    }
}