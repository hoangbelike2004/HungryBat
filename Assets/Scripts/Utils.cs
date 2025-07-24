using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    static List<NormalItem.eNormalType> eNormals = new List<NormalItem.eNormalType>();
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        //NormalItem.eNormalType[] allValues = ((NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType)));
        NormalItem.eNormalType result = eNormals[URandom.Range(0, eNormals.Count)];

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        return result;
    }

    public static void SetNormals(NormalItem.eNormalType[] normals)
    {
        if (eNormals.Count > 0)
        {
            eNormals.Clear();
        }
        int amount = normals.Length + 3;
        amount = Mathf.Clamp(amount, normals.Length, 7);
        List<NormalItem.eNormalType> allValues = Enum.GetValues(typeof(NormalItem.eNormalType))
                                                     .Cast<NormalItem.eNormalType>()
                                                     .ToList();
        for (int i = 0; i < allValues.Count; i++)
        {
            int rnd = URandom.Range(0, allValues.Count);
            NormalItem.eNormalType t = allValues[i];
            allValues[i] = allValues[rnd];
            allValues[rnd] = t;
        }
        for (int i = 0; i < normals.Length; i++)
        {
            eNormals.Add(normals[i]);
            allValues.Remove(allValues[i]);
        }
        for (int i = 0;i < amount - normals.Length; i++)
        {
            eNormals.Add(allValues[i]);
        }
    }
}
