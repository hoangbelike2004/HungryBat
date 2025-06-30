using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOutofLives :UICanvas
{
    [SerializeField] Button btnClose;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform box;
    [SerializeField] TextMeshProUGUI txtHeart, txtTimeHeart;

    private void Start()
    {
        btnClose.onClick.AddListener(Deactive);   
    }

    public void Active()
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(0.5f, 0.1f);
        box.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
    }

    public void Deactive()
    {
        SoundManager.Instance.PlaySound(eAudioType.CLOSE_CLIP);
        box.DOAnchorPos(new Vector2(0, 1700), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                GameController.Instance.SetCanvasOutOfLives(null);
                UIManager.Instance.CloseUI<CanvasOutofLives>(0f);
            });

        });
    }
    public void UpdateUI(int heart,string strTime)
    {
        txtHeart.text = heart.ToString();
        txtTimeHeart.text = strTime;
    }
}
