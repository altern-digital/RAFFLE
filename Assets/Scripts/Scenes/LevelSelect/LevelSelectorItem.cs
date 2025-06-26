using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectorItem : MonoBehaviour
{
    public LevelData levelData;

    [Header("UI")]
    public TMP_Text levelIndexText;

    [AutoProperty] public Image levelImage;
    [AutoProperty] public Button levelButton;

    public UnityEvent onLevelSelected;

    public bool IsUnlocked
    {
        get => levelButton.interactable;
        set
        {
            levelButton.interactable = value;
            levelImage.color = value ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }

    void Awake()
    {
        levelData.Load();
    }

    private void Start()
    {
        levelButton.onClick.AddListener(onLevelSelected.Invoke);
    }

    [ButtonMethod]
    public void LoadData()
    {
        levelData.Load();
    }

    [ButtonMethod]
    public void SaveData()
    {
        levelData.Save();
    }
}
