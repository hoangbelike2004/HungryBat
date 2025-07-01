using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGetReward : UICanvas
{
    [SerializeField] GameObject[] rewards;
    [SerializeField] TextMeshProUGUI txtAmout;
    [SerializeField] RectTransform box;
    [SerializeField] Button btnClose;
    private eAwardType currenttype;
    private void Start()
    {
        btnClose.onClick.AddListener(Deactive);
    }

    public void Active(eAwardType type,int amout)
    {
        currenttype = type;
        rewards[(int)currenttype].SetActive(true);
        txtAmout.text = amout.ToString();
        box.DOScale(1, 0.3f);
    }
    public void Deactive()
    {
        rewards[(int)currenttype].SetActive(false);
        UIManager.Instance.CloseUI<CanvasGetReward>(0f);
    }
}
