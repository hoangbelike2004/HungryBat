using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SupportBonus")]
public class GameSupportBonus : ScriptableObject
{
    public List<BonusData> bonusDatas = new List<BonusData>();
    public void LoadDataItem(List<BonusData> amouts)
    {
        for (int i = 0; i < bonusDatas.Count; i++)
        {
            bonusDatas[i].amout = amouts[i].amout;
            bonusDatas[i].state = amouts[i].state;
        }
    }
    public void Check()
    {
        for (int i = 0; i < bonusDatas.Count; i++)
        {
            if (bonusDatas[i].amout < 0)
            {
                Debug.Log("So luong vat pham dac biet > 0 vat pham thu: " + (i + 1));
            }
            if (bonusDatas[i].price < 0)
            {
                Debug.Log("Gia vat pham dac biet > 0 vat pham thu: " + (i + 1));
            }
            if (bonusDatas[i].name == null)
            {
                Debug.Log("Ten vat pham khong duoc de trong vat pham thu: " + (i + 1));
            }
            if (bonusDatas[i].description == null)
            {
                Debug.Log("Mo ta vat pham khong duoc de trong vat pham thu: " + (i + 1));
            }
            if (bonusDatas[i].prefab == null)
            {
                Debug.Log("Icon khong duoc de trong, vat pham thu: " + (i + 1));
            }
        }
    }
}
