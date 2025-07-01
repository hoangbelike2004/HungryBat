using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAudioType
{
    OPEN_CLIP,
    SETTING_CLIP,
    CLOSE_CLIP,
    POPUP_DEACTIVE_CLIP,
    POPUP_ACTIVE_CLIP,
    ITEM_NORMAL_DESTROY,
    SELECT_BOUNUS_ITEM,
    NOT_SELECT_BONUS_ITEM,
    MUCSIC_MAIN_MENU,
    MUCSIC_GAME_PLAY,
    COMPLETE_WIN,
    COMPLETE_NOT_WIN,
    STAR,
    ITEM_BOUNUS_LIGHTNING,
    ITEM_BOUNUS_BOMB,
    ITEM_BOUNUS_POTION,
    ITEM_BOUNUS_DIRECTION,
    
}
public class SoundAsset : Singleton<SoundAsset>
{
    public List<SoundAudioClip> clips;
}
[System.Serializable]
public struct SoundAudioClip
{
    public AudioClip AudioClip;
    public eAudioType AudioType;
}
