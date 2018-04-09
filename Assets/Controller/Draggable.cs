using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    Transform returnParent;
    CanvasGroup raycastBlocker;

    public Transform ReturnParent {
        get { return this.returnParent; }
        set { this.returnParent = value; }
    }

    public void Start() {
        this.raycastBlocker = this.gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        this.returnParent = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent, false);

        raycastBlocker.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 mousePoint = eventData.position;
        mousePoint.z = 1;
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnEndDrag(PointerEventData eventData) {
        this.transform.SetParent(returnParent, false);
        raycastBlocker.blocksRaycasts = true;
        this.reposition();
    }

    private void reposition() {
        DropArea dropArea = this.transform.parent.GetComponent<DropArea>();
        if (dropArea != null) {
            this.transform.SetSiblingIndex(this.computeOnDropIndex());
        }
        dropArea.AdjustOffset();
    }

    private int computeOnDropIndex() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<Draggable> draggables = this.transform.parent.GetComponent<DropArea>().GetDraggables();

        for (int i = 0; i < draggables.Count - 1; i++) {
            if (mousePos.x < draggables[i].transform.position.x) {
                return i;
            }
        }

        return this.transform.parent.childCount - 1;
    }
}
