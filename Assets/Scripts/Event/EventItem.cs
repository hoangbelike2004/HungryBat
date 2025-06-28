using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventItem : MonoBehaviour
{
    [SerializeField] Button btnFinshed;//button nhan thuong
    [SerializeField] Image unfinished;
    [SerializeField] TextMeshProUGUI txtprogess;
    [SerializeField] Image received;
    [SerializeField] private GameLevel gamelevel;
    public eStateEvent eStateEvent;
    private EventData evendata;
    private GameSupportBonus gameSupportBonus;

    private void Start()
    {
        btnFinshed.onClick.AddListener(GetReward);
    }

    public void GetReward()
    {
        evendata.stateEvent = eStateEvent.REWARDCLAIMED;
        switch (evendata.typeAward)
        {
            case eAwardType.AWARD_BOMB:
                gameSupportBonus.bonusDatas[0].amout += evendata.numberofReward;
                break;
            case eAwardType.AWARD_LIGHTNING:
                gameSupportBonus.bonusDatas[1].amout += evendata.numberofReward;
                break;
            case eAwardType.AWARD_POTION:
                gameSupportBonus.bonusDatas[2].amout += evendata.numberofReward;
                break;
            case eAwardType.AWARD_COIN:
                GameController.Instance.SetCoin(evendata.numberofReward);
                break;
            case eAwardType.AWARD_HEARTS:
                break;
        }
        UpdateUIEvent();
    }
    public void SetEventData(EventData eventData)
    {
        this.evendata = eventData;
    }
    public void SetGameSupportBonus(GameSupportBonus gameSupportBonus)
    {
        this.gameSupportBonus = gameSupportBonus;
    }
    public void UpdateUIEvent()
    {
        if (evendata == null) return;
        eStateEvent = evendata.stateEvent;
        if (evendata.stateEvent == eStateEvent.REWARDCLAIMED)
        {
            AciveState();
            return;
        };
        int level = 0;
        for (int i = 0; i < gamelevel.levels.Count; i++)
        {
            if (gamelevel.levels[i].levelType == eStateLevel.OPEN)
            {
                level = i;
                break;
            }
        }

        if (evendata.eventType == eEventType.EVENT_DAILY)
        {
            evendata.progess = GameController.Instance.GetCurrentProgess();

        }
        else if (evendata.eventType == eEventType.EVENT_NEWBIE)
        {
            evendata.progess = level;
        }

        if (evendata.progess < evendata.CompletionGoal)
        {
            evendata.stateEvent = eStateEvent.UNFINISHED;
        }else if(evendata.progess >= evendata.CompletionGoal)
        {
            evendata.stateEvent = eStateEvent.FINISHED;
        }
        AciveState();
    }

    public void AciveState()
    {
        if (evendata == null) return;
        if (evendata.stateEvent == eStateEvent.UNFINISHED)
        {
            btnFinshed.gameObject.SetActive(false);
            received.gameObject.SetActive(false);
            unfinished.gameObject.SetActive(true);
            txtprogess.text = evendata.progess + "/" + evendata.CompletionGoal;
        }
        else if (evendata.stateEvent == eStateEvent.FINISHED)
        {
            unfinished.gameObject.SetActive(false);
            received.gameObject.SetActive(false);
            btnFinshed.gameObject.SetActive(true);
        }
        else if (evendata.stateEvent == eStateEvent.REWARDCLAIMED)
        {
            unfinished.gameObject.SetActive(false);
            btnFinshed.gameObject.SetActive(false);
            received.gameObject.SetActive(true);
        }
    }
}
