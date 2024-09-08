using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Low level audio interface
/// </summary>
public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    public AudioDataList audioDataList; 

    public AudioSource sfxSrc;
    public AudioSource musicSrc;

    protected void Awake()
    {
        Debug.Log($"Awake AudioController");
        Instance = Instance != null ? Instance : this;
    }

    public static bool DoesInstanceExist()
    {
        return Instance;
    }

    /// <summary>
    /// Check if the Audio is playing.
    /// </summary>
    public static bool IsPlaying(string id)
    {
        return false;
    }

    /// <summary>
    /// Check if the Event is callable or not.
    /// </summary>
    public static bool IsValidAudioID(string id)
    {
        return Instance.audioDataList.list.Any(x => x.id == id);
    }


    /// <summary>
    /// Mutes all sounds in game
    /// </summary>
    public static void PauseSound(bool state)
    {
        Debug.Log($"<color=yellow> PauseSound::{state}</color>");
        //Instance.fmodPlayer.PauseSound(state);
    }

    public static void MuteSound(bool state)
    {
        //Instance.fmodPlayer.MuteSound(state);
        Instance.sfxSrc.mute = state;
        Instance.musicSrc.mute = state;
    }

    /// <summary>
    /// Plays an sfx.
    /// </summary>
    public static void PlaySfx(string id)
    {
        Instance.sfxSrc.PlayOneShot(Instance.audioDataList.list.First(x => string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase)).clip);
    }

    /// <summary>
    /// Plays an music.
    /// </summary>
    public static void PlayMusic(string id)
    {
        if (Instance.musicSrc.isPlaying)
        {
            Instance.musicSrc.Stop();
        }

        Instance.musicSrc.clip = Instance.audioDataList.list.First(x => string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase)).clip;
        Instance.musicSrc.Play();
    }

    /// <summary>
    /// Stops playing an sfx.
    /// </summary>
    public static void StopSfx()
    {
        Instance.sfxSrc.Stop();
    }

    /// <summary>
    /// Stops playing an music.
    /// </summary>
    public static void StopMusic()
    {
        Instance.musicSrc.Stop();
        Instance.musicSrc.clip = null;
    }

    /// <summary>
    /// Stops all audio
    /// </summary>
    public static void StopAll()
    {
        StopSfx();
        StopMusic();
    }
}