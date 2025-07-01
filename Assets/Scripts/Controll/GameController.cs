using DG.Tweening;
using System;
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
    public int coin, currentProgess;
    private eStateGame stateGame;
    private LevelData _levelData;
    private BoardController m_boarcontroll;
    private CanvasGamePlay m_canvasGamePlay;
    private CanvasMain m_canvasMain;
    private CanvasOutofLives m_canvasOutofLives;
    private int star, numbermove, sumItemAmout, score, hearts;
    private float completionrate, timeHeart;
    private bool isCoroutineRunning;
    private List<int> itemScore;
    private BonusData bonusdata;
    private GameSupportBonus m_gameSupportBonus;
    private GameEvent m_gameevent;
    private GameSetting m_gameSetting;
    private GameLevel m_gamelevel;
    private void Awake()
    {
        m_gameSetting = Resources.Load<GameSetting>(Constants.GAME_SETTINGS_PATH);
        m_gameSupportBonus = Resources.Load<GameSupportBonus>(Constants.GAME_SUPPORT_BONUS_PATH);
        m_gameevent = Resources.Load<GameEvent>(Constants.GAME_EVENT_PATH);
        m_gamelevel = Resources.Load<GameLevel>(Constants.GAME_LEVEL_PATH);
    }
    private void Start()
    {
        LoadDataPlayer();
        LoadHeartData();
        m_canvasMain = UIManager.Instance.OpenUI<CanvasMain>();
        m_canvasMain.SetGameSupportBonus(m_gameSupportBonus);
        m_canvasMain.SetGameEvent(m_gameevent);
        m_canvasMain.SetGameLevel(m_gamelevel);
        m_canvasMain.SetState(eStateMain.HOME);
        m_canvasMain.UpdateCoin(coin);
        SoundManager.Instance.PlaySound(eAudioType.MUCSIC_MAIN_MENU);
    }
    public void FixedUpdate()
    {
        if (m_gameSetting.hearts < m_gameSetting.heartMax)
        {
            timeHeart += Time.fixedDeltaTime;
            TimeSpan timespan = TimeSpan.FromSeconds(timeHeart);
            string strTime = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
            if(m_canvasOutofLives != null)
            {
                m_canvasOutofLives.UpdateUI(m_gameSetting.hearts,strTime);
            }
            if (timespan.TotalMinutes >= m_gameSetting.MaxTimeHeart)
            {
                m_gameSetting.hearts++;
                timeHeart = 0;
            }
            m_canvasMain.UpdateTimeAndHeart(strTime, m_gameSetting.hearts);
        }
        else
        {
            timeHeart = 0;
            m_canvasMain.UpdateTimeAndHeart("Full", m_gameSetting.hearts);
            if (m_canvasOutofLives != null)
            {
                m_canvasOutofLives.UpdateUI(m_gameSetting.hearts, "Full");
            }
        }
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
        if (bonusdata == data) return;
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
        SoundManager.Instance.PlaySound(eAudioType.MUCSIC_GAME_PLAY);
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
        m_boarcontroll.StartGame(_levelData, m_gameSetting);
    }
    public void GameComplete()
    {
        if (star > _levelData.starNumber)
        {
            _levelData.starNumber = star;
            if (_levelData.starNumber == 3)
            {
                _levelData.levelType = eStateLevel.COMPLETE;
                if (_levelData.coin > 0)
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
        SoundManager.Instance.PlaySound(eAudioType.MUCSIC_MAIN_MENU);
    }
    //public void ReLoad()
    //{
    //    score = 0;
    //    completionrate = 0;
    //    for (int i = 0; i < _levelData.itemAmount.Length; i++)
    //    {
    //        itemScore[i] = _levelData.itemAmount[i];
    //    }
    //    numbermove = _levelData.moveNumber;
    //    itemScore = new List<int>();
    //    m_boarcontroll.SetGameComplete(false);
    //    m_canvasGamePlay.ResetUI();
    //    Setscore(score);
    //}
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
        if (star < 3)
        {
            m_gameSetting.hearts--;
        }
        CanvasComplete completeUi = UIManager.Instance.OpenUI<CanvasComplete>();
        completeUi.OnActive(score, star, cointmp);
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
    public void SetHearts()
    {
        m_gameSetting.hearts -= 1;
    }
    public int GetHeart()
    {
        return m_gameSetting.hearts;
    }

    public int GetCurrentProgess()
    {
        return currentProgess;
    }
    public void SetCanvasOutOfLives(CanvasOutofLives canvas)
    {
        m_canvasOutofLives = canvas;
    }
    private void OnApplicationQuit()
    {
        if (m_gameSetting.hearts < m_gameSetting.heartMax)
        {
            DateTime date = DateTime.Now;
            PlayerPrefs.SetString("datequit", date.ToString());
            PlayerPrefs.SetFloat("timeheart", timeHeart);//thoi gian dang dem khi thoat game
        }
        SaveDataPlayer();
    }

    //SAVE AND LOAD DATALOCAL
    public void SaveDataPlayer()
    {
        LevelData level = new LevelData();
        for (int i = 0; i < m_gamelevel.levels.Count; i++)
        {
            if (m_gamelevel.levels[i].levelType == eStateLevel.OPEN)
            {
                level = m_gamelevel.levels[i];
                break;
            }
        }
        List<int> amoutBonus = new List<int>();
        for (int i = 0; i < m_gameSupportBonus.bonusDatas.Count; i++)
        {
            amoutBonus.Add(m_gameSupportBonus.bonusDatas[i].amout);
        }
        DataUtils.SaveData(m_gameSetting.hearts, coin, level, currentProgess, m_gameevent.events
            , amoutBonus, m_gameSetting.volumeMusic, m_gameSetting.volumeSound);
        DateTime date = DateTime.Now;
        PlayerPrefs.SetString(Constants.KEY_LATE_LOGIN_DATE,date.ToString("yyyy-MM-dd"));
    }

    public void LoadDataPlayer()
    {
        PlayerData playerdata = DataUtils.LoadData();
        if (playerdata != null)
        {
            //loadlevel
            LevelData leveldata = playerdata.currentlevel;
            m_gamelevel.LoadDataLevel(leveldata);
            //loadItemBonus
            m_gameSupportBonus.LoadDataItem(playerdata.AmoutItemBonus);
            //loadsetting
            m_gameSetting.LoadDataSetting(playerdata.VolumeMusic,playerdata.volumeSound);
            //loadevent
            if (PlayerPrefs.HasKey(Constants.KEY_LATE_LOGIN_DATE))
            {
                string strDate = PlayerPrefs.GetString(Constants.KEY_LATE_LOGIN_DATE);
                DateTime latedate = DateTime.Parse(strDate);
                DateTime today = DateTime.Now.Date;
                if (today > latedate)
                {
                    Debug.Log(1);
                    currentProgess = 0;
                }
                else
                {
                    currentProgess = playerdata.currentProgess;
                }
            }
            m_gameevent.LoadDataEvent(playerdata.events);
            //lOADCOIN
            coin = playerdata.coin;

        }
    }
    public void LoadHeartData()
    {
        if (PlayerPrefs.HasKey("datequit"))
        {
            DateTime datequit = DateTime.Parse(PlayerPrefs.GetString("datequit"));
            TimeSpan timepassed = DateTime.Now - datequit;//thời gian đã thoát game
            TimeSpan timeheartquit = TimeSpan.FromSeconds(PlayerPrefs.GetFloat("timeheart"));//thoi gian dang dem khi thoat
            timepassed = timepassed + timeheartquit;
            int amoutHeart = (int)(timepassed.TotalMinutes / m_gameSetting.MaxTimeHeart);//luot choi nhan duoc khi thoat
            m_gameSetting.hearts = m_gameSetting.hearts + amoutHeart;
            m_gameSetting.hearts = Mathf.Clamp(m_gameSetting.hearts, 0, m_gameSetting.heartMax);
            timeHeart = (float)(timepassed.TotalSeconds % (m_gameSetting.MaxTimeHeart * 60));
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
