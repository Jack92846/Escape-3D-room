using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePuzzle : Interactive
{
    public GameObject pastSolution;
    public GameObject presentSolution;
    public ItemName requiredTimeItem;

    protected override void OnClickedAction()
    {
        WatchTool watch = FindObjectOfType<WatchTool>();
        if (watch != null)
        {
            bool isPast = false;

            if (isPast)
            {
                SolveInPast();
            }
            else
            {
                SolveInPresent();
            }
        }
    }

    void SolveInPast()
    {
        if (pastSolution != null)
        {
            pastSolution.SetActive(true);
        }
    }

    void SolveInPresent()
    {
        if (presentSolution != null)
        {
            presentSolution.SetActive(true);
        }
    }

    public override void EmptyClicked()
    {
        
    }
}
