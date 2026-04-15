using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public Item itemData;
    public float pickupDistance = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Score setting")]
    public bool giveScore = true;
    public int customScore = -1;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryCollectItem();
        }
    }

    void OnMouseDown()
    {
        TryCollectItem();
    }

    public void TryCollectItem()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance <= pickupDistance)
        {
            CollectItem();
        }
        else
        {

        }
    }

    void CollectItem()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null && itemData != null)
        {
            inventory.AddItem(itemData);

            if (giveScore && ScoreManager.Instance != null)
            {
                int scoreToAdd = customScore >= 0 ? customScore : ScoreManager.Instance.GetItemScore(itemData.itemName);
                ScoreManager.Instance.AddScore(scoreToAdd);
            }

            if (itemData.itemName == ItemName.TimeWatch)
            {
                WatchTool watchTool = FindObjectOfType<WatchTool>();
                if (watchTool != null)
                {
                    watchTool.AcquireWatch();
                }
            }

        if (itemData.itemName == ItemName.Battery)
        {
            WatchTool watchTool = FindObjectOfType<WatchTool>();
            if (watchTool != null && watchTool.hasWatch)
            {
                watchTool.UpgradeWatchWithBattery();
            }
            else
            {

            }
        }

            Destroy(gameObject);
        }
    }

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
