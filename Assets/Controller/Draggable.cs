using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public void OnBeginDrag(PointerEventData eventData) {
        //throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 mousePoint = eventData.position;
        mousePoint.z = 1;
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnEndDrag(PointerEventData eventData) {
        //throw new NotImplementedException();
    }
}