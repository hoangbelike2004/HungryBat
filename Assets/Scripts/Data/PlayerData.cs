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
    public List<int> AmoutItemBonus;
    public float VolumeMusic;
    public float volumeSound;
    public PlayerData(int heart, int coin, LevelData currentlevel, int currentProgess, List<EventData> events,
    List<int> AmoutItemBonus, float VolumeMusic, float VolumeSound)
    {
        this.hearts = heart;
        this.coin = coin;
        this.currentlevel = currentlevel;
        this.currentProgess = currentProgess;//reset
        this.events = events;//reset
        this.AmoutItemBonus = AmoutItemBonus;
        this.VolumeMusic = VolumeMusic;
        this.volumeSound = VolumeSound;
    }
}
