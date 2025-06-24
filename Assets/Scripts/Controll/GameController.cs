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
    private CanvasMain m_canvasMain;
    private int star, numbermove, sumItemAmout;
    private float completionrate;
    bool isCoroutineRunning;
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
    }
    public void SetState(eStateGame state)
    {
        stateGame = state;
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
        m_canvasGamePlay = UIManager.Instance.OpenUI<CanvasGamePlay>();
        m_canvasGamePlay.SetLevelData(_levelData);
        completionrate = 0;
        numbermove = _levelData.moveNumber;
        itemScore = new List<int>();
        for (int i = 0; i < _levelData.itemAmount.Length; i++)
        {
            itemScore.Add(_levelData.itemAmount[i]);
            sumItemAmout += _levelData.itemAmount[i];
        }
        m_boarcontroll = new GameObject("BoardController").AddComponent<BoardController>();
        m_boarcontroll.StartGame(_levelData);
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
        ClearLevel();
    }
    internal void ClearLevel()
    {
        if (m_boarcontroll)
        {
            DOTween.KillAll();
            m_canvasGamePlay.ClearGoal();
            m_boarcontroll.Clear();
            Destroy(m_boarcontroll.gameObject);
            m_boarcontroll = null;
            m_canvasGamePlay = null;
        }
    }
    public void ShowMenu()
    {
        ClearLevel();
        m_canvasMain = UIManager.Instance.OpenUI<CanvasMain>();
        m_canvasMain.UpdateLevelUi();
        m_canvasMain.UpdateLevelUi();
    }
    private IEnumerator WaitBoardController()
    {
        if (isCoroutineRunning) yield return null;
        while (m_boarcontroll != null && m_boarcontroll.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }
        m_boarcontroll.SetGameComplete(true);
        yield return new WaitForSeconds(1f);
        if (star >= _levelData.starNumber)
        {
            _levelData.starNumber = star;
            if(_levelData.starNumber == 3)
            {
                _levelData.levelType = eStateLevel.COMPLETE;
            }
        }
        CanvasComplete completeUi = UIManager.Instance.OpenUI<CanvasComplete>();
        completeUi.OnActive(0,_levelData.starNumber);
    }
    public void SetMove()
    {
        numbermove--;
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate);
        if (numbermove == 0)
        {
            isCoroutineRunning = true;
            StartCoroutine(WaitBoardController());
        }
    }
    public void SetFruitGoalAndCompletionrate(NormalItem.eNormalType type, int score)//update ui mục tiêu và tỉ lệ hoàn thành
    {
        int tmp = 0;
        for (int i = 0; i < itemScore.Count; i++)
        {
                if (_levelData.normalItem[i] == type)
                {
                    itemScore[i] -= score;
                    if (itemScore[i] < 0)
                    {
                        itemScore[i] = 0;
                    }
                }
            tmp += itemScore[i];
        }
        completionrate = (float)(sumItemAmout - tmp) / sumItemAmout;
        star = completionrate < 0.3f ? 0 : (completionrate < 0.6f ? 1 : (completionrate < 1f ? 2 : 3));
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate);
        if (star == 3)
        {
            isCoroutineRunning = true;
            StartCoroutine(WaitBoardController());
        }
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
