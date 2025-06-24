using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSetting : UICanvas
{
    [SerializeField] Button btnClose;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform box;
    [SerializeField] Button btnQuit;
    void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            DeActive();
        });
        btnQuit.onClick.AddListener(() =>
        {
            box.DOAnchorPos(new Vector2(0, 1700), 0f);
            overlay.DOFade(0.1f, 0f);
            overlay.gameObject.SetActive(false);
            UIManager.Instance.CloseAll();
            GameController.Instance.SetState(eStateGame.MAIN_MENU);
            GameController.Instance.ChangeState();
        });
    }

    public void Active()
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(0.5f, 0.1f);
        box.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
    }
    public void DeActive()
    {
        box.DOAnchorPos(new Vector2(0, 1700), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                UIManager.Instance.CloseUI<CanvasSetting>(0f);
            });

        });

    }
}
