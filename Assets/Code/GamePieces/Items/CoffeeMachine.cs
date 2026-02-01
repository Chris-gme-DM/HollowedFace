public class CoffeeMachine : BaseInteractable
{
  public override void OnObjectInteraction()
  {
    Mug mug = FindFirstObjectByType<Mug>();
    mug.canInteract = true;
  }
}