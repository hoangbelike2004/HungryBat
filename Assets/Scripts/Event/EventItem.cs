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

    private EventData evendata;

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
                break;
            case eAwardType.AWARD_LIGHTNING:
                break;
            case eAwardType.AWARD_POTION:
                break;
            case eAwardType.AWARD_COIN:
                break;
            case eAwardType.AWARD_HEARTS:
                break;
        }
    }
    public void SetEventData(EventData eventData)
    {
        this.evendata = eventData;
    }
    public void UpdateUIEvent()
    {
        if (evendata == null) return;
        if (evendata.stateEvent == eStateEvent.REWARDCLAIMED) return;
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
