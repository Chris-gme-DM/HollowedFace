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
  void Awake()
  {
    if ( _instance != null)
    {
        if (_instance != this) Destroy(gameObject);
        _instance = this;
        DontDestroyOnLoad(this);
    }
  }
  void Update()
    {
        if( CurrentStatus == GameStatus.Gameplay)
        {
            _timer += Time.deltaTime;
            if ( _timer >= _secondsForTimePass)
            {
                _timer = 0;
                SatelliteDish.TimePass.Invoke(_currentInGameTime);
            }
        }
    }
  private void SetGameState(GameStatus next)
    {
        if (next == CurrentStatus) return;
        CurrentStatus = next;
        SatelliteDish.GameStatusChange.Invoke(next);
    }
  public void RestartGame()
    {
        ResetGame();
        TogglePause();
    }
    private void ResetGame()
    {
        _level = 1;
        AdjustEnergy(100);
        ChangeScene(1);
    }
    private void ChangeScene(int index)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == _level) return;
        SceneManager.LoadScene(index);
        SatelliteDish.SceneStatusChange.Invoke(SceneStatus.Invalid, SceneStatus.Loading);
    }
    public void TogglePause()
    { 
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
        GameStatus newStatus = (Time.timeScale == 0) ? GameStatus.Paused : GameStatus.Gameplay;
        SetGameState(newStatus);
    }
    public void AdjustEnergy(int amount)
    {
        _currentEnergy = Mathf.Clamp(_currentEnergy + amount , 0 , 100);
        SatelliteDish.EnergyEffect.Invoke(_currentEnergy);
        if (_currentEnergy <= 0)
        {
            ChangeScene(_gameOverScene.buildIndex);
            // Wait for the Scene to show GameOver, then open the Menu
            TogglePause();
        }
    }
}
