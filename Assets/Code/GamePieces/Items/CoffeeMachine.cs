using UnityEngine;

public class CoffeeMachine : BaseInteractable
{
  public override void OnObjectInteraction()
  {
    SatelliteDish.RequestEnergyAdjustment.Invoke(10);
    
  }
}