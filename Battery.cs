using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battery", menuName = "Inventory/Battery")]
public class Battery : Item
{
    public float powerLevel = 100f;
}