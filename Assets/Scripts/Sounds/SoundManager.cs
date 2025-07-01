using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] GameSetting gamesetting;
    private Sound audioMusic;
    public void SetGameSetting(GameSetting gamesetting)
    {
        this.gamesetting = gamesetting;
    }
    public void PlaySound(eAudioType type)
    {
        if(type == eAudioType.MUCSIC_MAIN_MENU || type == eAudioType.MUCSIC_GAME_PLAY)
        {
            if(audioMusic != null)
            {
                SimplePool.Despawn(audioMusic);
            }
            audioMusic = SimplePool.Spawn<Sound>(PoolType.POOL_MUSIC, Vector3.zero, Quaternion.identity);
            audioMusic.audioSource.clip = GetAudioClip(type);
            audioMusic.audioSource.volume = gamesetting.volumeMusic;
            audioMusic.audioSource.Play();
        }
        else
        {
            Sound audiosource = SimplePool.Spawn<Sound>(PoolType.POOL_SOUND, Vector3.zero, Quaternion.identity);
            audiosource.audioSource.clip = GetAudioClip(type);
            audiosource.audioSource.volume = gamesetting.volumeSound;
            audiosource.audioSource.Play();
            StartCoroutine(StopAudioClip(audiosource.audioSource.clip.length + 0.1f, audiosource));
        }
    }
    public void PlaySoundOnDelay(float time,eAudioType type)
    {
        StartCoroutine(PlaySoundCouroutine(time,type));
    }
    IEnumerator PlaySoundCouroutine(float time,eAudioType type)
    {
        yield return new WaitForSeconds(time);
        PlaySound(type);
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
    public void SetVolumeMusic(float volume)
    {
        audioMusic.audioSource.volume = volume;
    }

    IEnumerator StopAudioClip(float time,GameUnit audiosource)
    {
        yield return new WaitForSeconds(time);
        SimplePool.Despawn(audiosource);
    }
}
