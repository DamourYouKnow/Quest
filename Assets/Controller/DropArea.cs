using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

class DropArea : MonoBehaviour, IDropHandler {
    public void OnDrop(PointerEventData eventData) {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null) {
            draggable.ReturnParent = this.transform;
        }
    }
}

