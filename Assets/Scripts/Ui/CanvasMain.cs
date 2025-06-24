using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMain : UICanvas
{
    [SerializeField] Transform content;
    [SerializeField] Button btnSetting;
    LevelController m_levelctr;
    private void Awake()
    {
        m_levelctr = new GameObject("LevelController").AddComponent<LevelController>();
        m_levelctr.Init(content);
    }
    private void Start()
    {
        btnSetting.onClick.AddListener(() =>
        {
            CanvasSetting setting = UIManager.Instance.OpenUI<CanvasSetting>();
            setting.Active();
        });
    }
    public void UpdateLevelUi()
    {
        m_levelctr.UpdateLevel();
    }
}
