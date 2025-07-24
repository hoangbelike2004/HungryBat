using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] Button btnSetting, btnOpenAndCloseTutorial;
    [SerializeField] TextMeshProUGUI txtMoverNumber, txtScore;
    [SerializeField] RectTransform parent, Recttutorial, iconbtnOpenTutorial, RectIconHand, IconHand, iconhand;
    [SerializeField] Slider sliderbar;
    [SerializeField] List<BonusUI> bonusUIs;
    [SerializeField] GameSupportBonus gameSupportBonus;
    [SerializeField] Canvas  canvasHand;
    private List<UIGoal> listgoals;
    private List<GameObject> Pluses;
    private int sumItemAmout;
    private LevelData mLevelData;
    private bool isTutorial = false, isOpenTutorial = false, isClickbtn = false;
    private void Start()
    {
        btnSetting.onClick.AddListener(() =>
        {
            CanvasSetting setting = UIManager.Instance.OpenUI<CanvasSetting>();
            setting.Active();
            SoundManager.Instance.PlaySound(eAudioType.SETTING_CLIP);
        });
        btnOpenAndCloseTutorial.onClick.AddListener(() =>
        {
            if (isClickbtn) return;
            if (GameController.Instance.GetIndexTutotial() == 3)
            {
                GameController.Instance.SetIndexTutotial();
            }
            isOpenTutorial = !isOpenTutorial;
            GameController.Instance.SetIsTutorial(isOpenTutorial);
            isClickbtn = true;
            if (isOpenTutorial)
            {
                Recttutorial.gameObject.SetActive(true);
                iconbtnOpenTutorial.DORotate(new Vector3(0, 0, -180), 0.2f);
                Recttutorial.DOAnchorPosX(0, 0.3f).OnComplete(() =>
                {
                    RectIconHand.DOAnchorPosY(-750, 0.5f).OnStepComplete(() =>
                    {
                        IconHand.DORotate(new Vector3(0, 180, 10), 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
                        {
                            RectIconHand.DOAnchorPosY(0, 0.5f).OnComplete(() =>
                            {
                                IconHand.DORotate(new Vector3(0, 180, 10), 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(() => isClickbtn = false);
                            });
                        });
                    });
                });
            }
            else
            {
                Recttutorial.DOAnchorPosX(2500, 0.3f).OnComplete(() =>
                {
                    Recttutorial.gameObject.SetActive(false);
                    DOTween.Restart(RectIconHand);
                    DOTween.Restart(IconHand);
                    isClickbtn = false;
                });
                iconbtnOpenTutorial.DORotate(new Vector3(0, 0, -0.1f), 0.2f);
            }
        });
    }

    public void SetLevelData(LevelData levelData)
    {
        mLevelData = levelData;
        listgoals = new List<UIGoal>();
        Pluses = new List<GameObject>();
        for (int i = 0; i < mLevelData.normalItemtype.Length; i++)
        {
            sumItemAmout += mLevelData.itemAmount[i];
            UIGoal prefab = Resources.Load<UIGoal>(GetPrefab(mLevelData.normalItemtype[i]));
            prefab = Instantiate(prefab, parent);
            prefab.UpdateGoal(mLevelData.itemAmount[i]);
            listgoals.Add(prefab);
            if (i < mLevelData.normalItemtype.Length - 1)
            {
                GameObject plusPrefab = Resources.Load<GameObject>(Constants.PREFAB_PLUS_GOAL);
                plusPrefab = Instantiate(plusPrefab, parent);
                Pluses.Add(plusPrefab);
            }
        }
        for (int i = 0; i < bonusUIs.Count; i++)
        {
            if (bonusUIs[i].gameObject.activeSelf)
            {
                bonusUIs[i].DeactiveGlow();
                bonusUIs[i].gameObject.SetActive(false);
            }
        }
        txtMoverNumber.text = mLevelData.moveNumber.ToString();
        sliderbar.value = 0;
        txtScore.text = "0";
    }
    public void ResetUI()
    {
        for (int i = 0; i < listgoals.Count; i++)
        {
            listgoals[i].UpdateGoal(mLevelData.itemAmount[i]);
        }
        txtMoverNumber.text = mLevelData.moveNumber.ToString();
        sliderbar.value = 0;
        txtScore.text = "0";
    }
    public void UpdateUI(int moveNumber, List<int> itemAmout, float completionrate, int score)
    {
        txtMoverNumber.text = moveNumber.ToString();
        int sumtmp = 0;
        for (int i = 0; i < itemAmout.Count; i++)
        {
            sumtmp += mLevelData.itemAmount[i];
            listgoals[i].UpdateGoal(itemAmout[i]);
        }
        sliderbar.DOValue(completionrate, 0.2f).SetEase(Ease.InOutCubic);
        txtScore.text = score.ToString();
    }
    public void UpdateUISupportBonus()//chay moi khi người choi su dung item
    {
        for (int i = 0; i < gameSupportBonus.bonusDatas.Count; i++)
        {
            if (gameSupportBonus.bonusDatas[i].state == eStateBonusItem.SELECTED)
            {
                bonusUIs[i].gameObject.SetActive(true);
                bonusUIs[i].SetBonusData(gameSupportBonus.bonusDatas[i]);
                bonusUIs[i].UpdateAmout();
            }
        }
    }
    public string GetPrefab(NormalItem.eNormalType type)
    {
        string name = string.Empty;
        switch (type)
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
        for (int i = 0; i < listgoals.Count; i++)
        {
            GameObject.Destroy(listgoals[i].gameObject);
        }
        for (int i = 0; i < Pluses.Count; i++)
        {
            GameObject.Destroy(Pluses[i].gameObject);
        }
    }
    public void HandTutorial(bool isTutorial)
    {
        if (isTutorial)
        {
            iconhand.gameObject.SetActive(true);
            iconhand.DORotate(new Vector3(0, 0, 10), 0.15f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            iconhand.DOKill();
            iconhand.gameObject.SetActive(false);
        }
    }
}
