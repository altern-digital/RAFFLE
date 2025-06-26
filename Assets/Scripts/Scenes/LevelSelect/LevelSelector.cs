using UnityEngine;
using MyBox;
using System.Collections.Generic;
using UniRx;
using System;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector Instance;

    [AutoProperty]
    public CanvasGroup _canvasGroup;

    public List<LevelSelectorItem> levelItems;
    public int unlockedLevel = 0;

    public int? currentLevelIndex = null;

    private void Awake()
    {
        Instance = this;
    }

    [ButtonMethod]
    private void OnValidate()
    {
        levelItems = new List<LevelSelectorItem>(GetComponentsInChildren<LevelSelectorItem>());

        for (int i = 0; i < levelItems.Count; i++)
        {
            levelItems[i].gameObject.name = $"Level {i + 1}";
            levelItems[i].levelIndexText.text = (i + 1).ToString();
        }
    }

    private void Start()
    {
        unlockedLevel = Mathf.Clamp(unlockedLevel, 0, levelItems.Count - 1);
        UpdateLevelSelection();

        for (int i = 0; i < levelItems.Count; i++)
        {
            levelItems[i].onLevelSelected.AddListener(() =>
            {
                currentLevelIndex = i;
                Debug.Log($"Selected level {i + 1}: {levelItems[i].levelData.levelKey}");
            });
        }
    }

    private void Update()
    {
        _canvasGroup.blocksRaycasts = _canvasGroup.interactable = !currentLevelIndex.HasValue;
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _canvasGroup.interactable ? 1f : 0f, Time.deltaTime * 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, _canvasGroup.interactable ? Vector3.one : Vector3.one * 2, Time.deltaTime * 10f);
    }

    [ButtonMethod]
    private void UpdateLevelSelection()
    {
        for (int i = 0; i < levelItems.Count; i++)
        {
            levelItems[i].IsUnlocked = i <= unlockedLevel;
        }
    }

    [ButtonMethod]
    public void ExitLevel()
    {
        currentLevelIndex = null;
        Debug.Log("Exited level selection.");
    }
}

[System.Serializable]
public class LevelData
{
    public string levelKey;

    public void Load()
    {
        string json = PlayerPrefs.GetString(levelKey, "{}");
        JsonUtility.FromJsonOverwrite(json, this);

        Debug.Log($"Level data for {levelKey} loaded: {json}");
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(levelKey)) levelKey = Guid.NewGuid().ToString();

        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(levelKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Level data for {levelKey} saved: {json}");
    }
}