using System;
using UnityEngine;
using Quest.Core.View;

public class GameCard : MonoBehaviour {
	Card card;

	public Card Card {
		get { return this.card; }
		set { this.card = value; }
	}
	public GameCard ()
	{
		card = null;
	}
}
