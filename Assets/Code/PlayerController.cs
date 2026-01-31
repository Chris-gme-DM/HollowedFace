using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region UnityEditor
    [SerializeField] private List<MaskSetting> maskSettings;
    #endregion
    #region Setup
    private int _currentMaskIndex;
    private InputSystem_Actions _input;
    private InputActionMap _map;
    #endregion
    #region Initialization
    private void Awake()
    {
        _input = new();
        _map = _input.PointAndClick;

        _map.FindAction("Point").performed += OnPoint;
        _map.FindAction("Move").performed += OnMove;
        _map.FindAction("Interact").canceled += OnInteract;
        _map.FindAction("Mask").performed += OnMask;
        _map.FindAction("Pause").canceled += OnPause;
    }
    private void OnEnable()
    {
        _map.Enable();
    }
    private void OnDisable()
    {
        _map.Disable();
    }
  #endregion
  #region InputHandlers
  private void OnInteract(InputAction.CallbackContext ctx)
    {
        // erkenne das Interactable auf das der Mauszeiger gerichtet ist.
        // Rufe die Interact method des objects auf
        // Das object regelt den rest
    }
    private void OnPoint(InputAction.CallbackContext ctx)
    {
        // read the screenpoint to world position indem du einen raycast auf die mouse position werfen lässt
        // lies den value des mauszeigers aus
        // wenn es ein interactable erkennt soll es den namen des objects auslesen und als kleine box neben der maus anzeigen lassen
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        // lies den input der mouse position undprojektire sie auf den walkway der unter dem player liegt.
        // entwickle hierzu ein kreuzprodukt zwischen dem mouse ray und einem fiktiven ray zu der mask die wir für den walkway brauchen aus
        // bedenke hierbei dass die level ein objekt als walkway brauchen und dass du custom layer hinzufügen kannst die dann mit ihrem entsprechendden type ansprechbar sind.
        // du findest es schon raus chakka
    }
    private void OnMask(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;
        float scrollValue = ctx.ReadValue<Vector2>().y;
        if(scrollValue > 0) _currentMaskIndex = (_currentMaskIndex +1) % maskSettings.Count;
        else if(scrollValue < 0) _currentMaskIndex = (_currentMaskIndex - 1) % maskSettings.Count;
        MaskSetting activeMask = maskSettings[_currentMaskIndex];
        SatelliteDish.MaskChange.Invoke(activeMask);
    }
    private void OnPause(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled) FindAnyObjectByType<GameManager>().TogglePause();
    }
    #endregion
}
#region Serializables
public enum MaskType
{
    None,
    Happy,
    Angry,
    Sad,
    Indifferent
}
[Serializable]
public struct MaskSetting{
    public MaskType type;
    public int energyDrain;
    // Extend upon clearance how mask is shown in UI
    public Sprite sprite;
}
#endregion