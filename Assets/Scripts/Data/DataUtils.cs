using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataUtils
{
    public static void SaveData(int heart, int coin, LevelData currentlevel, int currentProgess, List<EventData> events,
    List<int> AmoutItemBonus, float VolumeMusic, float VolumeSound)
    {
        PlayerData dataplayer = new PlayerData(heart, coin, currentlevel, currentProgess,
            events, AmoutItemBonus, VolumeMusic, VolumeSound);
        string json = JsonUtility.ToJson(dataplayer);
        PlayerPrefs.SetString(Constants.KEY_DATA_PLAYER, json);
    }
    public static PlayerData LoadData()
    {
        if(PlayerPrefs.HasKey(Constants.KEY_DATA_PLAYER))
        {
            string json = PlayerPrefs.GetString(Constants.KEY_DATA_PLAYER);
            PlayerData playerdata = JsonUtility.FromJson<PlayerData>(json);
            return playerdata;
        }
        return null;
    }
}
