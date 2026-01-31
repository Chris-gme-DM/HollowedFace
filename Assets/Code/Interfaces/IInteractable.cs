/// <summary>
/// Use this for interactable objects to define their params. We can use MaskType and Interaction type to define default fallbacks, and configure interactions dynamically
/// Do not forget to create a base Class that implements the interface.s
/// </summary>
public interface IInteractable
{
    InteractionType Type {get;}
    MaskType MaskType{get;}
    void Interact();
}