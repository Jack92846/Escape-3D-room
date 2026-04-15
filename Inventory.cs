using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public float pickupRange = 5f;
    public LayerMask itemLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, itemLayer))
        {
            CollectableItem collectable = hit.collider.GetComponent<CollectableItem>();
            if (collectable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance <= pickupRange)
                {
                    AddItem(collectable.itemData);
                }
            }
        }
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
    }

    public bool HasItem(ItemName itemName)
    {
        return items.Exists(item => item.itemName == itemName);
    }

    public bool UseBattery()
    {
        Item battery = items.Find(item => item.itemName == ItemName.Battery);
        if (battery != null)
        {
            items.Remove(battery);
            if (ScoreManager.Instance != null)
            {
                 ScoreManager.Instance.AddScore(100);
            }
            return true;
        }
        return false;
    }

    public void RemoveItem(ItemName itemName)
    {
        Item itemToRemove = items.Find(item => item.itemName == itemName);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            Debug.Log("Removed item: " + itemName);
        }
    }

        public int GetItemCount(ItemName itemName)
        {
            return items.FindAll(item => item.itemName == itemName).Count;
        }

        public int GetTotalItemCount()
        {
            return items.Count;
        }
}