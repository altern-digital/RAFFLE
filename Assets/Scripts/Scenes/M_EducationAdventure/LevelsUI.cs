using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsUI : MonoBehaviour
{
    public GameObject prefab;
    public int levelCounts;
    public Transform content;

    void Start()
    {
        for (int i = 0; i < levelCounts; i++)
        {
            GameObject go = Instantiate(prefab, content);
            go.GetComponentInChildren<NiceUI>().delay = i * 0.05f;
            go.GetComponentInChildren<LevelUI>().levelIndex = i;
            go.name = "Level " + (i + 1);
            go.SetActive(true);
            go.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            Debug.Log($"Created level button for level {i + 1} with index {i}");
        }
    }
}
