using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eStateLevel
{
    LOCK,
    OPEN,
    COMPLETE,
}
[System.Serializable]
public class LevelData
{
    public int level;
    public eStateLevel levelType;
    public int moveNumber;
    public int starNumber;
    public NormalItem.eNormalType[] normalItem;
    public int[] itemAmount;
    public int coin;
}
