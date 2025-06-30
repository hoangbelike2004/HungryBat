using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] GameSetting gamesetting;
    public void SetGameSetting(GameSetting gamesetting)
    {
        this.gamesetting = gamesetting;
    }
    public void PlaySound(eAudioType type)
    {
        Sound audiosource = SimplePool.Spawn<Sound>(PoolType.POOL_SOUND,Vector3.zero,Quaternion.identity);
        if(type == eAudioType.BACKGROUNDMUCSIC)
        {
            audiosource.audioSource.clip = GetAudioClip(type);
            audiosource.audioSource.volume = gamesetting.volumeMusic;
        }
        else
        {
            audiosource.audioSource.clip = GetAudioClip(type);
            audiosource.audioSource.volume = gamesetting.volumeSound;
        }
        audiosource.audioSource.Play();
        if(type != eAudioType.BACKGROUNDMUCSIC)
        {
            StartCoroutine(StopAudioClip(audiosource.audioSource.clip.length+0.1f, audiosource));
        }
    }

    public AudioClip GetAudioClip(eAudioType audioType)
    {
        for(int i = 0;i < SoundAsset.Instance.clips.Count;i++)
        {
            if (SoundAsset.Instance.clips[i].AudioType == audioType)
            {
                return SoundAsset.Instance.clips[i].AudioClip;
            }
        }
        return null;
    }

    IEnumerator StopAudioClip(float time,GameUnit audiosource)
    {
        yield return new WaitForSeconds(time);
        SimplePool.Despawn(audiosource);
    }
}
