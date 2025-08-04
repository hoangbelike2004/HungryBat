using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameObject tutorial;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSetting m_gameSettings;

    private List<Cell> m_potentialMatch;

    private float m_timeAfterFill;

    private bool m_hintIsShown;
    private LevelData m_levelData;

    private bool m_gameComplete = false;
    private BonusData m_bonusData;
    public void StartGame(LevelData level, GameSetting gamesetting)
    {
        m_gameSettings = gamesetting;
        m_levelData = level;
        m_cam = Camera.main;
        m_board = new Board(this.transform, m_gameSettings);

        Fill();
    }
    private void Fill()
    {
        m_board.Fill();
        FindMatchesAndCollapse();
    }

    public void SetGameComplete(bool isComplete)
    {
        m_gameComplete = isComplete;
    }
    public void SetBonusData(BonusData bonus)
    {
        m_bonusData = bonus;
    }
    public void UpdateGame()
    {
        if (m_gameComplete) return;
        if (UIManager.Instance.IsOpen<CanvasSetting>()) return;
        if (IsBusy) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
                ShowHint();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (m_bonusData != null)
                {
                    Cell cellcr = hit.collider.GetComponent<Cell>();
                    if (cellcr.Item is NormalItem)
                    {
                        StopHints();
                        NormalItem nor = cellcr.Item as NormalItem;
                        cellcr.StopHintAnimation();
                        cellcr.ExplodeItem();
                        m_board.ConvertNormalToBonus(cellcr, nor.ItemType, m_bonusData.type, m_levelData);
                        cellcr.ExplodeItem();
                        Observer.OnMoveEvent?.Invoke();
                        GameController.Instance.UsedBonus();
                        m_potentialMatch = m_board.GetPotentialMatches();
                        m_bonusData = null;
                        if (GameController.Instance.GetIndexTutotial() == 2)
                        {
                            GameController.Instance.SetIndexTutotial();
                        }
                        return;
                    }
                    //thực hiện convert normal => bonus
                }
                m_isDragging = true;
                m_hitCollider = hit.collider;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }


        if (Input.GetMouseButton(0) && m_isDragging)
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (m_hitCollider != null && m_hitCollider != hit.collider)
                {
                    StopHints();
                    
                    Cell c1 = m_hitCollider.GetComponent<Cell>();
                    Cell c2 = hit.collider.GetComponent<Cell>();
                    if (c1.IsEmpty || c2.IsEmpty)
                    {
                        m_potentialMatch = m_board.GetPotentialMatches();
                        IsBusy = false;
                        return;
                    }
                    if (AreItemsNeighbor(c1, c2))
                    {
                        IsBusy = true;
                        SetSortingLayer(c1, c2);
                        m_board.Swap(c1, c2, () =>//sau khi di chuyen xong thi se bat dau 
                        {
                            FindMatchesAndCollapse(c1, c2);
                        });

                        ResetRayCast();
                    }
                }
            }
            else
            {
                ResetRayCast();
            }
        }
    }

    private void ResetRayCast()
    {
        m_isDragging = false;
        m_hitCollider = null;
    }


    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem && cell2.Item is BonusItem)
        {
            cell1.ExplodeItem();
            cell2.ExplodeItem();
            Observer.OnMoveEvent?.Invoke();
        }
        else if (cell1.Item is BonusItem)
        {
            cell1.ExplodeItem();
            Observer.OnMoveEvent?.Invoke();
        }
        else if (cell2.Item is BonusItem)
        {
            cell2.ExplodeItem();
            Observer.OnMoveEvent?.Invoke();
        }
        else
        {
            List<Cell> cells1 = GetMatches(cell1);//kiem tra xem khi di chuyen den vi tri thi có the xay ra matches theo dọc va ngang
            List<Cell> cells2 = GetMatches(cell2);
            List<Cell> matches = new List<Cell>();

            matches = cells1.Concat(cells2).Distinct().ToList();

            if (matches.Count < m_gameSettings.MatchesMin)//kiem tra xem neu ko xay ra matches thi di chuyen 2 item
            {
                m_board.Swap(cell1, cell2, () =>
                {
                    m_potentialMatch = m_board.GetPotentialMatches();
                    IsBusy = false;
                });
            }
            else
            {
                if (cells1.Count >= m_gameSettings.MatchesMin)
                {
                    CollapseMatches(cells1, cell1);
                }

                if (cells2.Count >= m_gameSettings.MatchesMin)
                {
                    CollapseMatches(cells2, cell2);
                }
                Observer.OnMoveEvent?.Invoke();
            }
        }
    }

    private void FindMatchesAndCollapse()//Goi y cho nguoi choi nhung item co the xep thanh 1 hang va tu dong inactive nhung item danh thanh 1 hang
    {
        List<Cell> matches = m_board.FindFirstMatch();
        if (matches.Count > 0)
        {
            CollapseMatches(matches, null);
        }
        else
        {
            m_potentialMatch = m_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                //StartCoroutine(RefillBoardCoroutine());
                StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    private List<Cell> GetMatches(Cell cell)
    {
        List<Cell> listHor = m_board.GetHorizontalMatches(cell);
        if (listHor.Count < m_gameSettings.MatchesMin)
        {
            listHor.Clear();
        }

        List<Cell> listVert = m_board.GetVerticalMatches(cell);
        if (listVert.Count < m_gameSettings.MatchesMin)
        {
            listVert.Clear();
        }

        return listHor.Concat(listVert).Distinct().ToList();
    }

    private void CollapseMatches(List<Cell> matches, Cell cellEnd)
    {
        NormalItem normalItem = matches[0].Item as NormalItem;
        NormalItem.eNormalType enor = normalItem.ItemType;
        for (int i = 0; i < m_levelData.normalItemtype.Length; i++)
        {
            List<Cell> cellGoals = new List<Cell>();
            for (int j = 0; j < matches.Count; j++)
            {
                if (matches[j].Item is NormalItem)
                {
                    NormalItem nor = matches[j].Item as NormalItem;
                    if (nor.ItemType == m_levelData.normalItemtype[i])
                    {
                        cellGoals.Add(matches[j]);
                    }
                }
            }
            if (cellGoals.Count > 0)
            {
                Observer.OnUpdateScore?.Invoke(m_levelData.normalItemtype[i], cellGoals.Count);
            }
        }
        GameController.Instance.Setscore(matches.Count);
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();//tao ra hieu ung bien mat cho item
        }
        SoundManager.Instance.PlaySoundOnDelay(0.1f, eAudioType.ITEM_NORMAL_DESTROY);

        if (matches.Count > m_gameSettings.MatchesMin)
        {
            m_board.ConvertNormalToBonus(matches, cellEnd, enor, m_levelData);
        }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()//sinh ra nhung item bị bien mat va goi y cho nguoi cho nguoi ch
    {
        IsBusy = true;
        yield return new WaitForSeconds(0.2f);
        m_board.ShiftDownItems();//di chuyen cac item xuong khi cac item ben duoi bi xoa

        yield return new WaitForSeconds(0.4f);

        m_board.FillGapsWithNewItems();

        yield return new WaitForSeconds(0.4f);

        FindMatchesAndCollapse();
    }

    //private IEnumerator RefillBoardCoroutine()
    //{
    //    m_board.ExplodeAllItems();

    //    yield return new WaitForSeconds(0.2f);

    //    m_board.Fill();

    //    yield return new WaitForSeconds(0.2f);

    //    FindMatchesAndCollapse();
    //}

    private IEnumerator ShuffleBoardCoroutine()
    {
        m_board.Shuffle();

        yield return new WaitForSeconds(0.3f);

        FindMatchesAndCollapse();
    }


    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (GameController.Instance.GetIndexTutotial() == 3) return;
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher(2);
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }
    public void ActiveTutorial()
    {
        Cell cel = m_board.GetItemCenter(m_gameSettings);
        cel.Item.SetSortingLayerHigher(4);
        GameObject prefab = Resources.Load<GameObject>(Constants.PREFAB_TUTORIAL);
        if(prefab != null)
        {
            tutorial = GameObject.Instantiate(prefab);
            tutorial.transform.SetParent(cel.Item.View);
            tutorial.transform.localPosition = new Vector3(-0.35f, -0.35f, 0);
            tutorial.transform.DORotate(new Vector3(0, 0, 10), 0.15f).SetLoops(-1, LoopType.Yoyo);
        }
    }
    public void DeactiveTutorial()
    {
        GameObject.Destroy(tutorial);
    }
    private void ShiftOnEvent()
    {
        StartCoroutine(ShiftDownItemsCoroutine());
    }
    public void OnEnable()
    {
        Observer.exploEvent += ShiftOnEvent;
    }
    private void OnDisable()
    {
        Observer.exploEvent -= ShiftOnEvent;
    }
}
