using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public List<Sprite> shapes;
    public List<Color> colors;

    public GameObject shapePrefab;

    public RectTransform content;
    public List<DropTargetUI> dropTargets;

    private int currentLevel = 0;
    private int droppedShapesCount = 0;

    public TMP_Text levelText;
    public TMP_Text scoreText;
    private int score = 0;
    public GameObject completeDialog;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private List<DraggableUI> activeShapes = new List<DraggableUI>();
    private List<DropTargetUI> filledDropTargets = new List<DropTargetUI>();

    private List<DropTargetUI> currentLevelActiveDropTargets = new List<DropTargetUI>();

    void Start()
    {
        if (dropTargets == null || dropTargets.Count == 0)
        {
            Debug.LogError("Drop targets are not assigned or the list is empty.");
            return;
        }

        UpdateScoreText();

        StartLevel();
    }

    void StartLevel()
    {
        currentLevel++;
        droppedShapesCount = 0;
        filledDropTargets.Clear();
        currentLevelActiveDropTargets.Clear();

        foreach (var shape in activeShapes) Destroy(shape.gameObject);
        activeShapes.Clear();

        foreach (var target in dropTargets)
        {
            target.Clear();
            target.ResetTargetVisual();
        }

        levelText.text = $"Level {currentLevel}";

        GenerateLevel(currentLevel);
    }

    void GenerateLevel(int level)
    {
        List<(Sprite shape, Color color)> allUniqueCombinations = new List<(Sprite, Color)>();
        foreach (Sprite s in shapes)
        {
            foreach (Color c in colors)
            {
                allUniqueCombinations.Add((s, c));
            }
        }

        int numberOfObjectsToSpawn = level;

        int numberOfActiveDropTargets = Mathf.Min(numberOfObjectsToSpawn, dropTargets.Count);

        numberOfActiveDropTargets = Mathf.Min(numberOfActiveDropTargets, allUniqueCombinations.Count);


        if (numberOfObjectsToSpawn == 0 || numberOfActiveDropTargets == 0)
        {
            Debug.LogError("Cannot generate level: Not enough objects to spawn or active drop targets.");
            EndGame();
            return;
        }

        List<(Sprite shape, Color color)> selectedTargetCombinations = GetRandomUniqueItems(allUniqueCombinations, numberOfActiveDropTargets);

        List<DropTargetUI> selectedDropTargets = GetRandomUniqueItems(dropTargets, numberOfActiveDropTargets);
        currentLevelActiveDropTargets.AddRange(selectedDropTargets);

        for (int i = 0; i < selectedTargetCombinations.Count; i++)
        {
            selectedDropTargets[i].SetTarget(selectedTargetCombinations[i].shape, selectedTargetCombinations[i].color);
        }

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            GameObject shapeObject = Instantiate(shapePrefab, content);
            DraggableUI shapeComponent = shapeObject.GetComponent<DraggableUI>();

            (Sprite shape, Color color) chosenCombination = selectedTargetCombinations[Random.Range(0, selectedTargetCombinations.Count)];

            shapeComponent.SetSprite(chosenCombination.shape);
            shapeComponent.SetColor(chosenCombination.color);

            RectTransform rectTransform = shapeObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(
                Random.Range(-content.rect.width / 2, content.rect.width / 2),
                Random.Range(-content.rect.height / 2, content.rect.height / 2)
            );

            activeShapes.Add(shapeComponent);
            shapeObject.name = $"Draggable {i + 1} ({chosenCombination.shape.name}) - Color ({chosenCombination.color.ToString()})";
            shapeObject.SetActive(true);
        }

        ShuffleLib.ShuffleList(activeShapes);
    }

    public void OnShapeDropped(DraggableUI droppedShape, DropTargetUI dropTarget)
    {
        bool isCorrectMatch = (droppedShape.sprite == dropTarget.targetShape && droppedShape.color == dropTarget.targetColor);

        if (isCorrectMatch)
        {
            AudioSource.PlayClipAtPoint(correctSound, Vector3.zero);
            score += 500;

            activeShapes.Remove(droppedShape);

            droppedShapesCount++;

            if (droppedShapesCount >= currentLevel)
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

    void EndGame()
    {
        Debug.Log("Game Complete!");
        completeDialog.SetActive(true);
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }

    List<T> GetRandomUniqueItems<T>(List<T> sourceList, int count)
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