using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
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
		private TMP_Text nameDisplay;
		private TMP_Text dialogueText;
		private Vector2 nameTagOffset = new(0,30);
  #endregion

  #region MonoBehaviour
  void Awake()
  {
    if ( _instance != null && _instance != this )
    {
        if( _instance != this) Destroy(this);
        _instance = this;
        DontDestroyOnLoad(this);
    }
  }
  private void Update()
  {
    if(nameDisplay.gameObject.activeSelf) nameDisplay.transform.position = Input.mousePosition + (Vector3)nameTagOffset;
		else nameDisplay.gameObject.SetActive(false);
  }
  #region Subscriptions
  private void OnEnable()
  {
    SatelliteDish.GameStatusChange.AddListener(HandleStatusChange);
    SatelliteDish.SceneStatusChange.AddListener(HandleSceneStatusChange);
    SatelliteDish.Interaction.AddListener(HandleInteraction);
  }
  private void OnDisable()
  {
    SatelliteDish.GameStatusChange.RemoveListener(HandleStatusChange);
    SatelliteDish.SceneStatusChange.RemoveListener(HandleSceneStatusChange);
    SatelliteDish.Interaction.RemoveListener(HandleInteraction);
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
			nameDisplay.text = data.Name;
			if(data.Type != InteractionType.Dialogue) return;
			// if the interaction validates showing the Dialogue
			dialogueText.text = data.Dialogue;
			SetUIStatus(PanelType.Dialogue);
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