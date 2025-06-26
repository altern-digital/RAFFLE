using MyBox;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static bool IsMuted
    {
        get => PlayerPrefs.GetInt("IsMuted", 0) == 1;
        set => PlayerPrefs.SetInt("IsMuted", value ? 1 : 0);
    }

    [AutoProperty]
    public AudioSource audioSource;

    private void Start()
    {
        SetMute(IsMuted);
    }

    public void SetMute(bool mute)
    {
        IsMuted = mute;
        audioSource.mute = mute;
    }
}
