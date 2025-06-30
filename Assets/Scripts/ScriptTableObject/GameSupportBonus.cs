using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SupportBonus")]
public class GameSupportBonus : ScriptableObject
{
    public List<BonusData> bonusDatas;
    public void LoadDataItem(List<int> amouts)
    {
        for (int i = 0; i < bonusDatas.Count; i++)
        {
            bonusDatas[i].amout = amouts[i];
        }
    }
}
