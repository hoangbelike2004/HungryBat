using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAwardType
{
    AWARD_BOMB,
    AWARD_LIGHTNING,
    AWARD_POTION,
    AWARD_COIN,
    AWARD_HEARTS,
}
public enum eEventType
{
    EVENT_DAILY,
    EVENT_NEWBIE,
}
public enum eStateEvent
{
    UNFINISHED,
    FINISHED,
    REWARDCLAIMED,
}
[System.Serializable]
public class EventData
{
    public string description;
    public eAwardType typeAward;
    public eEventType eventType;
    public eStateEvent stateEvent;
    public int CompletionGoal;
    public int progess;
    public int numberofReward;
}
