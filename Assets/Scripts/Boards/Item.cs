using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Item
{
    private Transform child;
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    public virtual void SetView()
    {
        PoolType prefabtype = GetPrefabType();

        if (prefabtype != PoolType.ITEM_NONE)
        {
            GameUnit prefab = SimplePool.Spawn<GameUnit>(prefabtype,Vector3.zero,Quaternion.identity);
            if (prefab)
            {
                View = prefab.transform;
                child = View.Find("Visual");
            }
        }
    }

    protected virtual PoolType GetPrefabType() { return PoolType.ITEM_NONE; }

    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null) return;

        View.DOMove(Cell.transform.position, 0.2f);
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View)
        {
            View.position = pos;
        }
    }

    //public void SetViewRoot(Transform root)
    //{
    //    if (View)
    //    {
    //        View.SetParent(root);
    //    }
    //}

    public void SetSortingLayerHigher(int indexLayer)
    {
        if (View == null) return;
        SpriteRenderer sp;
        if (child != null)
        {
            sp = child.GetComponent<SpriteRenderer>();
        }
        else
        {
            sp = View.GetComponent<SpriteRenderer>();
        }
        if (sp)
        {
            sp.sortingOrder = indexLayer;
        }
    }


    public void SetSortingLayerLower()
    {
        if (View == null) return;
        SpriteRenderer sp;
        if (child != null)
        {
            sp = child.GetComponent<SpriteRenderer>();
        }
        else
        {
            sp = View.GetComponent<SpriteRenderer>();
        }
        if (sp)
        {
            sp.sortingOrder = 1;
        }

    }

    internal void ShowAppearAnimation()
    {
        if (View == null) return;

        Vector3 scale = View.localScale;
        View.localScale = Vector3.one * 0.1f;
        View.DOScale(scale, 0.1f);
    }

    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    internal virtual void ExplodeView()
    {
        if (View)
        {
            View.DOScale(0.1f, 0.1f).OnComplete(
                () =>
                {
                    SimplePool.Despawn(View.GetComponent<GameUnit>());
                    View.DOScale(1, 0);
                    ParticalItem par = SimplePool.Spawn<ParticalItem>(PoolType.VFX_NORMALITEM, View.position, Quaternion.identity);
                    par.ActiveAndWaitForDeactive();
                    View = null;
                }
                );
        }
    }
    internal void ExplodeViewBonus()
    {
        if (View == null) return;
        View.GetComponent<SpriteRenderer>().DOFade(1, 0);
        View.DOScale(1, 0);
        View.DORotate(Vector3.zero, 0);
        View = null;
    }

    internal void AnimateForHint()
    {
        if (View)
        {
            View.DOPunchScale(View.localScale * 0.1f, 0.1f).SetLoops(-1);
        }
    }

    internal void StopAnimateForHint()
    {
        if (View)
        {
            View.DOKill();
        }
    }

    internal void Clear()
    {
        Cell = null;

        if (View)
        {
            GameObject.Destroy(View.gameObject);
            View = null;
        }
    }
}
