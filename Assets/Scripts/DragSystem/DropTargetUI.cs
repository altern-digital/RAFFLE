using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropTargetUI : MonoBehaviour, IDropHandler
{
    public List<DraggableUI> droppedItems = new List<DraggableUI>();

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI droppedItem = eventData.pointerDrag.GetComponent<DraggableUI>();

        if (droppedItem != null)
        {
            droppedItem.transform.SetParent(this.transform);

            droppedItems.Add(droppedItem);

            Debug.Log($"Item {droppedItem.name} dropped successfully on {gameObject.name}!");
            Debug.Log($"Items currently in {gameObject.name}:");
            foreach (DraggableUI item in droppedItems)
            {
                Debug.Log($"- {item.name}");
            }
        }
    }
}