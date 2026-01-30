using UnityEngine;

public enum Soundtype
{
  Maske_wuetend,
  Maske_traurig,
  Maske_lachen,
  Maske_gleichgueltig,
  Hintergrund01,
  Hintergrund02
}


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundlist;
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
        instance.audioSource.PlayOneShot(instance.soundlist[(int)sound], volume);
    }

    public void SetMasterVolume(float volume) => _masterVolume = volume;
    
    public void SetMusicVolume(float volume) => _musicVolume = volume;
    public void SetSoundVolume(float volume) => _sfxVolume = volume;
}
