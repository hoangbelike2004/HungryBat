using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel")]
public class GameLevel : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
    public void LoadDataLevel(LevelData leveldata)
    {
        for (int i = 0; i < leveldata.level; i++)
        {
            levels[i].levelType = eStateLevel.COMPLETE;
            levels[i].starNumber = 3;
            levels[i].coin = 0;
            if (i == leveldata.level - 1)
            {
                levels[i].levelType = leveldata.levelType;
                levels[i].starNumber = leveldata.starNumber;
                levels[i].coin = leveldata.coin;
            }
        }
    }
}
