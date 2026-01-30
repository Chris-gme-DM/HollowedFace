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
#endregion


#region Unity Editor
  [SerializeField] private EventSystemConfig config ;
  [SerializeField] private SceneContextEvents sceneContextEvents ;
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
private class EventSystemConfig {
[SerializeField] public string helloWorld = "hello world" ;
}
#endregion
}
