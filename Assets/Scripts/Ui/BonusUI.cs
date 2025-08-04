using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtAmoutItem;
    [SerializeField] Button btnSelect;
    [SerializeField] Image SelectionIndicator;
    [SerializeField] Image glow;
    private bool isSelected = false;
    private BonusData bonusdata;

    private void Start()
    {
        btnSelect.onClick.AddListener(() =>
        {
            if (bonusdata.amout > 0)
            {
                if (GameController.Instance.StateGame == eStateGame.STARTED)
                {
                    SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
                    GameController.Instance.SetBonusData(bonusdata);
                    glow.gameObject.SetActive(true);
                    if (GameController.Instance.GetIndexTutotial() == 3)
                    {
                        GameController.Instance.SetIndexTutotial();
                    }
                }
                else
                {
                    isSelected = !isSelected;
                    if (isSelected)
                    {
                        SoundManager.Instance.PlaySound(eAudioType.SELECT_BOUNUS_ITEM);
                        SelectionIndicator.gameObject.SetActive(true);
                        bonusdata.state = eStateBonusItem.SELECTED;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(eAudioType.NOT_SELECT_BONUS_ITEM);
                        SelectionIndicator.gameObject.SetActive(false);
                        bonusdata.state = eStateBonusItem.UNSELECTED;
                    }
                    if (GameController.Instance.GetIndexTutotial() == 5)
                    {
                        GameController.Instance.SetIndexTutotial();
                        Observer.SelectBonusTutorialEvent.Invoke();
                    }
                }
            }
        });
    }
    public void SetBonusData(BonusData bonusdata)
    {
        this.bonusdata = bonusdata;
        isSelected = bonusdata.state == eStateBonusItem.SELECTED ? true : false;
    }
    public void UpdateUI()
    {
        txtAmoutItem.text = bonusdata.amout.ToString();
        if (bonusdata.state == eStateBonusItem.UNSELECTED)
        {
            SelectionIndicator.gameObject.SetActive(false);
        }
        else
        {
            if (bonusdata.amout == 0)
            {
                bonusdata.state = eStateBonusItem.UNSELECTED;
                SelectionIndicator.gameObject.SetActive(false);
            }
            else
            {
                SelectionIndicator.gameObject.SetActive(true);
            }
        }
    }
    public void DeactiveGlow()
    {
        glow.gameObject.SetActive(false);
    }
    public void UpdateAmout()
    {
        txtAmoutItem.text = bonusdata.amout.ToString();
        DeactiveGlow();
    }
}
