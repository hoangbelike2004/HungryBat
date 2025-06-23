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
    private LevelData LevelData;
    private void Start()
    {
        btnClose.onClick.AddListener(DeActive);
        btnPlay.onClick.AddListener(() =>
        {
            PlayGame();
        });  
    }
    public void SetLevelData(LevelData levelData)
    {
        this.LevelData = levelData;
        UpdateUi();
    }
    public void UpdateUi()
    {
        txtTitleLevel.text = "Level " + LevelData.level.ToString();
        ActiveCollect();
    }
    public void Active()
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(0.5f, 0.1f);
        box.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
    }
    public void PlayGame()
    {
        box.DOAnchorPos(new Vector2(0, 1700), 0f);
        overlay.DOFade(0.1f, 0f);
        overlay.gameObject.SetActive(false);
        GameController.Instance.SetLevelData(LevelData);
        UIManager.Instance.CloseUI<CanvasMain>(0f);
        UIManager.Instance.CloseUI<SelectLevelUI>(0f);
    }
    public void DeActive()
    {
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
        for (int i = 0; i < LevelData.normalItem.Length; i++)
        {
            uiActive.Add(UiCollects[(int)LevelData.normalItem[i]]);
        }
        for (int i = 0; i < uiActive.Count; i++)
        {
            uiActive[i].gameObject.SetActive(true);
            uiActive[i].anchoredPosition = pos[i];

        }
    }
    public void GetItemSupport()
    {

    }
}
