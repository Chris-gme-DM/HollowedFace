using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls the GameStatus and invokes events as they occur if overarching gameplay rules apply.
/// </summary>
public class GameManager : MonoBehaviour
{
  private static GameManager _instance;
  public GameStatus CurrentStatus {get; private set;}
  private int _currentInGameTime;
  [SerializeField] private Scene _gameOverScene;
  [SerializeField] private float _secondsForTimePass;
  private float _timer;
  private int _currentEnergy;
  private int _level;
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
  }
  private void OnDisable()
  {
    SatelliteDish.MaskChange.RemoveListener(HandleMaskChange);
    SatelliteDish.RequestEnergyAdjustment.RemoveListener(AdjustEnergy);
    SatelliteDish.SceneStatusChange.RemoveListener(HandleSceneChange);
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
        _level = 1;
        _currentInGameTime = 480;
        SatelliteDish.TimePass.Invoke(_currentInGameTime);
        AdjustEnergy(80);
        ChangeScene(1);
    }
    private void ChangeScene(int index)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == _level) return;
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
    private void HandleMaskChange(MaskSetting mask) => _currentMask = mask;
}
