using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

class DropArea : MonoBehaviour, IDropHandler {
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

    private void adjustOffset() {
        float width = this.GetComponent<RectTransform>().rect.width;
        // TODO.
    }
}

