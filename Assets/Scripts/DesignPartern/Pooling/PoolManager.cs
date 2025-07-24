using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolAmount[] poolAmounts;


    private void Awake()
    {
        for (int i = 0; i < poolAmounts.Length; i++)
        {
            SimplePool.PreLoad(poolAmounts[i].prefab, poolAmounts[i].amount, poolAmounts[i].parent);
        }
    }

}

[System.Serializable]
public class PoolAmount
{
    public int amount;
    public GameUnit prefab;
    public Transform parent;
}

public enum PoolType
{
    POOL_SOUND,
    POOL_MUSIC,
    ITEM_APPLE,
    ITEM_BANANA,
    ITEM_BLUEBERRY,
    ITEM_GRAPE,
    ITEM_ORANGE,
    ITEM_PEAR,
    ITEM_STRAWBERRY,
    ITEM_HORIZONTAL,
    ITEM_VERTICAL,
    ITEM_BOMB,
    ITEN_LIGHTNING,
    ITEM_POTION,
    ITEM_NONE,
    BG0,
    BG1,
    VFX_BOMB,
    VFX_POTIONBLUE,
    VFX_POTIONRED,
    VFX_POTIONORANGEANDYELLOW,
    VFX_POTIONPURPLE,
    VFX_POTIONGREEN,
    VFX_NORMALITEM,
    VFX_LINGTNING,
}