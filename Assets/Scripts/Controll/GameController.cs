using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum eStateGame
{
    MAIN_MENU,
    STARTED,
    COMPLETE,
}
public class GameController : Singleton<GameController>
{
    private eStateGame stateGame;
    private LevelData _levelData;
    private BoardController m_boarcontroll;
    private CanvasGamePlay m_canvasGamePlay;
    private int star, numbermove, sumItemAmout;
    private float completionrate;
    public List<int> itemScore;
    private void Start()
    {
        UIManager.Instance.OpenUI<CanvasMain>();
    }
    public void Update()
    {
        if (m_boarcontroll != null)
        {
            m_boarcontroll.UpdateGame();
        }
    }
    public void SetLevelData(LevelData levelData)//cần setdata khi started
    {
        _levelData = levelData;
        SetState(eStateGame.STARTED);
    }
    public void SetState(eStateGame state)
    {
        stateGame = state;
        ChangeState();
    }

    public void ChangeState()
    {
        switch (stateGame)
        {
            case eStateGame.MAIN_MENU:
                ShowMenu();
                break;
            case eStateGame.STARTED:
                GameStart();
                break;
            case eStateGame.COMPLETE:
                GameComplete();
                break;
        }
    }
    public void GameStart()
    {
        m_boarcontroll = new GameObject("BoardController").AddComponent<BoardController>();
        m_boarcontroll.StartGame(_levelData);
        m_canvasGamePlay = UIManager.Instance.OpenUI<CanvasGamePlay>();
        m_canvasGamePlay.SetLevelData(_levelData);
        completionrate = 1;
        numbermove = _levelData.moveNumber;
        itemScore = new List<int>();
        for(int i = 0;i<_levelData.itemAmount.Length;i++)
        {
            itemScore.Add(_levelData.itemAmount[i]);
            sumItemAmout += _levelData.itemAmount[i];
        }
    }
    public void GameComplete()
    {
        if (star > _levelData.starNumber)
        {
            _levelData.starNumber = star;
            if (_levelData.starNumber == 3)
            {
                _levelData.levelType = eStateLevel.COMPLETE;
            }
        }
    }
    internal void ClearLevel()
    {
        if (m_boarcontroll)
        {
            DOTween.KillAll();
            m_boarcontroll.Clear();
            Destroy(m_boarcontroll.gameObject);
            m_boarcontroll = null;
            m_canvasGamePlay = null;
        }
    }
    public void ShowMenu()
    {
        ClearLevel();
        CanvasMain main = UIManager.Instance.OpenUI<CanvasMain>();
        main.UpdateLevelUi();
    }
    private IEnumerator WaitBoardController()
    {
        while (m_boarcontroll.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        UIManager.Instance.OpenUI<CanvasComplete>();
    }
    public void SetMove()
    {
        numbermove--;
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate);
        if(numbermove == 0)
        {
            SetState(eStateGame.COMPLETE);
        }
    }
    public void SetFruitGoalAndCompletionrate(NormalItem.eNormalType type,int score)//update ui mục tiêu và tỉ lệ hoàn thành
    {
        int tmp = 0;
        for(int i = 0; i < itemScore.Count; i++)
        {
            if (_levelData.normalItem[i] == type)
            {
                itemScore[i] -= score;
                if(itemScore[i] < 0)
                {
                    itemScore[i] = 0;
                }
            }
            tmp += itemScore[i];
        }
        completionrate = (float)(sumItemAmout - tmp) / sumItemAmout;
        star = completionrate < 0.3f ? 0 : completionrate < 0.6f ? 1 : completionrate < 1f ? 2 : 3; 
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate);
    }
    private void OnEnable()
    {
        Observer.OnMoveEvent += SetMove;
        Observer.OnUpdateScore += SetFruitGoalAndCompletionrate;
    }
    private void OnDisable()
    {
        Observer.OnMoveEvent -= SetMove;
        Observer.OnUpdateScore -= SetFruitGoalAndCompletionrate;
    }
}
