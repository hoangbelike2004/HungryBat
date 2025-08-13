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
    public int IndexTutorial = 6;
    private eStateGame stateGame;
    private LevelData _levelData;
    private BoardController m_boarcontroll;
    private CanvasGamePlay m_canvasGamePlay;
    private CanvasMain m_canvasMain;
    private SelectLevelUI m_selectLevelUI;
    private CanvasOutofLives m_canvasOutofLives;
    private CanvasTutorial m_CanvasTutorial;
    private int star, numbermove, sumItemAmout, score;
    private float completionrate, timeHeart;
    private bool isCoroutineRunning,isTutorial;
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
        m_gamelevel.Check();
        m_gameSupportBonus.Check();
        m_gameSetting.Check();
        m_gameevent.Check();
    }
    private void Start()
    {
        isCoroutineRunning = false;
        LoadDataPlayer();
        LoadHeartData();
        m_canvasMain = UIManager.Instance.OpenUI<CanvasMain>();
        m_canvasMain.SetGameSupportBonus(m_gameSupportBonus);
        m_canvasMain.SetGameEvent(m_gameevent);
        m_canvasMain.SetGameLevel(m_gamelevel);
        m_canvasMain.SetState(eStateMain.HOME);
        m_canvasMain.UpdateCoin(coin);
        SoundManager.Instance.PlaySound(eAudioType.MUCSIC_MAIN_MENU);
        if (PlayerPrefs.HasKey(Constants.KEY_TUTORIAL))
        {
            IndexTutorial = PlayerPrefs.GetInt(Constants.KEY_TUTORIAL);
        }
        if (IndexTutorial == 6)
        {
            m_CanvasTutorial = UIManager.Instance.OpenUI<CanvasTutorial>();
            m_canvasMain.SetCanvasLevel1(4);
            m_canvasMain.HandTutorial(true);
            for(int i = 0;i < m_gameSupportBonus.bonusDatas.Count;i++)
            {
                m_gameSupportBonus.bonusDatas[i].state = eStateBonusItem.UNSELECTED;    
            }
        }
        StartCoroutine(CheckTimeQuit());
    }
    public void FixedUpdate()
    {
        if (m_gameSetting.hearts < m_gameSetting.heartMax)
        {
            //if (!isCoroutineRunning)
            //{
            //    autoSaveHeart = StartCoroutine(AutoSaveHeart(1f));
            //}
            timeHeart += Time.fixedDeltaTime;
            TimeSpan timespan = TimeSpan.FromSeconds(timeHeart);
            string strTime = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
            if (m_canvasOutofLives != null)
            {
                m_canvasOutofLives.UpdateUI(m_gameSetting.hearts, strTime);
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
            //if (autoSaveHeart != null)
            //{
            //    StopCoroutine(autoSaveHeart);
            //}
            //isCoroutineRunning = false;
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
        if (isTutorial) return;
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
    public void SetDefaultCanvasLevel()
    {
        m_canvasMain.SetCanvasLevel1(0);
    }
    public void GameStart()
    {
        SoundManager.Instance.PlaySound(eAudioType.MUCSIC_GAME_PLAY);
        m_canvasGamePlay = UIManager.Instance.OpenUI<CanvasGamePlay>();
        m_canvasGamePlay.SetLevelData(_levelData);
        m_canvasGamePlay.UpdateUISupportBonus();
        Utils.SetNormals(_levelData.normalItemtype);
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
            bonusdata = null;
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
                    int tmp = _levelData.coin;
                    coin += _levelData.coin;
                    cointmp = _levelData.coin;
                    _levelData.coin = 0;
                    m_canvasMain.UpdateCoin(coin);
                    _levelData.coin = tmp;
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
            if (_levelData.normalItemtype[i] == type)
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
        m_canvasMain.UpdateCoin(this.coin);
    }
    public int GetCoin()
    {
        return this.coin;
    }
    public void SetHearts(int hearts)
    {
        m_gameSetting.hearts -= hearts;
    }
    public int GetHeart()
    {
        return m_gameSetting.hearts;
    }
    public int GetIndexTutotial()
    {
        return IndexTutorial;
    }
    public void SetIsTutorial(bool isTutorial)
    {
        this.isTutorial = isTutorial;
    }
    public void SetIndexTutotial()
    {
        IndexTutorial -= 1;
        if(IndexTutorial == 5)
        {
            m_canvasMain.HandTutorial(false);
        }else if(IndexTutorial == 4)
        {
            m_selectLevelUI.HandTutorial(false,false);
        }else if(IndexTutorial == 3)
        {
            m_canvasGamePlay.HandTutorial(true);
        }
        else if(IndexTutorial == 2)
        {
            m_canvasGamePlay.HandTutorial(false);
            m_boarcontroll.ActiveTutorial();
        }
        else if(IndexTutorial == 1)
        {
            m_boarcontroll.DeactiveTutorial();
            UIManager.Instance.CloseUI<CanvasTutorial>(0f);
            PlayerPrefs.SetInt(Constants.KEY_TUTORIAL, IndexTutorial);
        }
    }
    //public void ActiveTutorialGameplay()
    //{
    //    if (GameController.Instance.GetIndexTutotial() == 3)
    //    {
    //        m_canvasGamePlay.HandTutorial(true);
    //    }
    //}

    public void SetCanvasTutorial(string name)
    {
        m_CanvasTutorial.Canvas.sortingLayerName = name;
    }
    public int GetCurrentProgess()
    {
        return currentProgess;
    }
    public void SetCanvasOutOfLives(CanvasOutofLives canvas)
    {
        m_canvasOutofLives = canvas;
    }

    //SAVE AND LOAD DATALOCAL
    public void SaveHeart()
    {
        if (m_gameSetting.hearts < m_gameSetting.heartMax)
        {
            DateTime date = DateTime.Now;
            PlayerPrefs.SetString("datequit", date.ToString());
            PlayerPrefs.SetFloat("timeheart", timeHeart);//thoi gian dang dem khi thoat game
        }

    }
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
        DataUtils.SaveData(m_gameSetting.hearts, coin, level, currentProgess, m_gameevent.events
            , m_gameSupportBonus.bonusDatas, m_gameSetting.volumeMusic, m_gameSetting.volumeSound);
        SaveTimeQuit();

    }

    IEnumerator CheckTimeQuit()
    {
        WaitForSeconds waitFor = new WaitForSeconds(60);
        while (true)
        {
            yield return waitFor;
            SaveTimeQuit();
            LoadDataEvent(currentProgess);
        }
    }
    public void SaveTimeQuit()
    {
        DateTime date = DateTime.Now;
        PlayerPrefs.SetString(Constants.KEY_LATE_LOGIN_DATE, date.ToString("yyyy-MM-dd"));
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
            m_gameSupportBonus.LoadDataItem(playerdata.Itembonuses);
            //loadsetting
            m_gameSetting.LoadDataSetting(playerdata.hearts, playerdata.VolumeMusic, playerdata.volumeSound);
            //loadevent
            LoadDataEvent(playerdata.currentProgess);
            m_gameevent.LoadDataEvent(playerdata.events);
            //lOADCOIN
            coin = playerdata.coin;
        }
        else
        {
            m_gameSetting.hearts = m_gameSetting.heartMax;
        }
    }
    public void LoadDataEvent(int currentProgess)
    {
        if (PlayerPrefs.HasKey(Constants.KEY_LATE_LOGIN_DATE))
        {
            string strDate = PlayerPrefs.GetString(Constants.KEY_LATE_LOGIN_DATE);
            DateTime latedate = DateTime.Parse(strDate);
            DateTime today = DateTime.Now.Date;
            if (today > latedate)
            {
                currentProgess = 0;
            }
            else
            {
                this.currentProgess = currentProgess;
            }
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
    public void SetSelectLevelUI(SelectLevelUI selectLevelUI)
    {
        this.m_selectLevelUI = selectLevelUI;
    }
    private void OnApplicationQuit()
    {
        SaveHeart();
        SaveDataPlayer();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveHeart();
            SaveDataPlayer();
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
