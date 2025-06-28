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
    public string scoreKey = "ColorShapeScore";
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

        score = PlayerPrefs.GetInt(scoreKey, 0);
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
        // Get all possible unique shape-color combinations from your defined lists.
        List<(Sprite shape, Color color)> allUniqueCombinations = new List<(Sprite, Color)>();
        foreach (Sprite s in shapes)
        {
            foreach (Color c in colors)
            {
                allUniqueCombinations.Add((s, c));
            }
        }

        // Determine the number of objects to spawn for this level.
        // This is exactly the 'level' number.
        int numberOfObjectsToSpawn = level;

        // Ensure we don't try to spawn more objects than we have available drop targets.
        // If level > dropTargets.Count, we can only use as many targets as available.
        int numberOfActiveDropTargets = Mathf.Min(numberOfObjectsToSpawn, dropTargets.Count);

        // Ensure we have enough unique combinations to pick for our active targets.
        // If numberOfActiveDropTargets > allUniqueCombinations.Count, we cap it.
        numberOfActiveDropTargets = Mathf.Min(numberOfActiveDropTargets, allUniqueCombinations.Count);


        if (numberOfObjectsToSpawn == 0 || numberOfActiveDropTargets == 0)
        {
            Debug.LogError("Cannot generate level: Not enough objects to spawn or active drop targets.");
            EndGame();
            return;
        }

        // 1. Select the unique combinations that will be represented by the drop targets
        List<(Sprite shape, Color color)> selectedTargetCombinations = GetRandomUniqueItems(allUniqueCombinations, numberOfActiveDropTargets);

        // 2. Assign these combinations to a subset of your actual DropTargetUI elements
        List<DropTargetUI> selectedDropTargets = GetRandomUniqueItems(dropTargets, numberOfActiveDropTargets);
        currentLevelActiveDropTargets.AddRange(selectedDropTargets); // Track active targets for this level

        for (int i = 0; i < selectedTargetCombinations.Count; i++)
        {
            selectedDropTargets[i].SetTarget(selectedTargetCombinations[i].shape, selectedTargetCombinations[i].color);
        }

        // 3. Spawn 'level' number of draggable objects, allowing duplicates.
        // Each spawned object will randomly pick one of the 'selectedTargetCombinations'.
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            GameObject shapeObject = Instantiate(shapePrefab, content);
            DraggableUI shapeComponent = shapeObject.GetComponent<DraggableUI>();

            // Randomly pick one of the active target combinations for this draggable item
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

        ShuffleShapesPosition();
    }

    void ShuffleShapesPosition()
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
        bool isCorrectMatch = (droppedShape.sprite == dropTarget.targetShape && droppedShape.color == dropTarget.targetColor);

        if (isCorrectMatch)
        {
            AudioSource.PlayClipAtPoint(correctSound, Vector3.zero);
            score += 500;

            activeShapes.Remove(droppedShape);
            // We do NOT add to filledDropTargets here because a target can receive multiple correct drops.
            // A target is only "filled" in terms of its type being available.

            droppedShapesCount++;

            // Check if ALL 'level' objects have been correctly dropped.
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
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.Save();
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