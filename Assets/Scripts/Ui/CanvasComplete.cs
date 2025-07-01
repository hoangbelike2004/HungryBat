using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CanvasComplete : UICanvas
{
    [SerializeField] Button btnHome, btnReload;
    [SerializeField] TextMeshProUGUI txtScore, txtCoin;
    [SerializeField] RectTransform box, glow;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform[] visualstars;
    [SerializeField] LevelData levelData;
    private void Start()
    {
        btnHome.onClick.AddListener(OnDeactive);
        btnReload.onClick.AddListener(ReLoadGame);
    }

    public void OnActive(int score, int star, int coin)
    {
        txtCoin.text = "0";
        txtScore.text = "0";
        overlay.DOFade(0.5f, 0.1f);
        glow.DOScale(1f, 0.3f);
        glow.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        if (star == 3)
        {
            SoundManager.Instance.PlaySound(eAudioType.COMPLETE_WIN);
        }
        else
        {
            SoundManager.Instance.PlaySound(eAudioType.COMPLETE_NOT_WIN);
        }
        box.DOScale(1f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            int startvalue = 0;
            int startvalue2 = 0;
            DOTween.To(() => startvalue2, x =>
            {
                startvalue = x;
                txtScore.text = x.ToString();
            }, score, 1f).SetEase(Ease.Linear);
            DOTween.To(() => startvalue, x =>
            {
                startvalue = x;
                txtCoin.text = x.ToString();
            }, coin, 1f).SetEase(Ease.Linear);
            Sequence sq = DOTween.Sequence();
            for (int i = 0; i < star; i++)
            {
                visualstars[i].gameObject.SetActive(true);
                sq.InsertCallback(sq.Duration() + 0.25f - 0.05f, () =>
                {
                    SoundManager.Instance.PlaySound(eAudioType.STAR);
                });
                sq.Append(visualstars[i].DOScale(1f, 0.4f).SetDelay(0.25f).SetEase(Ease.OutElastic));
            }
        });
    }

    public void OnDeactive()
    {
        SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
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
        SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
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
