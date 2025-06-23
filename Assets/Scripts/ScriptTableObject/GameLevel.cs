using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel")]
public class GameLevel : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}
