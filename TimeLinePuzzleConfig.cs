using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Time Puzzle", menuName = "Puzzle/Time Line Puzzle")]
public class TimeLinePuzzleConfig : ScriptableObject
{
    public string puzzleName;
    public Sprite puzzleIcon;

    [Header("Time Line position")]
    public List<Vector3> pastNodePositions;
    public List<Vector3> presentNodePositions;
    public List<Vector3> futureNodePositions;

    [Header("correct connection")]
    public List<TimeNodeConnectionData> correctConnections;

    [Header("hint message")]
    public string hintInPast;
    public string hintInPresent;
    public string hintInFuture;
}

[System.Serializable]
public class TimeNodeConnectionData
{
    public int startNodeId;
    public int endNodeId;
    public TimeState connectionTime;
}