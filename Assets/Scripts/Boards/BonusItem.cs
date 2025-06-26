using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum eBonusType
{
    NONE,
    HORIZONTAL,
    VERTICAL,
    BOMB,
    LIGHTNING,
    POTION,
}
public class BonusItem : Item
{
    public eBonusType ItemType;
    public Cell[,] cells;
    public NormalItem.eNormalType normal;
    private LevelData levelData;

    public void SetLevelData(LevelData levelData)
    {
        this.levelData = levelData;
    }

    public void SetType(eBonusType type)
    {
        ItemType = type;
    }
    public void SetCells(Cell[,] cells)
    {
        this.cells = cells;
    }
    public void SetEnormal(NormalItem.eNormalType type)
    {
        normal = type;
    }
    protected override string GetPrefabName()
    {
        string prefabname = string.Empty;
        switch (ItemType)
        {
            case eBonusType.NONE:
                break;
            case eBonusType.HORIZONTAL:
                prefabname = Constants.PREFAB_BONUS_HORIZONTAL;
                break;
            case eBonusType.VERTICAL:
                prefabname = Constants.PREFAB_BONUS_VERTICAL;
                break;
            case eBonusType.BOMB:
                prefabname = Constants.PREFAB_BONUS_BOMB;
                break;
            case eBonusType.LIGHTNING:
                prefabname = Constants.PREFAB_BONUS_LIGHTNING;
                break;
            case eBonusType.POTION:
                prefabname = Constants.PREFAB_BONUS_POTION;
                break;
        }

        return prefabname;
    }

    internal override bool IsSameType(Item other)
    {
        BonusItem it = other as BonusItem;

        return it != null && it.ItemType == this.ItemType;
    }

    internal override void ExplodeView()
    {
        SpriteRenderer spr = View.GetComponent<SpriteRenderer>();
        switch (ItemType)
        {
            case eBonusType.HORIZONTAL:
                View.DOScale(1.4f, 0.4f);
                spr.DOFade(0.1f, 0.4f).OnComplete(() =>
                {
                    GameObject.Destroy(View.gameObject);
                    SetNullView();
                    ExplodeHorizontal();
                });
                break;
            case eBonusType.VERTICAL:
                View.DOScale(1.4f, 0.4f);
                spr.DOFade(0.1f, 0.4f).OnComplete(() =>
                {
                    GameObject.Destroy(View.gameObject);
                    SetNullView();
                    ExplodeVertical();
                });
                break;
            case eBonusType.BOMB:
                View.DOShakePosition(0.75f, 0.1f, 100, 90, false, true);
                View.DORotate(new Vector3(0, 0, -40), 0.15f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.InOutQuad);
                View.DOScale(1.4f, 0.15f).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
                {
                    GameObject.Destroy(View.gameObject);
                    SetNullView();
                    ExplodeBomb();
                });
                break;
            case eBonusType.LIGHTNING:
                View.DOScale(1.4f, 0.4f);
                spr.DOFade(0.1f, 0.4f).OnComplete(() =>
                {
                    GameObject.Destroy(View.gameObject);
                    SetNullView();
                    ExplodeLightning();
                });
                break;
            case eBonusType.POTION:
                View.DOShakePosition(0.75f, 0.1f, 100, 90, false, true);
                View.DORotate(new Vector3(0, 0, -40), 0.15f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.InOutQuad);
                View.DOScale(1.4f, 0.15f).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
                {
                    GameObject.Destroy(View.gameObject);
                    SetNullView();
                    ExplodePotion();
                });
                break;

        }
    }
    private void ExplodeBomb()
    {
        List<Cell> list = new List<Cell>();
        if (Cell.NeighbourBottom) list.Add(Cell.NeighbourBottom);
        if (Cell.NeighbourUp) list.Add(Cell.NeighbourUp);
        if (Cell.NeighbourLeft)
        {
            list.Add(Cell.NeighbourLeft);
            if (Cell.NeighbourLeft.NeighbourUp)
            {
                list.Add(Cell.NeighbourLeft.NeighbourUp);
            }
            if (Cell.NeighbourLeft.NeighbourBottom)
            {
                list.Add(Cell.NeighbourLeft.NeighbourBottom);
            }
        }
        if (Cell.NeighbourRight)
        {
            list.Add(Cell.NeighbourRight);
            if (Cell.NeighbourRight.NeighbourUp)
            {
                list.Add(Cell.NeighbourRight.NeighbourUp);
            }
            if (Cell.NeighbourRight.NeighbourBottom)
            {
                list.Add(Cell.NeighbourRight.NeighbourBottom);
            }
        }

        bool isBonus = true;
        GetFruitGoal(list);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Item is BonusItem)
            {
                isBonus = false;
            }
            list[i].ExplodeItem();
        }
        if (isBonus)
        {
            Observer.exploEvent.Invoke();
        }
    }

    public void ExplodeHorizontal()
    {
        List<Cell> list = new List<Cell>();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourRight;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourLeft;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }


        bool isBonus = true;
        GetFruitGoal(list);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Item is BonusItem)
            {
                isBonus = false;
            }
            list[i].ExplodeItem();
        }
        if (isBonus)
        {
            Observer.exploEvent.Invoke();
        }
    }
    public void ExplodeVertical()
    {
        List<Cell> list = new List<Cell>();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourUp;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourBottom;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }

        bool isBonus = true;
        GetFruitGoal(list);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Item is BonusItem)
            {
                isBonus = false;
            }
            list[i].ExplodeItem();
        }
        if (isBonus)
        {
            Observer.exploEvent.Invoke();
        }
    }
    public void ExplodeLightning()
    {
        List<Cell> list = new List<Cell>();
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (!cells[i, j].IsEmpty)
                {
                    list.Add(cells[i, j]);
                }
            }
        }
        int rnd = UnityEngine.Random.Range(7, 10);
        for (int i = 0; i < list.Count; i++)
        {
            int j = UnityEngine.Random.Range(i,list.Count);
            Cell cell = list[i];
            list[i] = list[j];
            list[j] = cell;
        }
        bool isBonus = true;
        if(rnd <= list.Count)
        {
            List<Cell> cells = list.GetRange(0, rnd);
            GetFruitGoal(cells);
            for (int i = 0;i < rnd; i++)
            {
                if(list[i].Item is BonusItem)
                {
                    isBonus = false;
                }
                list[i].ExplodeItem();
            }
        }
        else
        {
            GetFruitGoal(list);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item is BonusItem)
                {
                    isBonus = false;
                }
                list[i].ExplodeItem();
            }
        }
        if (isBonus)
        {
            Observer.exploEvent.Invoke();
        }
    }

    public void ExplodePotion()
    {
        int rnd = UnityEngine.Random.Range(7, 10);
        List<Cell> list = new List<Cell>();
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (!cells[i, j].IsEmpty && !(cells[i,j].Item is BonusItem))
                {
                    NormalItem item = cells[i, j].Item as NormalItem;
                    if (item.ItemType == normal)
                    {
                        list.Add(cells[i, j]);
                    }
                }
            }
        }
        bool isBonus = true;
        if (rnd <= list.Count)
        {
            List<Cell> listRange = list.GetRange(0, rnd);
            GetFruitGoal(list);
            for (int i = 0; i < rnd; i++)
            {
                if (list[i].Item is BonusItem)
                {
                    isBonus = false;
                }
                list[i].ExplodeItem();
            }
        }
        else
        {
            GetFruitGoal(list);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item is BonusItem)
                {
                    isBonus = false;
                }
                list[i].ExplodeItem();
            }
        }
        if (isBonus)
        {
            Observer.exploEvent.Invoke();
        }
    }

    public void GetFruitGoal(List<Cell> cells)
    {
        for (int i = 0; i < levelData.normalItem.Length; i++)
        {
            List<Cell> cellGoals = new List<Cell>();
            for (int j = 0; j < cells.Count; j++)
            {
                if (cells[j].Item is NormalItem)
                {
                    NormalItem nor = cells[j].Item as NormalItem;
                    if (nor.ItemType == levelData.normalItem[i])
                    {
                        cellGoals.Add(cells[j]);
                    }
                }
            }
            if (cellGoals.Count > 0)
            {
                Observer.OnUpdateScore?.Invoke(levelData.normalItem[i], cellGoals.Count);
            }
        }
        GameController.Instance.Setscore(cells.Count);
    }
}
