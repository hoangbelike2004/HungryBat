using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtAmoutItem;
    [SerializeField] Button btnSelect;
    [SerializeField] Image SelectionIndicator;
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
                }
            }
        });
    }
    public void SetBonusData(BonusData bonusdata)
    {
        this.bonusdata = bonusdata;
    }
    public void UpdateUI()
    {
        txtAmoutItem.text = bonusdata.amout.ToString();
        if(bonusdata.state == eStateBonusItem.UNSELECTED)
        {
            SelectionIndicator.gameObject.SetActive(false);
        }
        else
        {
            if(bonusdata.amout == 0)
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
    public void UpdateAmout()
    {
        txtAmoutItem.text = bonusdata.amout.ToString();
    }
}
