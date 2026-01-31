using System;
using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour
{
  [SerializeField] private string objectName;
  [SerializeField] protected MaskType requiredMask;
  [SerializeField] protected InteractionType interactionType;
  [SerializeField, TextArea] private string dialogueText;

  public void OnInteract()
  {
    InteractableData data = new()
    {
      Name = objectName,
      Dialogue = dialogueText,
      Type = interactionType,
      Mask = requiredMask
    };
    OnInteraction(data);
  }
  public virtual void OnInteraction(InteractableData data)
  {
    switch (data.Type)
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
    SatelliteDish.Interaction.Invoke(data); 
    OnObjectInteraction();
  }
  public abstract void OnObjectInteraction();
} 
[Serializable]
public struct InteractableData
{
  public string Name;
  public string Dialogue;
  public InteractionType Type;
  public MaskType Mask;
}
