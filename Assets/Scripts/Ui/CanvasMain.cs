using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum eStateMain
{
    HOME,
    EVENT,
    SHOP,
}
public class CanvasMain : UICanvas
{
    [SerializeField] Transform content;
    [SerializeField] TextMeshProUGUI txtCoin;
    [SerializeField] Button btnSetting, btnhome, btnEvent, btnShop;
    [SerializeField] GameObject[] listObject;
    [SerializeField] RectTransform[] iconRects;
    [SerializeField] int offsetYIcon;
    GameSupportBonus gameSupportBonus;
    LevelController m_levelctr;


    [Header("SHOP")]
    public Transform parentItemShop;
    [SerializeField] Image overlayNotification,parentNotification;
    [SerializeField] RectTransform NotificationFailRect, NotificationSuccessRect;
    private List<BuyBonusItem> buyBonusItems = new List<BuyBonusItem>();

    private eStateMain cunrentState;
    private void Awake()
    {
        m_levelctr = new GameObject("LevelController").AddComponent<LevelController>();
    }
    private void Start()
    {
        btnSetting.onClick.AddListener(() =>
        {
            CanvasSetting setting = UIManager.Instance.OpenUI<CanvasSetting>();
            setting.Active();
        });
        btnhome.onClick.AddListener(() =>
        {
            SetState(eStateMain.HOME);
        });
        btnEvent.onClick.AddListener(() =>
        {
            SetState(eStateMain.EVENT);
        });
        btnShop.onClick.AddListener(() =>
        {
            SetState(eStateMain.SHOP);
        });
        ChangeState();
    }
    public void SetState(eStateMain state)
    {
        if (cunrentState != state)
        {
            cunrentState = state;
            ChangeState();
        }
    }
    public void ChangeState()
    {
        listObject[(int)cunrentState].SetActive(true);
        iconRects[(int)cunrentState].DORotate(new Vector3(0, 0, -30), 0.3f);
        iconRects[(int)cunrentState].DOScale(1.4f, 0.3f);
        int target = (int)iconRects[(int)cunrentState].anchoredPosition.y + offsetYIcon;
        iconRects[(int)cunrentState].DOAnchorPosY(target, 0.3f);
        switch (cunrentState)
        {
            case eStateMain.HOME:
                ActiveHome();
                break;
            case eStateMain.EVENT:
                ActiveEvent();
                break;
            case eStateMain.SHOP:
                ActiveShop();
                break;
        }
        DeactiveOldState();
    }
    public void ActiveHome()
    {
        m_levelctr.Init(content);
    }
    public void ActiveEvent()
    {
    }

    public void ActiveShop()
    {
        if (buyBonusItems.Count > 0) return;
        for (int i = 0; i < gameSupportBonus.bonusDatas.Count; i++)
        {
            BuyBonusItem prefab = Resources.Load<BuyBonusItem>(Constants.PREFAB_ITEM_SHOP);
            prefab = Instantiate(prefab,parentItemShop);
            prefab.SetData(gameSupportBonus.bonusDatas[i]);
            prefab.SetCanvasMain(this);
            buyBonusItems.Add(prefab);
        }
    }
    public void DeactiveOldState()
    {
        for (int i = 0; i < listObject.Length; i++)
        {
            if (i != (int)cunrentState)
            {
                listObject[i].SetActive(false);
                iconRects[i].DORotate(new Vector3(0, 0, 0), 0.3f);
                iconRects[i].DOScale(1f, 0.3f);
                iconRects[i].DOAnchorPosY(200f, 0.3f);
            }
        }
    }
    public void UpdateLevelUi()
    {
        m_levelctr.UpdateLevel();
    }
    public void SetGameSupportBonus(GameSupportBonus gameSupportBonus)
    {
        this.gameSupportBonus = gameSupportBonus;
    }
    public void ActiveNotification(bool isSucces)
    {
        parentNotification.gameObject.SetActive(true);
        overlayNotification.DOFade(0.5f, 0.2f);
        if(!isSucces)
        {
            NotificationFailRect.DOAnchorPosX(0, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                NotificationFailRect.DOAnchorPosX(1500, 0.4f).SetEase(Ease.InQuint).SetDelay(1f).OnComplete(() =>
                {
                    NotificationFailRect.DOAnchorPosX(-1500, 0);
                    overlayNotification.DOFade(0f, 0f);
                    parentNotification.gameObject.SetActive(false);
                });
            });
        }
        else
        {
            NotificationSuccessRect.DOScale(1, 0.2f).SetEase(Ease.InElastic).OnComplete(() =>
            {
                NotificationSuccessRect.DOScale(0, 0.2f).SetEase(Ease.OutElastic).SetDelay(1f).OnComplete(() =>
                {
                    overlayNotification.DOFade(0f, 0f);
                    parentNotification.gameObject.SetActive(false);
                });
            });
        }
    }
    public void UpdateCoin(int coin)
    {
        txtCoin.text = coin.ToString();
    }
}
