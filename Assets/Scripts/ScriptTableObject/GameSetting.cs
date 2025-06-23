using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;

    public int MatchesMin = 3;

    public int LevelMoves = 16;

    public float LevelTime = 30f;

    public float TimeForHint = 5f;
}
