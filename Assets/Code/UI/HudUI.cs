using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _energyBar;
    [SerializeField] private Image _maskImage;
    [SerializeField] private TMP_Text _clockDisplay;

  private void OnEnable()
  {
    SatelliteDish.TimePass.AddListener(HandleTimePass);
    SatelliteDish.EnergyEffect.AddListener(HandleEnergyEffect);
    SatelliteDish.MaskChange.AddListener(HandleMaskChange);
  }
  private void OnDisable()
  {
    SatelliteDish.TimePass.RemoveListener(HandleTimePass);
    SatelliteDish.EnergyEffect.RemoveListener(HandleEnergyEffect);
    SatelliteDish.MaskChange.RemoveListener(HandleMaskChange);
  }
void Update()
  {
    _energyBar.fillAmount = GameManager._instance.CurrentEnergy;
  }
  private void HandleMaskChange(MaskSetting mask)
  {
    _maskImage.sprite = mask.sprite;
  }

  private void HandleEnergyEffect(int arg0)
  {
    _energyBar.fillAmount = arg0/100;
  }

  private void HandleTimePass(int arg0)
  {
    _clockDisplay.text = $"{arg0 / 60:D2}:{arg0 % 60:D2}";
  }
}
