using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;

    public float OriginX = -2.5f;

    public float OriginY = -2.5f;

    public int MatchesMin = 3;

    public float TimeForHint = 5f;

    public int hearts = 2;

    public int heartMax = 5;

    public float MaxTimeHeart = 5;

    public float volumeMusic = 1;

    public float volumeSound = 1;

    public void LoadDataSetting(float music,float sound)
    {
        this.volumeMusic = music;
        this.volumeSound = sound;
    }
}
