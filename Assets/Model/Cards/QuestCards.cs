﻿using System;
using System.Collections.Generic;

namespace Quest.Core.Cards{
	public abstract class QuestCard:StoryCard {
		public int stages;
		public List<FoeCard> questFoes;

		public QuestCard(QuestMatch match):base(match) {
			
		}
		public virtual void run(){
			this.match.CurrentStory = this;
		}
	}
	public class BoarHunt : QuestCard{
		public BoarHunt(QuestMatch match) : base(match) {
			this.name = "Boar Hunt";
			this.imageFilename = "quest_boar_hunt.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class DefendTheQueensHonor : QuestCard{
		public DefendTheQueensHonor(QuestMatch match) : base(match) {
			this.name = "Defend the Queen's Honor";
			this.imageFilename = "quest_defend_the_queens_honor.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class JourneyThroughTheEnchantedForest : QuestCard{
		public JourneyThroughTheEnchantedForest(QuestMatch match) : base(match) {
			this.name = "Journey Through the Enchanted Forest";
			this.imageFilename = "quest_journey_through_the_enchanted_forest.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class RepelTheSaxonRaiders : QuestCard{
		public RepelTheSaxonRaiders(QuestMatch match) : base(match) {
			this.name = "Repel the Saxon Raiders";
            this.imageFilename = "quest_repel_the_saxon_raiders.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class RescueTheFairMaiden : QuestCard{
		public RescueTheFairMaiden(QuestMatch match) : base(match) {
			this.name = "Rescue the Fair Maiden";
			this.imageFilename = "quest_rescue_the_fair_maiden.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class SearchForTheHolyGrail : QuestCard{
		public SearchForTheHolyGrail(QuestMatch match) : base(match) {
			this.name = "Search for the Holy Grail";
			this.imageFilename = "quest_search_for_the_holy_grail.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class SearchForTheQuestingBeast : QuestCard{
		public SearchForTheQuestingBeast(QuestMatch match) : base(match) {
			this.name = "Search for the Questing Beast";
			this.imageFilename = "quest_search_for_the_questing_beast.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class SlayTheDragon : QuestCard{
		public SlayTheDragon(QuestMatch match) : base(match) {
			this.name = "Slay the Dragon";
			this.imageFilename = "quest_slay_the_dragon.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class TestOfTheGreenKnight : QuestCard{
		public TestOfTheGreenKnight(QuestMatch match) : base(match) {
			this.name = "Test of the Green Knight";
			this.imageFilename = "quest_test_of_the_green_knight.png";
		}
		public override void run(){
			base.run ();

		}
	}
	public class VanquishKingArthursEnemies : QuestCard{
		public VanquishKingArthursEnemies(QuestMatch match) : base(match) {
			this.name = "Vanquish King Arthur's Enemies";
			this.imageFilename = "quest_vanquish_king_arthurs_enemies.png";
		}
		public override void run(){
			base.run ();

		}
	}
}
	