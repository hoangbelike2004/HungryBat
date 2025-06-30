using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAudioType
{
    BACKGROUNDMUCSIC,
    OPEN_CLIP,
    SETTING_CLIP,
    CLOSE_CLIP,
    POPUP_DEACTIVE_CLIP,
    POPUP_ACTIVE_CLIP,
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
