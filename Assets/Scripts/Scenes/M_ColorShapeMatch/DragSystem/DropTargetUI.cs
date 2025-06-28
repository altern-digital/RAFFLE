using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DropTargetUI : MonoBehaviour, IDropHandler
{
    public Sprite targetShape;
    public Color targetColor;
    public Image image;
    private LevelManager levelManager;
    public List<DraggableUI> draggableItems;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene.");
        }

        if (image == null)
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("Image component not found on DropTargetUI. Please assign it in the Inspector or ensure one exists.");
            }
        }
    }

    public void Clear()
    {
        foreach (var item in draggableItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        draggableItems.Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI droppedItem = eventData.pointerDrag.GetComponent<DraggableUI>();

        if (droppedItem != null && levelManager != null)
        {
            bool isShapeMatch = (targetShape == null || droppedItem.sprite == targetShape);
            bool isColorMatch = (targetColor == Color.clear || droppedItem.color == targetColor);

            if (isShapeMatch && isColorMatch)
            {
                droppedItem.transform.SetParent(this.transform);
                droppedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;

                levelManager.OnShapeDropped(droppedItem, this);
                draggableItems.Add(droppedItem);
            }
            else
            {
                levelManager.OnShapeDropped(droppedItem, this);
            }
        }
    }

    public void SetTarget(Sprite shape, Color color)
    {
        targetShape = shape;
        targetColor = color;

        if (image != null)
        {
            image.sprite = targetShape;
            image.color = targetColor;
            image.enabled = (targetShape != null || targetColor != Color.clear);
        }
    }

    public void ResetTargetVisual()
    {
        targetShape = null;
        targetColor = Color.clear;

        if (image != null)
        {
            image.sprite = null;
            image.color = Color.clear;
            image.enabled = false;
        }
    }
}