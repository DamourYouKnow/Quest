using System;
using UnityEngine;
using Quest.Core.Cards;
using Quest.Core;
using System.Collections.Generic;

public class TournamentCardArea : MonoBehaviour {
	TournamentArea cards;

	public TournamentArea Cards {
		get { return this.cards; }
		set { this.cards = value; }
	}
	public TournamentCardArea ()
	{
		cards = null;
	}


}
