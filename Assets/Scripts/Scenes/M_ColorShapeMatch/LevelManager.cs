using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public List<Sprite> shapes;
    public List<Color> colors;

    public GameObject shapePrefab;

    public RectTransform content;
    public List<DropTargetUI> dropTargets;

    public int totalLevels = 20;
    private int currentLevel = 0;
    private int droppedShapesCount = 0;

    public TMP_Text levelText;
    public TMP_Text scoreText;
    private int score = 0;
    public GameObject completeDialog;
    public string scoreKey = "ColorShapeScore";
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private List<DraggableUI> activeShapes = new List<DraggableUI>();

    private void Start()
    {
        if (dropTargets == null || dropTargets.Count == 0)
        {
            Debug.LogError("Drop targets are not assigned or the list is empty.");
            return;
        }

        score = PlayerPrefs.GetInt(scoreKey, 0);
        UpdateScoreText();

        StartLevel();
    }

    private void StartLevel()
    {
        currentLevel++;
        droppedShapesCount = 0;

        if (currentLevel > totalLevels)
        {
            EndGame();
            return;
        }

        foreach (var shape in activeShapes) Destroy(shape.gameObject);
        activeShapes.Clear();

        levelText.text = $"Level {currentLevel}/{totalLevels}";

        GenerateLevel(currentLevel);
    }

    private void GenerateLevel(int level)
    {
        int numberOfShapes = Mathf.Min(level, shapes.Count);

        List<Sprite> levelShapes = GetRandomUniqueItems(shapes, numberOfShapes);

        for (int i = 0; i < dropTargets.Count; i++)
        {
            dropTargets[i].Clear();
        }

        for (int i = 0; i < numberOfShapes; i++)
        {
            GameObject shapeObject = Instantiate(shapePrefab, content);
            DraggableUI shapeComponent = shapeObject.GetComponent<DraggableUI>();

            shapeComponent.SetSprite(levelShapes[i]);
            shapeComponent.SetColor(colors[Random.Range(0, colors.Count)]); // Assign a random color

            RectTransform rectTransform = shapeObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(
                Random.Range(-content.rect.width / 2, content.rect.width / 2),
                Random.Range(-content.rect.height / 2, content.rect.height / 2)
            );

            activeShapes.Add(shapeComponent);
            shapeObject.name = $"Shape {i + 1} ({levelShapes[i].name})";
            shapeObject.SetActive(true);
        }

        ShuffleShapesPosition();
    }

    private void ShuffleShapesPosition()
    {
        System.Random rng = new System.Random();
        int n = activeShapes.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector2 tempPosition = activeShapes[k].GetComponent<RectTransform>().anchoredPosition;
            activeShapes[k].GetComponent<RectTransform>().anchoredPosition = activeShapes[n].GetComponent<RectTransform>().anchoredPosition;
            activeShapes[n].GetComponent<RectTransform>().anchoredPosition = tempPosition;
        }
    }

    public void OnShapeDropped(DraggableUI droppedShape, DropTargetUI dropTarget)
    {
        bool isCorrectMatch = (droppedShape.sprite == dropTarget.targetShape);

        if (isCorrectMatch)
        {
            AudioSource.PlayClipAtPoint(correctSound, Vector3.zero);
            score += 500;

            activeShapes.Remove(droppedShape);

            droppedShapesCount++;

            if (droppedShapesCount >= activeShapes.Count + droppedShapesCount) // Corrected condition: check against the number of targets used
            {
                Debug.Log("Level Complete!");
                Invoke("StartLevel", 1f);
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(incorrectSound, Vector3.zero);
            score -= 250;
        }

        UpdateScoreText();
    }

    private void EndGame()
    {
        Debug.Log("Game Complete!");
        completeDialog.SetActive(true);
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.Save();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }

    private List<T> GetRandomUniqueItems<T>(List<T> sourceList, int count)
    {
        List<T> result = new List<T>();
        List<T> tempList = new List<T>(sourceList);
        System.Random rng = new System.Random();

        for (int i = 0; i < count; i++)
        {
            if (tempList.Count == 0) break;
            int randomIndex = rng.Next(tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        return result;
    }
}