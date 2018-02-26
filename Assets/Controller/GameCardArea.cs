using System;
using UnityEngine;
using Quest.Core.Cards;
using Quest.Core;

public class GameCardArea : MonoBehaviour {
	CardArea cards;

	public CardArea Cards {
		get { return this.cards; }
		set { this.cards = value; }
	}
	public GameCardArea ()
	{
		cards = null;
	}
}

