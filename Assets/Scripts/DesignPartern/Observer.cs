using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Observer
{
    public static UnityAction exploEvent;
    public static UnityAction OnMoveEvent;
    public static UnityAction<NormalItem.eNormalType,int> OnUpdateScore;
    public static UnityAction NotificationEvent;
}
