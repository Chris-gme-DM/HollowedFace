using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.PlaySound(Soundtype.Maske_traurig);
    }
}
