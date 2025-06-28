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
    public eStateGame StateGame => stateGame;
    public int coin;
    private eStateGame stateGame;
    private LevelData _levelData;
    private BoardController m_boarcontroll;
    private CanvasGamePlay m_canvasGamePlay;
    private CanvasMain m_canvasMain;
    private int star, numbermove, sumItemAmout, score,currentProgess;
    private float completionrate;
    private bool isCoroutineRunning;
    private List<int> itemScore;
    private BonusData bonusdata;
    private GameSupportBonus m_gameSupportBonus;
    private GameEvent m_gameevent;
    private void Start()
    {
        m_canvasMain = UIManager.Instance.OpenUI<CanvasMain>();
        m_gameSupportBonus = Resources.Load<GameSupportBonus>(Constants.GAME_SUPPORT_BONUS_PATH);
        m_gameevent = Resources.Load<GameEvent>(Constants.GAME_EVENT_PATH);
        m_canvasMain.SetGameSupportBonus(m_gameSupportBonus);
        m_canvasMain.SetGameEvent(m_gameevent);
        m_canvasMain.SetState(eStateMain.HOME);
        m_canvasMain.UpdateCoin(coin);
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
    public void SetBonusData(BonusData data)
    {
        if (bonusdata != null) return;
        bonusdata = data;
        if (m_boarcontroll != null)
        {
            m_boarcontroll.SetBonusData(data);
        }
    }
    public void UsedBonus()
    {
        bonusdata.amout -= 1;
        bonusdata = null;
        m_canvasGamePlay.UpdateUISupportBonus();
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
        m_canvasGamePlay.UpdateUISupportBonus();
        sumItemAmout = 0;
        score = 0;
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
        Debug.Log(1);
        if (star > _levelData.starNumber)
        {
            _levelData.starNumber = star;
            if (_levelData.starNumber == 3)
            {
                _levelData.levelType = eStateLevel.COMPLETE;
                if(_levelData.coin > 0)
                {
                    coin += _levelData.coin;
                    _levelData.coin = 0;
                    m_canvasMain.UpdateCoin(coin);
                }
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
            itemScore.Clear();
            StopAllCoroutines();
        }
    }
    public void ShowMenu()
    {
        ClearLevel();
        m_canvasMain.Open();
        m_canvasMain.UpdateLevelUi();
    }
    public void ReLoad()
    {
        score = 0;
        completionrate = 0;
        for (int i = 0; i < _levelData.itemAmount.Length; i++)
        {
            itemScore[i] = _levelData.itemAmount[i];
        }
        numbermove = _levelData.moveNumber;
        itemScore = new List<int>();
        m_boarcontroll.SetGameComplete(false);
        m_canvasGamePlay.ResetUI();
        Setscore(score);
    }
    private IEnumerator WaitBoardController()
    {
        if (isCoroutineRunning) yield return null;
        while (m_boarcontroll != null && m_boarcontroll.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }
        m_boarcontroll.SetGameComplete(true);
        int cointmp = 0;
        yield return new WaitForSeconds(1f);
        if (star >= _levelData.starNumber)
        {
            _levelData.starNumber = star;
            if (_levelData.starNumber == 3)
            {
                _levelData.levelType = eStateLevel.COMPLETE;
                if (_levelData.coin > 0)
                {
                    coin += _levelData.coin;
                    cointmp = _levelData.coin;
                    _levelData.coin = 0;
                    m_canvasMain.UpdateCoin(coin);
                }
            }
        }
        CanvasComplete completeUi = UIManager.Instance.OpenUI<CanvasComplete>();
        completeUi.OnActive(score, _levelData.starNumber,cointmp);
        completeUi.SetLevelData(_levelData);
        currentProgess++;
    }
    public void Setscore(int score)
    {
        this.score += score;
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate, this.score);
    }
    public void SetMove()
    {
        numbermove--;
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate, score);
        if (numbermove == 0)
        {
            isCoroutineRunning = true;
            StartCoroutine(WaitBoardController());
        }
    }
    public void SetFruitGoalAndCompletionrate(NormalItem.eNormalType type, int numbergoal)//update ui mục tiêu và tỉ lệ hoàn thành
    {
        int tmp = 0;
        for (int i = 0; i < itemScore.Count; i++)
        {
            if (_levelData.normalItem[i] == type)
            {
                itemScore[i] -= numbergoal;
                if (itemScore[i] < 0)
                {
                    itemScore[i] = 0;
                }
            }
            tmp += itemScore[i];
        }
        completionrate = (float)(sumItemAmout - tmp) / sumItemAmout;
        star = completionrate < 0.3f ? 0 : (completionrate < 0.6f ? 1 : (completionrate < 1f ? 2 : 3));
        m_canvasGamePlay.UpdateUI(numbermove, itemScore, completionrate, this.score);
        if (star == 3)
        {
            isCoroutineRunning = true;
            StartCoroutine(WaitBoardController());
        }
    }
    public void SetCoin(int coin)
    {
        this.coin -= coin;
    }
    public int GetCoin()
    {
        return this.coin;
    }

    public int GetCurrentProgess()
    {
        return currentProgess;
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
