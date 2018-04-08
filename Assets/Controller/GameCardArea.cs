using System;
using System.Collections.Generic;
using UnityEngine;
using Quest.Core.View;

public class GameCardArea  : MonoBehaviour {
	List<Card> cards;

	public List<Card> Cards {
		get { return this.cards; }
		set { this.cards = value; }
	}
	void Awake() {
		this.cards = null;
	}
}
