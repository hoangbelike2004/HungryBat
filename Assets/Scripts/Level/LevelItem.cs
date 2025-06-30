using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] GameObject[] stars;
    [SerializeField] GameObject glow;
    [SerializeField] GameObject lockup;
    [SerializeField] TextMeshProUGUI txtLevel;
    [SerializeField] Button btn;
    private LevelData levelData;
    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            if(GameController.Instance.GetHeart() > 0)
            {
                SelectLevelUI sel = UIManager.Instance.OpenUI<SelectLevelUI>();
                sel.SetLevelData(levelData);
                sel.Active();
            }
            else
            {
                CanvasOutofLives cv = UIManager.Instance.OpenUI<CanvasOutofLives>();
                cv.Active();
                GameController.Instance.SetCanvasOutOfLives(cv);
            }
        });
    }
    public void SetData(LevelData levelData)
    {
        this.levelData = levelData;
        SetUI();
    }
    public void SetRoot(Transform parent)
    {
        this.transform.SetParent(parent, true);
    }

    public void SetUI()
    {
        if (levelData == null) return;
        txtLevel.text = levelData.level.ToString();
        if(levelData.levelType == eStateLevel.LOCK)
        {
            lockup.SetActive(true); 
        }
        else if (levelData.levelType == eStateLevel.OPEN)
        {
            lockup.SetActive(false);
            glow.SetActive(true);
        }
        else if(levelData.levelType == eStateLevel.COMPLETE)
        {
            lockup.SetActive(false);
            glow.SetActive(false);
        }
        for (int i = 0; i < levelData.starNumber; i++)
        {
            stars[i].SetActive(true);
        }
    }
}
