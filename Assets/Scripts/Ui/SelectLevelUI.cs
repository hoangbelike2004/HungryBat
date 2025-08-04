using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelUI : UICanvas
{
    [SerializeField] Button btnPlay;
    [SerializeField] Button btnClose;
    [SerializeField] TextMeshProUGUI txtTitleLevel, txtBomb, txtPotion, txtLightning;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform box;
    [SerializeField] Vector2[] pos;
    [SerializeField] RectTransform[] UiCollects;
    [SerializeField] GameSupportBonus gameSupport;
    [SerializeField] Transform parent;
    [SerializeField] List<BonusUI> bonUIs;
    [SerializeField] GameSetting gamesetting;
    [SerializeField] Canvas canvasBonusItems;
    [SerializeField] Canvas canvasBtnPlay, canvasHand, canvasHand2;
    [SerializeField] RectTransform iconhand, iconhand2;
    private LevelData LevelData;
    private void Start()
    {
        btnClose.onClick.AddListener(DeActive);
        btnPlay.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
            PlayGame();
        });
        Observer.SelectBonusTutorialEvent += StartTutorial;
    }
    public void SetLevelData(LevelData levelData)
    {
        this.LevelData = levelData;
        UpdateUi();
    }
    public void UpdateUi()
    {
        txtTitleLevel.text = "Level " + LevelData.level.ToString();
        SetItemSupport();
        ActiveCollect();
    }
    public void Active()
    {
        SoundManager.Instance.PlaySound(eAudioType.POPUP_ACTIVE_CLIP);
        overlay.gameObject.SetActive(true);
        overlay.DOFade(0.5f, 0.1f);
        box.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
        StartTutorial();
    }
    public void PlayGame()
    {
        box.DOAnchorPos(new Vector2(0, 1700), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                GameController.Instance.SetLevelData(LevelData);
                GameController.Instance.SetState(eStateGame.STARTED);
                GameController.Instance.ChangeState();
                if (GameController.Instance.GetIndexTutotial() == 4)
                {
                    canvasBtnPlay.sortingOrder = 2;
                    HandTutorial(false, true);
                    GameController.Instance.SetIndexTutotial();
                    GameController.Instance.SetCanvasTutorial("Board");
                    //GameController.Instance.ActiveTutorialGameplay();
                }
                UIManager.Instance.CloseUI<CanvasMain>(0f);
                UIManager.Instance.CloseUI<SelectLevelUI>(0f);
            });

        });
    }
    public void DeActive()
    {
        SoundManager.Instance.PlaySound(eAudioType.CLOSE_CLIP);
        SoundManager.Instance.PlaySound(eAudioType.POPUP_DEACTIVE_CLIP);
        box.DOAnchorPos(new Vector2(0, 1700), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                LevelData = null;
                UIManager.Instance.CloseUI<SelectLevelUI>(0f);
            });

        });
    }
    public void ActiveCollect()
    {
        List<RectTransform> uiActive = new List<RectTransform>();
        for (int i = 0; i < UiCollects.Length; i++)
        {
            UiCollects[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < LevelData.normalItemtype.Length; i++)
        {
            uiActive.Add(UiCollects[(int)LevelData.normalItemtype[i]]);
        }
        for (int i = 0; i < uiActive.Count; i++)
        {
            uiActive[i].gameObject.SetActive(true);
            uiActive[i].anchoredPosition = pos[i];

        }
    }
    public void SetItemSupport()
    {
        for (int i = 0; i < gameSupport.bonusDatas.Count; i++)
        {
            bonUIs[i].SetBonusData(gameSupport.bonusDatas[i]);
            bonUIs[i].UpdateUI();
        }
    }
    public void StartTutorial()
    {
        if (GameController.Instance.GetIndexTutotial() == 5)
        {
            canvasBonusItems.sortingOrder = 4;
            HandTutorial(true,false);
        }
        else if(GameController.Instance.GetIndexTutotial() == 4)
        {
            canvasBonusItems.sortingOrder = 2;
            canvasBtnPlay.sortingOrder = 4;
            HandTutorial(true, true);
            Observer.SelectBonusTutorialEvent -= StartTutorial;
        }
    }
    public void HandTutorial(bool isTutorial,bool isPlay)
    {
        if (!isPlay)
        {
            if (isTutorial)
            {
                canvasHand.gameObject.SetActive(true);
                iconhand.DORotate(new Vector3(0, 180, 10), 0.15f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                canvasHand.gameObject.SetActive(false);
                iconhand.DOKill();
            }
        }
        else
        {
            if (isTutorial)
            {
                canvasHand2.gameObject.SetActive(true);
                iconhand2.DORotate(new Vector3(0, 180, 10), 0.15f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                canvasHand2.gameObject.SetActive(false);
                iconhand2.DOKill();
            }
        }
    }
}
