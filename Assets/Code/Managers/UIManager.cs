using System.Collections;
using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
/// <remarks>
///     <para>
///         Author: Christof Kloninger <a href = "mailto: gme.24.kloninger@gmail.com>
///     </para>
///     <para>
///         This is our glorified lightswitch operator. It listens to events and toggles the active panels
///     </para>
/// </remarks>
/// <summary>
/// 
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    #region Unity Editor
    [SerializeField] private List<PanelData> panels;
		[SerializeField] private TMP_Text nameDisplay;
		[SerializeField] private TMP_Text dialogueText;
		private Vector2 nameTagOffset = new(0,30);
  #endregion

  #region MonoBehaviour
  void Awake()
  {
		if (_instance != null && _instance != this)
    {
        Destroy(gameObject); // Kill the new one
        return; // Stop execution here!
    }
    _instance = this;
    DontDestroyOnLoad(gameObject);
	}
  #region Subscriptions
  private void Start()
  {
    SatelliteDish.GameStatusChange.AddListener(HandleStatusChange);
    SatelliteDish.SceneStatusChange.AddListener(HandleSceneStatusChange);
    SatelliteDish.Interaction.AddListener(HandleInteraction);
		SatelliteDish.PointerMoved.AddListener(HandlePointerMove);
  }
  private void OnDisable()
  {
    SatelliteDish.GameStatusChange.RemoveListener(HandleStatusChange);
    SatelliteDish.SceneStatusChange.RemoveListener(HandleSceneStatusChange);
    SatelliteDish.Interaction.RemoveListener(HandleInteraction);
		SatelliteDish.PointerMoved.AddListener(HandlePointerMove);
  }
	#endregion
	#region Handlers
  private void HandleStatusChange(GameStatus status)
    {
			switch ( status )
			{
				case GameStatus.Gameplay:
					SetUIStatus(PanelType.HUD);
					break;
				case GameStatus.Paused:
					SetUIStatus(PanelType.Menu);
					break;
				case GameStatus.GameOver:
					SetUIStatus(PanelType.None);
					break;
			}
    }
  private void HandleSceneStatusChange(SceneStatus prev, SceneStatus next)
	{
		if ( prev == next) return;

		switch ( next )
		{
			case SceneStatus.Invalid:
				SetUIStatus(PanelType.None);
				break;
			case SceneStatus.Loading:
				SetUIStatus(PanelType.Loading);
				break;
			case SceneStatus.Running:
				HandleStatusChange(GameStatus.Gameplay);
				break;
		}
	}
    /// <summary>
    /// watch this Method for changes in interaction system
    /// </summary>
  private void HandleInteraction(InteractableData data)
	{
		if (data.Type == InteractionType.None || string.IsNullOrEmpty(data.Name))
		{
			nameDisplay.text = "";
			SetUIStatus(PanelType.HUD); 
			return;
		}
		nameDisplay.text = data.Name;
		if (string.IsNullOrEmpty(data.Dialogue) || data.Dialogue.Length <= 1)
		{
			SetUIStatus(PanelType.HUD); 
			return;
		}

		dialogueText.text = data.Dialogue;
		SetUIStatus(PanelType.Dialogue, PanelType.HUD);    
	}
	private void HandlePointerMove(Vector2 pointer)
	{
	  if(nameDisplay.gameObject.activeSelf) nameDisplay.gameObject.transform.position = pointer + nameTagOffset;
		else nameDisplay.gameObject.SetActive(false);
	}
		#endregion
		#region Helpers
  public void SetUIStatus(params PanelType[] panelToShow)
    {
			foreach ( var p in panels )
			{
				bool match = false;
				foreach (var t in panelToShow)
				{
					if(p.Type == t)
					{
						match = true;
						break;
					}
				}
				p.PanelObject.SetActive(match);
				// Will probably extend functions here upon loading.
				// Cerate an Enumerator for the Loading screen or Make a separate function
			}
    }
	public void TriggerLoadingScreen()
	{
		StartCoroutine(LoadingSequence());
	}
	private IEnumerator LoadingSequence()
	{
		SetUIStatus(PanelType.Loading);
		yield return new WaitForSecondsRealtime(5f);
		SatelliteDish.SceneStatusChange.Invoke(SceneStatus.Loading, SceneStatus.Running);
	}
		#endregion
    #endregion
    #region Serializables
    public enum PanelType
    {
        None,
        HUD,
        Menu,
        Options,
        Dialogue,
        Loading
    }
    [Serializable]
    public struct PanelData
    {
        public PanelType Type;
        public GameObject PanelObject;
    }
    #endregion
    }