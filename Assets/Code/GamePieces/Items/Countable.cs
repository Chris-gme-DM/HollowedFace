public class Countable : BaseInteractable
{
  public override void OnObjectInteraction(){Destroy(this);} 
}