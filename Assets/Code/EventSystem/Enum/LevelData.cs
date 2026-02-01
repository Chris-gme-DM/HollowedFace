using UnityEngine;

[CreateAssetMenu(menuName = "HollowedFace/LevelData")]
public class LevelData : ScriptableObject
{
  public LevelData Instance;
  public int LevelIndex;
  public int RequiredItems;
  [TextArea] public string LevelDescription;
  public bool IsCompleted = false;
  public int Counter;
  public int CountRequired;
  public void Count()
  {
    Counter++;
    if(Counter >= CountRequired) IsCompleted = true;
  }
}