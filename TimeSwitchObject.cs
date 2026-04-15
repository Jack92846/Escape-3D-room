using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSwitchObject : MonoBehaviour
{
    public GameObject pastVersion;
    public GameObject presentVersion;

    void Start()
    {
        SwitchToPresent();
    }

    public void SwitchToPast()
    {
        if (pastVersion != null) pastVersion.SetActive(true);
        if (presentVersion != null) presentVersion.SetActive(false);
    }

    public void SwitchToPresent()
    {
        if (pastVersion != null) pastVersion.SetActive(false);
        if (presentVersion != null) presentVersion.SetActive(true);
    }
}
