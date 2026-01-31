using System;
using UnityEngine;
using UnityEngine.Events;

/// <remarks>
///   <para>
///     Author: Maria Lindling / <a href="mailto:maria.lindling@protonmail.com">maria.lindling@protonmail.com</a>
///   </para>
///   <para>
///     This class' events should only be listened to by other persistent objects.
///   </para>
///   <para>
///     This class should maintain a strictly unique instance.
///   </para>
/// </remarks>
/// <summary>
///   Host class for our custom game events, which may be invoked from this class' static properties of the same names.
/// </summary>
public class SatelliteDish : MonoBehaviour
{
#region Singleton
  private static SatelliteDish _instance ;

  public GameStatus CurrentGameStatus {get; private set;}
#endregion


#region Unity Editor
  [SerializeField] private EventSystemConfig config ;
  [SerializeField] private SceneContextEvents sceneContextEvents ;
  [SerializeField] private GameContextEvents gameContextEvents;
#endregion


#region Invokables: StatisticsTracking
  /// <remarks>
  ///   <para>
  ///     Author: Maria Lindling / <a href="mailto:maria.lindling@protonmail.com">maria.lindling@protonmail.com</a>
  ///   </para>
  ///   <para>
  ///     Type parameters SceneStatus,SceneStatus represent 'prev' and 'next' respectively.
  ///   </para>
  ///   <para>
  ///     Listeners are assigned in the Unity Editor. This should only ever be invoked.
  ///   </para>
  /// </remarks>
  /// <summary>
  ///   Invokable UnityEvent to broadcast changes in the scene's status across the game.
  /// </summary>
  public static UnityEvent<SceneStatus,SceneStatus> SceneStatusChange => _instance.sceneContextEvents.sceneStatusChange ;
#endregion

#region Invokables: GameEvents
  public static UnityEvent<GameStatus> GameStatusChange => _instance.gameContextEvents.gameStatusChange ;
  public static UnityEvent Interaction => _instance.gameContextEvents.interaction;
  public static UnityEvent<int> EnergyEffect => _instance.gameContextEvents.energyEffect;
  public static UnityEvent<MaskSetting> MaskChange => _instance.gameContextEvents.maskChange;
  public static UnityEvent<int> TimePass => _instance.gameContextEvents.timePassed;
#endregion
#region MonoBehavior
private void Awake()
{
  // generally validate that there is only ever one SatelliteDish instance
  // stop everything and throw an exception if not
  if( _instance != null )
    throw new Exception("Program attempted to create an instance of SatelliteDish, but one already existed.") ;
  
  _instance = this ;

  DontDestroyOnLoad( this ) ;
}
#endregion


#region Serializables
[Serializable]
private class SceneContextEvents {
[SerializeField] public UnityEvent<SceneStatus,SceneStatus> sceneStatusChange ;
}
[Serializable]
private class GameContextEvents
  {
    [SerializeField] public UnityEvent<GameStatus> gameStatusChange = new() ;
    [SerializeField] public UnityEvent interaction = new() ;
    [SerializeField] public UnityEvent<int> energyEffect = new() ;
    [SerializeField] public UnityEvent<int> timePassed = new() ;
    [SerializeField] public UnityEvent<MaskSetting> maskChange = new() ;
  }

[Serializable]
private class EventSystemConfig {
[SerializeField] public string helloWorld = "hello world" ;
}

}
#endregion