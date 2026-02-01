using UnityEngine;

[CreateAssetMenu(menuName = "HollowedFace/LevelData")]
public class LevelData : ScriptableObject
{
  public int LevelIndex;
  public int RequiredItems;
  [TextArea] public string LevelDescription;
  public int TimeToStart; // 480 e.g. is 8:00 o clock
  public int TimeBeforeEnd; // be reasonable
  public bool IsCompleted = false;
  public int Counter = 0;
  public int CountRequired;
  public void Count()
  {
    Counter++;
    if(Counter >= CountRequired) IsCompleted = true;
  }
}