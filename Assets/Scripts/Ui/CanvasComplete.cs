using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CanvasComplete : UICanvas
{
    [SerializeField] Button btnHome;
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] RectTransform box,glow;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform[] visualstars;
    private void Start()
    {
        btnHome.onClick.AddListener(OnDeactive);
    }

    public void OnActive(int score,int star)
    {
        overlay.DOFade(0.5f, 0.1f);
        glow.DOScale(1f, 0.3f);
        glow.DORotate(new Vector3(0, 0, 360), 1f).SetLoops(-1);
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
            UIManager.Instance.CloseAll();
            GameController.Instance.ClearLevel();
            GameController.Instance.SetState(eStateGame.MAIN_MENU);
            GameController.Instance.ChangeState();
            ResetUI();
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
}
