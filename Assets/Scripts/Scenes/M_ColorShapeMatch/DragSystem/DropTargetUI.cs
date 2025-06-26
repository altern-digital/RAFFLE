using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class DropTargetUI : MonoBehaviour, IDropHandler
{
    public Sprite targetShape;
    private LevelManager levelManager;
    public List<DraggableUI> draggableItems;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene.");
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

            if (isShapeMatch)
            {
                droppedItem.transform.SetParent(this.transform);
                droppedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;

                levelManager.OnShapeDropped(droppedItem, this);
                draggableItems.Add(droppedItem);
            }
        }
    }

    public void SetTarget(Sprite shape)
    {
        targetShape = shape;
    }
}