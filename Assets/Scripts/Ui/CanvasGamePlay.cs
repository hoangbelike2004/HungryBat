using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] Button btnSetting;
    [SerializeField] TextMeshProUGUI txtMoverNumber,txtScore;
    [SerializeField] RectTransform parent;
    [SerializeField] Slider sliderbar;
    private List<UIGoal> listgoals;
    private int sumItemAmout;
    private LevelData mLevelData;
    private void Start()
    {
        btnSetting.onClick.AddListener(() =>
        {
            CanvasSetting setting = UIManager.Instance.OpenUI<CanvasSetting>();
            setting.Active();
        });
    }

    public void SetLevelData(LevelData levelData)
    {
        mLevelData = levelData;
        listgoals = new List<UIGoal>();
        for (int i = 0; i < mLevelData.normalItem.Length; i++)
        {
            sumItemAmout += mLevelData.itemAmount[i];
            UIGoal prefab = Resources.Load<UIGoal>(GetPrefab(mLevelData.normalItem[i]));
            prefab = Instantiate(prefab);
            prefab.transform.SetParent(parent, false);
            prefab.UpdateGoal(levelData.itemAmount[i]);
            listgoals.Add(prefab);
            if (i  < mLevelData.normalItem.Length - 1)
            {
                GameObject plusPrefab = Resources.Load<GameObject>(Constants.PREFAB_PLUS_GOAL);
                plusPrefab = Instantiate(plusPrefab);
                plusPrefab.transform.SetParent(parent, false);
            }
        }
        txtMoverNumber.text = mLevelData.moveNumber.ToString();
        sliderbar.value = 0;
        txtScore.text = "0";
    }

    public void UpdateUI(int moveNumber, List<int> itemAmout,float completionrate)
    {
        txtMoverNumber.text = moveNumber.ToString();
        int sumtmp = 0;
        for(int i = 0;i < itemAmout.Count; i++)
        {
            sumtmp += mLevelData.itemAmount[i];
            listgoals[i].UpdateGoal(itemAmout[i]);
        }
        sliderbar.value = completionrate;

    }
    public string GetPrefab(NormalItem.eNormalType type)
    {
        string name = string.Empty;
        switch(type)
        {
            case NormalItem.eNormalType.TYPE_ONE:
                name = Constants.PREFAB_NORMAL_TYPE_ONE_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_TWO:
                name = Constants.PREFAB_NORMAL_TYPE_TWO_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_THREE:
                name = Constants.PREFAB_NORMAL_TYPE_THREE_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_FOUR:
                name = Constants.PREFAB_NORMAL_TYPE_FOUR_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_FIVE:
                name = Constants.PREFAB_NORMAL_TYPE_FIVE_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_SIX:
                name = Constants.PREFAB_NORMAL_TYPE_SIX_GOAL;
                break;
            case NormalItem.eNormalType.TYPE_SEVEN:
                name = Constants.PREFAB_NORMAL_TYPE_SEVEN_GOAL;
                break;
        }
        return name;
    }
    public void ClearGoal()
    {
        for(int i = 0; i < listgoals.Count; i++)
        {
            GameObject.Destroy(listgoals[i].gameObject);
        }
    }
}
