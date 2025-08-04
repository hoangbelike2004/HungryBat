using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSetting : UICanvas
{
    [SerializeField] Button btnClose;
    [SerializeField] Image overlay;
    [SerializeField] RectTransform box, QuitGameLevelPopup, QuitGamePopup;
    [SerializeField] Button btnQuit, btnQuitlevel, btnContinue, btnDisagree, btnAgree;
    [SerializeField] GameSetting gameSetting;
    public float musicvalue;

    [Header("MUSIC")]
    [SerializeField] Button btnActiveMusic;
    [SerializeField] Button btnDeactiveMucsic;
    [SerializeField] Slider sliderMusic;

    [Header("SOUND EFFECTS")]
    [SerializeField] Button btnActiveSound;
    [SerializeField] Button btnDeactiveSound;
    [SerializeField] Slider sliderSound;
    void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(eAudioType.CLOSE_CLIP);
            SoundManager.Instance.PlaySound(eAudioType.POPUP_DEACTIVE_CLIP);
            DeActive();
        });
        btnQuit.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
            if (GameController.Instance.StateGame == eStateGame.STARTED)
            {
                Invoke(nameof(ActiveQuitGameLevelPopup), 0.1f);
            }
            else
            {
                Invoke(nameof(ActiveQuitGamePopup), 0.1f);
            }
        });
        btnContinue.onClick.AddListener(() =>
        {
            QuitGameLevelPopup.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
            {
                overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
                {
                    overlay.gameObject.SetActive(false);
                    QuitGameLevelPopup.gameObject.SetActive(false);
                    UIManager.Instance.CloseUI<CanvasSetting>(0f);
                });
            });
        });
        btnDisagree.onClick.AddListener(() =>
        {
            QuitGamePopup.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
            {
                overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
                {
                    overlay.gameObject.SetActive(false);
                    QuitGamePopup.gameObject.SetActive(false);
                    UIManager.Instance.CloseUI<CanvasSetting>(0f);
                });
            });
        });
        btnAgree.onClick.AddListener(DeactiveQuitGamePopup);
        btnQuitlevel.onClick.AddListener(DeactiveQuitGameLevelPopup);
        //MUSIC
        btnActiveMusic.onClick.AddListener(ActiveMusic);
        btnDeactiveMucsic.onClick.AddListener(DeActiveMusic);
        sliderMusic.onValueChanged.AddListener(delegate { SetVolumSilderMusic(); });

        //SOUND EFECT
        btnActiveSound.onClick.AddListener(ActiveSound);
        btnDeactiveSound.onClick.AddListener(DeActiveSound);
        sliderSound.onValueChanged.AddListener(delegate { SetVolumSilderSound(); });
    }
    public void ActiveMusic()
    {
        SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
        btnActiveMusic.gameObject.SetActive(false);
        btnDeactiveMucsic.gameObject.SetActive(true);
        sliderMusic.value = 1;
        gameSetting.volumeMusic = sliderMusic.value;
        SoundManager.Instance.SetVolumeMusic(gameSetting.volumeMusic);
    }
    public void DeActiveMusic()
    {
        SoundManager.Instance.PlaySound(eAudioType.CLOSE_CLIP);
        btnDeactiveMucsic.gameObject.SetActive(false);
        btnActiveMusic.gameObject.SetActive(true);
        sliderMusic.value = 0;
        gameSetting.volumeMusic = sliderMusic.value;
        SoundManager.Instance.SetVolumeMusic(gameSetting.volumeMusic);
    }
    public void SetVolumSilderMusic()
    {
        gameSetting.volumeMusic = sliderMusic.value;
        SoundManager.Instance.SetVolumeMusic(gameSetting.volumeMusic);
        if (sliderMusic.value > 0)
        {
            btnActiveMusic.gameObject.SetActive(false);
            btnDeactiveMucsic.gameObject.SetActive(true);
        }
        else
        {
            btnDeactiveMucsic.gameObject.SetActive(false);
            btnActiveMusic.gameObject.SetActive(true);
        }
    }


    public void ActiveSound()
    {
        btnActiveSound.gameObject.SetActive(false);
        btnDeactiveSound.gameObject.SetActive(true);
        sliderSound.value = 1;
        gameSetting.volumeSound = sliderSound.value;
        SoundManager.Instance.PlaySound(eAudioType.OPEN_CLIP);
    }
    public void DeActiveSound()
    {
        SoundManager.Instance.PlaySound(eAudioType.CLOSE_CLIP);
        btnDeactiveSound.gameObject.SetActive(false);
        btnActiveSound.gameObject.SetActive(true);
        sliderSound.value = 0;
        gameSetting.volumeSound = sliderSound.value;
    }
    public void SetVolumSilderSound()
    {
        gameSetting.volumeSound = sliderSound.value;
        if (sliderSound.value > 0)
        {
            btnActiveSound.gameObject.SetActive(false);
            btnDeactiveSound.gameObject.SetActive(true);
        }
        else
        {
            btnDeactiveSound.gameObject.SetActive(false);
            btnActiveSound.gameObject.SetActive(true);
        }
    }
    public void Active()
    {
        SoundManager.Instance.PlaySound(eAudioType.POPUP_ACTIVE_CLIP);
        overlay.gameObject.SetActive(true);
        overlay.DOFade(0.5f, 0.1f);
        box.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
        sliderMusic.value = gameSetting.volumeMusic;
        sliderSound.value = gameSetting.volumeSound;
        SetVolumSilderMusic();
        SetVolumSilderSound();
    }
    public void DeActive()
    {
        box.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                UIManager.Instance.CloseUI<CanvasSetting>(0f);
            });

        });

    }

    public void ActiveQuitGamePopup()
    {
        box.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            QuitGamePopup.gameObject.SetActive(true);
            QuitGamePopup.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
        });
    }
    public void DeactiveQuitGamePopup()
    {
        QuitGamePopup.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                QuitGamePopup.gameObject.SetActive(false);
                UIManager.Instance.CloseUI<CanvasSetting>(0f);
                Application.Quit();
            });
        });
    }
    public void ActiveQuitGameLevelPopup()
    {
        box.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            QuitGameLevelPopup.gameObject.SetActive(true);
            QuitGameLevelPopup.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBounce);
        });
    }
    public void DeactiveQuitGameLevelPopup()
    {
        QuitGameLevelPopup.DOAnchorPos(new Vector2(0, 2000), 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            overlay.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                QuitGameLevelPopup.gameObject.SetActive(false);
                UIManager.Instance.CloseAll();
                GameController.Instance.SetState(eStateGame.MAIN_MENU);
                GameController.Instance.ChangeState();
                GameController.Instance.SetHearts(1);
            });
        });
    }
}
