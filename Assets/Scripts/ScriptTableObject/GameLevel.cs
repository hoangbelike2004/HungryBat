using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public void Check()
    {
        for (int i = 0; i < levels.Count; i++)
        {

            if (levels[i].level != (i + 1))
            {
                Debug.LogError("Errol level: " + (i + 1));
            }
            if (i > 0 && (levels[i - 1].levelType == eStateLevel.OPEN || levels[i - 1].levelType == eStateLevel.LOCK) &&
                (levels[i].levelType == eStateLevel.COMPLETE || levels[i].levelType == eStateLevel.OPEN))
            {
                Debug.LogError("Errol type level: " + (i + 1));
            }
            if (levels[i].normalItemtype.Length != levels[i].itemAmount.Length)
            {
                Debug.LogError("Loi so luong khong hop le: " + (i + 1));
            }
            if (levels[i].normalItemtype.Length == 0)
            {
                Debug.LogError("Chua nhap item can thu gon");
            }
            if (levels[i].itemAmount.Length == 0)
            {
                Debug.LogError("Chua nhap so luong item can thu gon");
            }
            if (levels[i].coin < 0 &&
                (levels[i].levelType == eStateLevel.LOCK || levels[i].levelType == eStateLevel.LOCK))
            {
                Debug.LogError("Phan thuong khong hop le!");
            }
            if (levels[i].moveNumber <= 0)
            {
                Debug.LogError("So luot di chuyen > 0");
            }
        }
    }
}
