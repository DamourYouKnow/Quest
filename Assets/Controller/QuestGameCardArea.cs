using System;
using UnityEngine;
using Quest.Core.Cards;
using Quest.Core;

public class QuestGameCardArea : MonoBehaviour {
	QuestArea questCards;

	public QuestArea QuestCards {
		get { return this.questCards; }
		set { this.questCards = value; }
	}
	public QuestGameCardArea ()
	{
		questCards = null;
	}
}
