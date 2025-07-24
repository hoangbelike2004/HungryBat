using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private GameLevel GameLevel;
    List<LevelItem> levelitems;
    List<Transform> levelitemsparent;
    public void CreateLevel(GameLevel gamelevel,List<Transform> ParentItems)
    {
        levelitems = new List<LevelItem>();
        levelitemsparent = new List<Transform>();
        this.GameLevel = gamelevel;
        levelitemsparent = ParentItems;
        CreateLevel();
        UpdateLevel();
    }
    //private void Start()
    //{
    //    GameLevel = Resources.Load<GameLevel>(Constants.GAME_LEVEL_PATH);
    //    CreateLevel();
    //}
    public void CreateLevel()
    {
        GameObject prefab = Resources.Load<GameObject>(Constants.LEVEL_ITEM_PATH);
        for (int i = 0; i < GameLevel.levels.Count; i++)
        {
            GameObject levelitem = Instantiate(prefab, levelitemsparent[i]);
            LevelItem level = levelitem.GetComponent<LevelItem>();
            levelitems.Add(level);
            level.SetData(GameLevel.levels[i]);
        }
    }

    public void UpdateUI()//khi ngươi dùng chiến thắng thì sẽ updatUI
    {
        for (int i = 0; i < levelitems.Count; i++)
        {
            levelitems[i].SetUI();
        }
    }
    public void UpdateLevel()
    {
        for (int i = 0; i < GameLevel.levels.Count; i++)
        {
            if (GameLevel.levels[i].levelType == eStateLevel.LOCK)
            {
                if(GameLevel.levels[i-1].levelType == eStateLevel.COMPLETE)
                {
                    GameLevel.levels[i].levelType = eStateLevel.OPEN;
                    break;
                }
            }
        }
        UpdateUI();
    }
}
