using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameLevel GameLevel;
    [SerializeField] Transform root;
    List<LevelItem> leveltimes;
    public void Init(Transform r)
    {
        root = r;
    }
    private void Start()
    {
        leveltimes = new List<LevelItem>();
        GameLevel = Resources.Load<GameLevel>(Constants.GAME_LEVEL_PATH);
        CreateLevel();
        UpdateLevel();
    }
    public void CreateLevel()
    {
        GameObject prefab = Resources.Load<GameObject>(Constants.LEVEL_ITEM_PATH);
        for (int i = 0; i < GameLevel.levels.Count; i++)
        {
            GameObject levelitem = Instantiate(prefab, root);
            LevelItem level = levelitem.GetComponent<LevelItem>();
            leveltimes.Add(level);
            level.SetData(GameLevel.levels[i]);
        }
    }

    public void UpdateUI()//khi ngươi dùng chiến thắng thì sẽ updatUI
    {
        for (int i = 0; i < leveltimes.Count; i++)
        {
            leveltimes[i].SetUI();
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
