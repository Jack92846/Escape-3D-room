using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineNode : MonoBehaviour
{
    [Header("node id")]
    public int nodeId;
    public TimeState nodeTime;

    [Header("appearance")]
    public Material normalMaterial;
    public Material selectedMaterial;
    public Material connectedMaterial;
    public GameObject glowEffect;

    [Header("statu")]
    public bool isStartNode = false;
    public bool isEndNode = false;
    public bool canBeReused = false;

    private Renderer nodeRenderer;
    private bool isSelected = false;
    private bool isConnected = false;
    private TimeLinePuzzle parentPuzzle;

    void Awake()
    {
        nodeRenderer = GetComponent<Renderer>();
        if (nodeRenderer == null)
        {
            nodeRenderer = GetComponentInChildren<Renderer>();
        }
    }

    public void Initialize(TimeLinePuzzle puzzle)
    {
        parentPuzzle = puzzle;
        ResetNode();
    }

    public bool CanBeSelected()
    {
        if (isStartNode) return true;
        if (isConnected && !canBeReused) return false;

        WatchTool watch = FindObjectOfType<WatchTool>();
        if (watch != null && watch.hasUpgraded)
        {
            return true;
        }
        else
        {
            return nodeTime == TimeState.Past || nodeTime == TimeState.Present;
        }
    }

    public void SelectNode(bool selected)
    {
        isSelected = selected;

        if (nodeRenderer != null)
        {
            if (selected)
            {
                nodeRenderer.material = selectedMaterial;
                if (glowEffect != null)
                    glowEffect.SetActive(true);
            }
            else if (isConnected)
            {
                nodeRenderer.material = connectedMaterial;
            }
            else
            {
                nodeRenderer.material = normalMaterial;
                if (glowEffect != null)
                    glowEffect.SetActive(false);
            }
        }
    }

    public void SetConnected(bool connected)
    {
        isConnected = connected;

        if (nodeRenderer != null)
        {
            if (connected)
            {
                nodeRenderer.material = connectedMaterial;
            }
            else
            {
                nodeRenderer.material = normalMaterial;
            }
        }
    }

    public void ResetNode()
    {
        isConnected = false;
        isSelected = false;

        if (nodeRenderer != null)
        {
            nodeRenderer.material = normalMaterial;
        }

        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        if (!isConnected && CanBeSelected())
        {
            if (glowEffect != null)
                glowEffect.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (!isConnected && !isSelected)
        {
            if (glowEffect != null)
                glowEffect.SetActive(false);
        }
    }
}