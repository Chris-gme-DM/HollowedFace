using UnityEngine;

public class Cat : BaseInteractable
{    
    private Animator _animator => GetComponentInChildren<Animator>();
  public override void OnObjectInteraction()
  {
    if (_animator != null) _animator.SetTrigger("Interact");
    GameObject player = GameObject.FindWithTag("Player"); 
    if (player != null)
    {
        Animator playerAnim = player.GetComponentInChildren<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetTrigger("PetCat");
        }
    }
  }
}