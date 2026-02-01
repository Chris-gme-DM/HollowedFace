using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls the GameStatus and invokes events as they occur if overarching gameplay rules apply.
/// </summary>
public class GameManager : MonoBehaviour
{
  private static GameManager _instance;
  public GameStatus CurrentStatus {get; private set;}
  public List<LevelData> Levels;
  private LevelData _currentLevelData;
  public LevelData CurrentLevelData => _currentLevelData;
  private int _currentInGameTime;
  [SerializeField] private Scene _gameOverScene;
  [SerializeField] private float _secondsForTimePass;
  private float _timer;
  private int _currentEnergy;
  private InputSystem_Actions _input;
  private MaskSetting _currentMask;
  void Awake()
  {
    if ( _instance != null)
    {
        if (_instance != this) Destroy(gameObject);
        if(_instance == null) _instance = this;
        DontDestroyOnLoad(this);
    }
    _input = new();
  }
  private void Start()
  {
    ResetGame();
    SetGameState(GameStatus.Paused);
    TogglePause();
    SatelliteDish.MaskChange.AddListener(HandleMaskChange);
    SatelliteDish.RequestEnergyAdjustment.AddListener(AdjustEnergy);
    SatelliteDish.SceneStatusChange.AddListener(HandleSceneChange);
    SatelliteDish.Interaction.AddListener(Count);
    SatelliteDish.Interaction.AddListener(CheckLevelChange);
  }
  private void OnDisable()
  {
    SatelliteDish.MaskChange.RemoveListener(HandleMaskChange);
    SatelliteDish.RequestEnergyAdjustment.RemoveListener(AdjustEnergy);
    SatelliteDish.SceneStatusChange.RemoveListener(HandleSceneChange);
    SatelliteDish.Interaction.RemoveListener(Count);
    SatelliteDish.Interaction.RemoveListener(CheckLevelChange);

  }
  void Update()
    {
        if( CurrentStatus == GameStatus.Gameplay)
        {
            _timer += Time.deltaTime;
            if ( _timer >= _secondsForTimePass)
            {
                _timer = 0;
                AdjustEnergy(-_currentMask.energyDrain);
                SatelliteDish.TimePass.Invoke(_currentInGameTime);
            }
        }
    }
  private void SetGameState(GameStatus next)
    {
        if (next == CurrentStatus) return;
        CurrentStatus = next;
        SatelliteDish.GameStatusChange.Invoke(next);
        Debug.Log($"GameState changed to: {next}");
    }
  public void RestartGame()
    {
        ResetGame();
        TogglePause();
    }
    private void ResetGame()
    {
        _currentInGameTime = 480;
        SatelliteDish.TimePass.Invoke(_currentInGameTime);
        AdjustEnergy(80);
        ChangeScene(1);
    }
    private void ChangeScene(int index)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == _currentLevelData.LevelIndex) return;
        SceneManager.LoadScene(index);
        SatelliteDish.SceneStatusChange.Invoke(SceneStatus.Invalid, SceneStatus.Loading);
    }
    private void HandleSceneChange(SceneStatus prev, SceneStatus next)
    {
        if(prev == next) return;
        if(next == SceneStatus.Invalid) SetGameState(GameStatus.Paused);
        if(next == SceneStatus.Loading) SetGameState(GameStatus.Paused);
        if(next == SceneStatus.Running) SetGameState(GameStatus.Gameplay);
    }
    public void TogglePause()
    { 
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            _input.PointAndClick.Disable();
        } 
        else
        {
            Time.timeScale = 1;
            _input.PointAndClick.Enable();
        }
        GameStatus newStatus = (Time.timeScale == 0) ? GameStatus.Paused : GameStatus.Gameplay;
        SetGameState(newStatus);
    }
    private void AdjustEnergy(int amount)
    {
        _currentEnergy = Mathf.Clamp(_currentEnergy + amount , 0 , 100);
        SatelliteDish.EnergyEffect.Invoke(_currentEnergy);
        Debug.Log($"{_currentEnergy}");
        if (_currentEnergy <= 0)
        {
            ChangeScene(_gameOverScene.buildIndex);
            // Wait for the Scene to show GameOver, then open the Menu
            TogglePause();
        }
    }
  private void Count(InteractableData data)
  {
    if(data.Type != InteractionType.Item) return;
    _currentLevelData.Counter++;
    if(_currentLevelData.Counter >= _currentLevelData.CountRequired) _currentLevelData.IsCompleted = true;
  }
  private void CheckLevelChange(InteractableData data)
    {
        if(data.Name != "GoodDoor") return;
        if(data.Type == InteractionType.Action && data.Name == "GoodDoor")
        {
            _currentLevelData.Counter = 0;
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            ChangeScene(sceneIndex+1);
        }
    }
    private void HandleMaskChange(MaskSetting mask) => _currentMask = mask;
}
