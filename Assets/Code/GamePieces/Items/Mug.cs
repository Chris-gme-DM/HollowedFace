public class Mug : BaseInteractable
{
  public bool canInteract = false;
  public override void OnObjectInteraction()
  {
    if(canInteract) SatelliteDish.RequestEnergyAdjustment.Invoke(10);
    canInteract = false;
  }
}