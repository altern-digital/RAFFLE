using UnityEngine;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Start()
    {
        Application.targetFrameRate = 999; // Set the target frame rate to 60 FPS
    }
}
