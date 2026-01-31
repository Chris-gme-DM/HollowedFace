using UnityEngine;
/// <summary>
/// I will make this obsolete
/// </summary>
public class OptionsUI : MonoBehaviour
{
    private float _masterVol;
    private float _musicVol;
    private float _sfxVol;

    public void OnEndEditMasterVol(float value)
    {
        _masterVol = value;
    }
}