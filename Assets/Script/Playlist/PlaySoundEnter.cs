using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private Soundtype sound;
    [SerializeField, Range(0, 1)] private float volume = 1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound, volume);
    }
}
