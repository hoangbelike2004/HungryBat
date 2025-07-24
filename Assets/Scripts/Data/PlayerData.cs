using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public int hearts;
    public int coin;
    public LevelData currentlevel;
    public int currentProgess;
    public List<EventData> events;
    public List<BonusData> Itembonuses;
    public float VolumeMusic;
    public float volumeSound;
    public PlayerData(int heart, int coin, LevelData currentlevel, int currentProgess, List<EventData> events,
    List<BonusData> Itembonuses, float VolumeMusic, float VolumeSound)
    {
        this.hearts = heart;
        this.coin = coin;
        this.currentlevel = currentlevel;
        this.currentProgess = currentProgess;//reset
        this.events = events;//reset
        this.Itembonuses = Itembonuses;
        this.VolumeMusic = VolumeMusic;
        this.volumeSound = VolumeSound;
    }
}
