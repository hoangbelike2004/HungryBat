using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eStateBonusItem
{
    SELECTED,
    UNSELECTED,
}
[System.Serializable]
public class BonusData
{
    public string name;
    public string description;
    public int price;
    public int amout;
    public eBonusType type;
    public eStateBonusItem state;
    public GameObject prefab;
}
