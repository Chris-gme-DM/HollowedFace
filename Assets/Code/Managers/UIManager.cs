using System.Collections.Generic;
using UnityEngine;
using System;
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
  public void TogglePanel(PanelType type)
    {
        foreach ( var p in panels )
        {
            if(p.Type == type)
            {
                bool isActive = p.PanelObject.activeSelf;
                p.PanelObject.SetActive(!isActive);
            }
            // Will probably extend functions here upon loading.
            // Cerate an Enumerator for the Loading screen or Make a separate function
        }
    }
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