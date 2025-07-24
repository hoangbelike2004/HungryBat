using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : Item
{
    public enum eNormalType
    {
        TYPE_ONE,
        TYPE_TWO,
        TYPE_THREE,
        TYPE_FOUR,
        TYPE_FIVE,
        TYPE_SIX,
        TYPE_SEVEN,
    }

    public eNormalType ItemType;

    public void SetType(eNormalType type)
    {
        ItemType = type;
    }

    protected override PoolType GetPrefabType()
    {
        PoolType prefabtype = PoolType.ITEM_NONE;
        switch (ItemType)
        {
            case eNormalType.TYPE_ONE:
                prefabtype = PoolType.ITEM_APPLE;
                break;
            case eNormalType.TYPE_TWO:
                prefabtype = PoolType.ITEM_BANANA;
                break;
            case eNormalType.TYPE_THREE:
                prefabtype = PoolType.ITEM_BLUEBERRY;
                break;
            case eNormalType.TYPE_FOUR:
                prefabtype = PoolType.ITEM_GRAPE;
                break;
            case eNormalType.TYPE_FIVE:
                prefabtype = PoolType.ITEM_ORANGE;
                break;
            case eNormalType.TYPE_SIX:
                prefabtype = PoolType.ITEM_PEAR;
                break;
            case eNormalType.TYPE_SEVEN:
                prefabtype = PoolType.ITEM_STRAWBERRY;
                break;
        }

        return prefabtype;
    }

    internal override bool IsSameType(Item other)
    {
        NormalItem it = other as NormalItem;

        return it != null && it.ItemType == this.ItemType;
    }
}
