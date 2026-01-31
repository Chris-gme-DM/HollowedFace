using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour
{
  [SerializeField] private string objectName;
  [SerializeField] private MaskType requiredMask;
  [SerializeField] private InteractionType interactionType;
  [SerializeField, TextArea] private string dialogueText;
  public InteractionType Type => interactionType;

  public MaskType MaskType => requiredMask;

  
  public virtual void OnInteract(InteractionType interactionType)
  {
    switch (interactionType)
    {
      case InteractionType.None:
      // Play the non interact sound
        break;
      case InteractionType.Action:
        //Play animation
        break;
      case InteractionType.Dialogue:
        break;
      case InteractionType.Item:
        // Whatever the Item does
        break;
    }
    if (interactionType != InteractionType.None) SatelliteDish.Interaction.Invoke(interactionType);
    OnObjectInteraction();
  }
  public abstract void OnObjectInteraction();
}
