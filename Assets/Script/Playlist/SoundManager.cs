using UnityEngine;

public enum Soundtype
{
    
}


[RequireComponent(typeof(Audiosource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private AudioSource audioSource;

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

    }
}
