using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CanvasComplete : UICanvas
{
    [SerializeField] Button btnHome,btnReload;
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] RectTransform box,glow;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform[] visualstars;
    [SerializeField] LevelData levelData;
    private void Start()
    {
        btnHome.onClick.AddListener(OnDeactive);
        btnReload.onClick.AddListener(ReLoadGame);
    }

    public void OnActive(int score,int star)
    {
        overlay.DOFade(0.5f, 0.1f);
        glow.DOScale(1f, 0.3f);
        glow.DOScale(1.1f, 1f).SetLoops(-1,LoopType.Yoyo);
        box.DOScale(1f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            txtScore.text = score.ToString();
            Sequence sq = DOTween.Sequence();
            for (int i = 0; i < star; i++)
            {
                visualstars[i].gameObject.SetActive(true);
                sq.Append(visualstars[i].DOScale(1f, 0.4f).SetEase(Ease.OutElastic));
            }
        });
    }

    public void OnDeactive()
    {
        overlay.DOFade(0.01f, 0.2f);
        glow.DOScale(0.1f, 0.2f);
        box.DOScale(0.1f, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < visualstars.Length; i++)
            {
                seq.Append(visualstars[i].DOScale(0.01f, 0f).OnComplete(() =>
                {
                    visualstars[i].gameObject.SetActive(false);
                }));
            }
            seq.OnComplete(() =>
            {
                ResetUI();
                UIManager.Instance.CloseAll();
                GameController.Instance.ClearLevel();
                GameController.Instance.SetState(eStateGame.MAIN_MENU);
                GameController.Instance.ChangeState();
            });
            
        });
    }
    public void ReLoadGame()
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < visualstars.Length; i++)
        {
            seq.Append(visualstars[i].DOScale(0.01f, 0f).OnComplete(() =>
            {
                visualstars[i].gameObject.SetActive(false);
            }));
        }
        seq.OnComplete(() =>
        {
            ResetUI();
            GameController.Instance.ClearLevel();
            GameController.Instance.SetLevelData(levelData);
            GameController.Instance.SetState(eStateGame.STARTED);
            GameController.Instance.ChangeState();
            UIManager.Instance.CloseUI<CanvasComplete>(0f);
        });
    }
    public void ResetUI()
    {
        for (int i = 0; i < visualstars.Length; i++)
        {
            visualstars[i].DOScale(0.01f, 0f).OnComplete(() =>
            {
                visualstars[i].gameObject.SetActive(false);
            });
        }
    }
    public void SetLevelData(LevelData level)
    {
        this.levelData = level;
    }
}
