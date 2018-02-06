using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public void OnBeginDrag(PointerEventData eventData) {
        throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData) {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        throw new NotImplementedException();
    }
}