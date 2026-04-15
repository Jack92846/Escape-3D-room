using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLinePuzzle : Interactive
{
    [Header("puzzle node")]
    public List<TimeLineNode> pastNodes;
    public List<TimeLineNode> presentNodes;
    public List<TimeLineNode> futureNodes;

    [Header("line settings")]
    public LineRenderer lineRendererPrefab;
    public Material correctLineMaterial;
    public Material wrongLineMaterial;
    public float nodeRadius = 1.5f;
    public LayerMask nodeLayer;

    [Header("puzzle answer")]
    public List<TimeNodeConnection> correctConnections;

    [Header("sloved statu")]
    public bool puzzleSolved = false;
    public GameObject successEffect;
    public AudioClip connectSound;
    public AudioClip successSound;

    private TimeLineNode currentSelectedNode;
    private List<GameObject> currentLines = new List<GameObject>();
    private List<TimeNodeConnection> playerConnections = new List<TimeNodeConnection>();
    private Camera mainCamera;
    private bool isDrawing = false;
    private WatchTool watchTool;

    void Start()
    {
        mainCamera = Camera.main;
        watchTool = FindObjectOfType<WatchTool>();

        InitializeNodes(pastNodes);
        InitializeNodes(presentNodes);
        InitializeNodes(futureNodes);
    }

    void InitializeNodes(List<TimeLineNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node != null)
            {
                node.Initialize(this);
            }
        }
    }

    void Update()
    {
        if (puzzleSolved) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }

        if (isDrawing && currentSelectedNode != null)
        {
            UpdateLineToMouse();
        }
    }

    void StartDrawing()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, nodeLayer))
        {
            TimeLineNode node = hit.collider.GetComponent<TimeLineNode>();
            if (node != null && node.CanBeSelected())
            {
                currentSelectedNode = node;
                isDrawing = true;

                node.SelectNode(true);

                CreateNewLine(node.transform.position);
            }
        }
    }

    void StopDrawing()
    {
        if (isDrawing && currentSelectedNode != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, nodeLayer))
            {
                TimeLineNode endNode = hit.collider.GetComponent<TimeLineNode>();
                if (endNode != null && endNode != currentSelectedNode && endNode.CanBeSelected())
                {
                    TryConnectNodes(currentSelectedNode, endNode);
                }
            }

            currentSelectedNode.SelectNode(false);
            currentSelectedNode = null;
        }

        isDrawing = false;

        if (currentLines.Count > 0)
        {
            GameObject lastLine = currentLines[currentLines.Count - 1];
            if (lastLine != null && lastLine.GetComponent<LineRenderer>().positionCount < 2)
            {
                Destroy(lastLine);
                currentLines.RemoveAt(currentLines.Count - 1);
            }
        }
    }

    void CreateNewLine(Vector3 startPos)
    {
        GameObject lineObj = new GameObject("TempLine");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, startPos);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = wrongLineMaterial;

        currentLines.Add(lineObj);
    }

    void UpdateLineToMouse()
    {
        if (currentLines.Count == 0) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 endPos;
        if (Physics.Raycast(ray, out hit, 100f, nodeLayer))
        {
            endPos = hit.point;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, currentSelectedNode.transform.position);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                endPos = ray.GetPoint(distance);
            }
            else
            {
                endPos = ray.GetPoint(10f);
            }
        }

        LineRenderer currentLine = currentLines[currentLines.Count - 1].GetComponent<LineRenderer>();
        currentLine.SetPosition(1, endPos);
    }

    void TryConnectNodes(TimeLineNode startNode, TimeLineNode endNode)
    {
        if (!CanConnectInCurrentTime(startNode, endNode))
        {
            return;
        }

        GameObject lineObj = new GameObject("Connection");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, startNode.transform.position);
        line.SetPosition(1, endNode.transform.position);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        TimeNodeConnection connection = new TimeNodeConnection
        {
            startNodeId = startNode.nodeId,
            endNodeId = endNode.nodeId,
            timeState = watchTool.currentTime,
            lineObject = lineObj
        };

        playerConnections.Add(connection);

        if (connectSound != null)
        {
            AudioSource.PlayClipAtPoint(connectSound, transform.position);
        }

        startNode.SetConnected(true);
        endNode.SetConnected(true);

        CheckPuzzleComplete();
    }

    bool CanConnectInCurrentTime(TimeLineNode start, TimeLineNode end)
    {
        if (watchTool == null) return false;

        return (start.nodeTime == watchTool.currentTime &&
                end.nodeTime == watchTool.currentTime);
    }

    void CheckPuzzleComplete()
    {
        if (playerConnections.Count != correctConnections.Count)
            return;

        bool allCorrect = true;
        for (int i = 0; i < correctConnections.Count; i++)
        {
            if (i >= playerConnections.Count)
            {
                allCorrect = false;
                break;
            }

            if (!correctConnections[i].Equals(playerConnections[i]))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            CompletePuzzle();
        }
    }

    void CompletePuzzle()
    {
        puzzleSolved = true;

        foreach (var conn in playerConnections)
        {
            if (conn.lineObject != null)
            {
                LineRenderer line = conn.lineObject.GetComponent<LineRenderer>();
                line.material = correctLineMaterial;
            }
        }

        if (successEffect != null)
        {
            Instantiate(successEffect, transform.position, Quaternion.identity);
        }

        if (successSound != null)
        {
            AudioSource.PlayClipAtPoint(successSound, transform.position);
        }

        isDone = true;
    }

    public override void EmptyClicked()
    {

    }


    public void ResetPuzzle()
    {
        foreach (var conn in playerConnections)
        {
            if (conn.lineObject != null)
                Destroy(conn.lineObject);
        }
        playerConnections.Clear();

        foreach (var node in pastNodes) node.ResetNode();
        foreach (var node in presentNodes) node.ResetNode();
        foreach (var node in futureNodes) node.ResetNode();

        puzzleSolved = false;
    }
}

[System.Serializable]
public class TimeNodeConnection
{
    public int startNodeId;
    public int endNodeId;
    public TimeState timeState;
    public GameObject lineObject;

    public bool Equals(TimeNodeConnection other)
    {
        return startNodeId == other.startNodeId &&
               endNodeId == other.endNodeId &&
               timeState == other.timeState;
    }
}