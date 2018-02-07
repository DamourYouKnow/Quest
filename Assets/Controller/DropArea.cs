using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

class DropArea : MonoBehaviour, IDropHandler {
    private const int defaultOffset = 3;

    public void OnDrop(PointerEventData eventData) {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null) {
            draggable.ReturnParent = this.transform;
        }
    }

    public List<Draggable> GetDraggables() {
        List<Draggable> draggables = new List<Draggable>();
        foreach (Transform t in this.transform) {
            if (t.GetComponent<Draggable>() != null) {
                draggables.Add(t.GetComponent<Draggable>());
            }
        }
        return draggables;
    }

    public void AdjustOffset() {
        float width = this.GetComponent<RectTransform>().rect.width;
        float draggablesWidth = this.getDraggablesWidth();
        float currentOffset = defaultOffset;
        float usedWidth = draggablesWidth + (currentOffset * this.GetDraggables().Count);

        float newOffset = defaultOffset;
        if (Math.Ceiling(usedWidth) > width) {
            newOffset = (float)(Math.Ceiling((width - (currentOffset * 50) - draggablesWidth) / this.GetDraggables().Count));
        }

        this.GetComponent<HorizontalLayoutGroup>().spacing = newOffset;
    }

    private float getDraggablesWidth() {
        float widthSum = 0f;
        List<Draggable> draggables = this.GetDraggables();
        foreach (Draggable draggable in draggables) {
            widthSum += draggable.GetComponent<RectTransform>().rect.width;
        }
        return widthSum;
    }
}

