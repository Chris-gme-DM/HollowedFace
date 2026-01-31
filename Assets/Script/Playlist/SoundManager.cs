using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Soundtype
{
  Maske_wuetend,
  Maske_traurig,
  Maske_lachen,
  Maske_gleichgueltig,
  Hintergrund01,
  Hintergrund02
}


[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private Soundlist[] soundlist;
    private static SoundManager instance;
    private AudioSource audioSource;
    private float _masterVolume;
    private float _musicVolume;
    private float _sfxVolume;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(Soundtype sound, float volume = 1 )
    {
       // instance.audioSource.PlayOneShot(instance.soundlist[(int)sound], volume);
    }

#if Unity_Editor
    private void OnEnable()
    {
        string[] names = enum.GetNames(typeof(SoundType));
        Array.Resize(soundList, names.length);
        for(int i = 0; i < soundList.length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif

    public void SetMasterVolume(float volume) => _masterVolume = volume;
    
    public void SetMusicVolume(float volume) => _musicVolume = volume;
    public void SetSoundVolume(float volume) => _sfxVolume = volume;
}

[Serializable]
public struct Soundlist 
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
