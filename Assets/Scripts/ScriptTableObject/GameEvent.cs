using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public List<EventData> events = new List<EventData>();
    public void LoadDataEvent(List<EventData> eventdatas)
    {
        for(int i = 0; i < eventdatas.Count; i++)
        {
            events[i].stateEvent = eventdatas[i].stateEvent;
            events[i].progess = eventdatas[i].progess;

        }
        if (PlayerPrefs.HasKey(Constants.KEY_LATE_LOGIN_DATE))
        {
            string strDate = PlayerPrefs.GetString(Constants.KEY_LATE_LOGIN_DATE);
            DateTime latedate = DateTime.Parse(strDate);
            DateTime today = DateTime.Now.Date;
            if(today > latedate)
            {
                for (int i = 0; i < eventdatas.Count; i++)
                {
                    if (events[i].eventType == eEventType.EVENT_DAILY)
                    {
                        events[i].stateEvent = eStateEvent.UNFINISHED;
                        events[i].progess = 0;
                    }

                }
            }
        }
    }
    public void Check()
    {
        for(int i = 0; i<events.Count; i++)
        {
            if (events[i].CompletionGoal < 0)
            {
                Debug.Log("Muc tieu hoan thanh > 0 nhiem vu: "+ (i + 1));
            }
            if(events[i].numberofReward < 0)
            {
                Debug.Log("Phan thuong phai > 0 nhiem vu: "+(i + 1));
            }
            if (events[i].description == null)
            {
                Debug.LogError("Noi dung nhiem vu khong duoc de trong: " + (i + 1));
            }
        }
    }
}
