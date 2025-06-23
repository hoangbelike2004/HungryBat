using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGoal : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtGoal;
    public void UpdateGoal(int score)
    {
        txtGoal.text = score.ToString();
    }
}
