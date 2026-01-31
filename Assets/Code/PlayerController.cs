using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region UnityEditor
    [SerializeField] private List<MaskSetting> maskSettings;
    [SerializeField] private float walkSpeed = 5f;      //Base MS
    #endregion
    #region Setup
    private int _currentMaskIndex;
    private InputSystem_Actions _input;
    private InputActionMap _map;
    private BaseInteractable _interactable;
    private Vector2 mousePosition;
    private Ray _currentRay;
    private int _walkableLayer;
    private Vector3 _targetPosition;
    private bool _isWalking;
    private Coroutine _interactionCoroutine;
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

        _walkableLayer = LayerMask.GetMask("Walkable");
        _targetPosition = transform.position;
    }
    private void OnEnable()
    {
        _map.Enable();
    }
    private void OnDisable()
    {
        _map.Disable();
    }
  private void Update()
  {
    if (!_isWalking) return;
    transform.position = Vector3.MoveTowards(
        transform.position,
        _targetPosition,
        walkSpeed * Time.deltaTime
    );
    if (Vector3.Distance(transform.position, _targetPosition) < 0.01f) _isWalking = false;
  }
  #endregion
  #region Helpers
  private void MovePlayer()
    {
        float t = -_currentRay.origin.z / _currentRay.direction.z;
        Vector3 intersectionPoint = _currentRay.origin + (_currentRay.direction * t);
        Collider[] colliders = Physics.OverlapSphere(intersectionPoint, 2f, _walkableLayer);
        if( colliders.Length > 0)
        {
            Vector3 rawPos = colliders[0].ClosestPoint(intersectionPoint);
            _targetPosition = new(rawPos.x, rawPos.y + 1f, rawPos.z);
            _isWalking = true;
        }
    }
    private IEnumerator WalkAndInteract(BaseInteractable target)
    {
        MovePlayer();
        while( Vector3.Distance(transform.position, _targetPosition) > 0.1f) yield return null;
        _interactable.OnInteract();
        _interactionCoroutine = null;
    }
  #endregion
  #region InputHandlers
  private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_interactionCoroutine != null) StopCoroutine(_interactionCoroutine);
        if (_interactable != null) StartCoroutine(WalkAndInteract(_interactable));
    }
    private void OnPoint(InputAction.CallbackContext ctx)
    {
        mousePosition = ctx.ReadValue<Vector2>();
        // read the screenpoint to world position indem du einen raycast auf die mouse position werfen lässt
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        _currentRay = ray;
        RaycastHit hit;
        // lies den value des mauszeigers aus
        if (Physics.Raycast(ray, out hit))
        {
            BaseInteractable objectHit = hit.collider.GetComponent<BaseInteractable>();
            _interactable = objectHit;
            Debug.Log($"{objectHit}");
        } else _interactable = null;

    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MovePlayer();
    }
    private void OnMask(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;
        float scrollValue = ctx.ReadValue<Vector2>().y;
        if(scrollValue > 0) _currentMaskIndex = (_currentMaskIndex +1) % maskSettings.Count;
        else if(scrollValue < 0) _currentMaskIndex = (_currentMaskIndex - 1 + maskSettings.Count) % maskSettings.Count;
        MaskSetting activeMask = maskSettings[_currentMaskIndex];
        Debug.Log($"Mask changed to {activeMask.type}");
        // Update HeadRotation
        SatelliteDish.MaskChange.Invoke(activeMask);
    }
    private void OnPause(InputAction.CallbackContext ctx)
    {
        FindAnyObjectByType<GameManager>().TogglePause();
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