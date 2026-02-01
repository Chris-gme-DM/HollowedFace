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
    [SerializeField] private GameObject _maskHead;
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
    private Animator _animator;
    private Quaternion _baseRotation = Quaternion.Euler(0, 180, 0);
    private int _interactionLayer => LayerMask.GetMask("Interactable");
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
        _animator = GetComponentInChildren<Animator>();
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
    if (!_isWalking)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _baseRotation, Time.deltaTime * 10f);
        return;
    }
    transform.position = Vector3.MoveTowards(
        transform.position,
        _targetPosition,
        walkSpeed * Time.deltaTime
    );
    if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
    {
        _isWalking = false;
        _animator.SetBool("isWalking", false);
    } 
    else _animator.SetBool("isWalking", true);
    // Rotation
    Vector3 direction = (_targetPosition - transform.position).normalized;
    if (direction != Vector3.zero)
    {
        direction.y = 0; 
        Quaternion targetRotation = Quaternion.LookRotation(-direction);    // Offset for reasons
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
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
            _targetPosition =  colliders[0].ClosestPoint(intersectionPoint);
            _isWalking = true;
        }
    }
    private IEnumerator WalkAndInteract(BaseInteractable target)
    {
        MovePlayer();
        while( Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            if(!_isWalking) yield break;
            yield return null;
        }
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
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _interactionLayer))
        {
            BaseInteractable objectHit = hit.collider.GetComponent<BaseInteractable>();
            _interactable = objectHit;
        } else _interactable = null;
        Debug.Log($"Targeting: {(_interactable != null ? _interactable.name : "None")}");
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MovePlayer();
        Debug.Log($"MOVE");
    }
    private void OnMask(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;
        float scrollValue = ctx.ReadValue<Vector2>().y;
        if(scrollValue > 0) _currentMaskIndex = (_currentMaskIndex +1) % maskSettings.Count;
        else if(scrollValue < 0) _currentMaskIndex = (_currentMaskIndex - 1 + maskSettings.Count) % maskSettings.Count;
        MaskSetting activeMask = maskSettings[_currentMaskIndex];
        // Rotate head
        float targetY = 0;
        switch (activeMask.type)
        {
            case MaskType.Happy: targetY = 90f; break;
            case MaskType.Angry: targetY = 180f; break;
            case MaskType.Sad: targetY = 270f; break;
            case MaskType.Indifferent: targetY = 0f; break; 
            default: targetY = 0f; break;
        }
        Transform headTransform = _maskHead.transform;
        headTransform.localRotation = Quaternion.Euler(0, targetY, 0);

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