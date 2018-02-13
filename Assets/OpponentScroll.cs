using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentScroll : MonoBehaviour {
	Scrollbar scrollbar;
	float parentOffset;
	float opponentSize;
	// Use this for initialization
	void Start () {
		scrollbar = this.gameObject.GetComponent<Scrollbar>();
		scrollbar.onValueChanged.AddListener (valueChanged);
		parentOffset = 0;
	}

	// Update is called once per frame
	void Update () {
		//assumes first child is opponents
		scrollbar.numberOfSteps = scrollbar.transform.parent.GetChild (0).childCount;
		if (scrollbar.numberOfSteps > 0) {
			RectTransform oppTransform = (scrollbar.transform.parent.GetChild (0).GetChild(0) as RectTransform);
			RectTransform screenSize = (scrollbar.transform.parent.parent as RectTransform);
			opponentSize = oppTransform.rect.width;
			opponentSize += (screenSize.rect.width % opponentSize);
			opponentSize /= 100.0f;
		}
	}

	void valueChanged(float value){
		scrollbar.transform.parent.transform.GetChild(0).transform.Translate (parentOffset-(value * opponentSize*scrollbar.numberOfSteps), 0, 0);
		parentOffset = value * opponentSize*scrollbar.numberOfSteps;
	}
}
