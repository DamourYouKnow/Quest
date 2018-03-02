using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Quest.Core.Cards;
using Quest.Core;

class DropArea : MonoBehaviour, IDropHandler {
    private const int defaultOffset = 3;

	void Update() {
		this.AdjustOffset ();
	}

    public void OnDrop(PointerEventData eventData) {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null) {
			Card c = draggable.gameObject.GetComponent<GameCard> ().Card;
			if (c != null) {
				GameCardArea gca = draggable.ReturnParent.gameObject.GetComponent<GameCardArea> ();
				CardArea ca1 = null;
				if (gca == null) {
					ca1 = draggable.ReturnParent.gameObject.GetComponent<QuestGameCardArea> ().QuestCards;
				}
				else {
					ca1 = gca.Cards;
				}
				gca = null;
				gca = this.transform.gameObject.GetComponent<GameCardArea> ();
				CardArea ca2 = null;
				QuestArea qa2 = null;
				if (gca == null) {
					ca2 = this.transform.gameObject.GetComponent<QuestGameCardArea> ().QuestCards;
				}
				else {
					ca2 = gca.Cards;
				}
				ca1.Transfer (ca2, c);
				if (ca2 != null && ca2.Cards.Contains (c)) {
					draggable.ReturnParent = this.transform;
				}
			}
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

