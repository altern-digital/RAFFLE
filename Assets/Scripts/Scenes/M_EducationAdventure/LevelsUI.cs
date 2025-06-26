using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsUI : MonoBehaviour
{
    public GameObject prefab;
    public int levelCounts;
    public Transform content;
    public M5LevelManager levelManager;

    void Start()
    {
        for (int i = 0; i < levelCounts; i++)
        {
            GameObject go = Instantiate(prefab, content);
            go.GetComponentInChildren<NiceUI>().delay = i * 0.05f;
            go.name = "Level " + (i + 1);
            go.SetActive(true);
            go.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            go.GetComponentInChildren<Button>().onClick.AddListener(() => levelManager.PlayMinigame(i));
        }
    }
}
