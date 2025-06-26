using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M5LevelManager : MonoBehaviour
{
    public List<string> minigames;

    public void PlayMinigame(int index)
    {
        string sceneName = minigames[index % minigames.Count];
        SceneManager.LoadScene(sceneName);
    }
}
