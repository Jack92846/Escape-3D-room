using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemName
{
    Key,
    Note,
    Tool,
    Watch,
    TimeWatch,
    Battery,
    Bed,
    Lamp,
    Mirror,
    Painting,
    Table,
    Bird,
    Equipment,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public ItemName itemName;
    public Sprite icon;
    public string description;
    public GameObject prefab;
}
