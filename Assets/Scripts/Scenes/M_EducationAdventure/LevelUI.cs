using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public int levelIndex;
    public M5LevelManager levelManager;


    public void Play()
    {
        levelManager?.PlayMinigame(levelIndex);
    }
}
