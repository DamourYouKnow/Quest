using System;
using System.Collections.Generic;

namespace Quest.Core.Cards{
	public abstract class QuestCard:StoryCard {
		protected int stages;
		protected List<Type> questFoes;

		public int Stages {
			get { return this.stages; }
		}
		public List<Type> QuestFoes {
			get { return this.questFoes; }
		}

		public QuestCard(QuestMatch match):base(match) {
			this.questFoes = new List<Type> ();
			
		}
		public abstract void run ();
	}
	public class BoarHunt : QuestCard{
		public BoarHunt(QuestMatch match) : base(match) {
			this.name = "Boar Hunt";
			this.imageFilename = "quest_boar_hunt.png";
			this.stages = 2;
			this.questFoes.Add (typeof(Boar));
		}
		public override void run(){
			this.match.CurrentStory = this;
			Logger log = new Logger ();
			log.Log (this.match.CurrentStory.ToString());
		}
	}
	public class DefendTheQueensHonor : QuestCard{
		public DefendTheQueensHonor(QuestMatch match) : base(match) {
			this.name = "Defend the Queen's Honor";
			this.imageFilename = "quest_defend_the_queens_honor.png";
			this.stages = 4;
			this.questFoes.Add (typeof(BlackKnight));
			this.questFoes.Add (typeof(Boar));
			this.questFoes.Add (typeof(Dragon));
			this.questFoes.Add (typeof(EvilKnight));
			this.questFoes.Add (typeof(Giant));
			this.questFoes.Add (typeof(GreenKnight));
			this.questFoes.Add (typeof(Mordred));
			this.questFoes.Add (typeof(RobberKnight));
			this.questFoes.Add (typeof(SaxonKnight));
			this.questFoes.Add (typeof(Saxons));
			this.questFoes.Add (typeof(Thieves));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class JourneyThroughTheEnchantedForest : QuestCard{
		public JourneyThroughTheEnchantedForest(QuestMatch match) : base(match) {
			this.name = "Journey Through the Enchanted Forest";
			this.imageFilename = "quest_journey_through_the_enchanted_forest.png";
			this.stages = 3;
			this.questFoes.Add (typeof(EvilKnight));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class RepelTheSaxonRaiders : QuestCard{
		public RepelTheSaxonRaiders(QuestMatch match) : base(match) {
			this.name = "Repel the Saxon Raiders";
			this.imageFilename = "quest_repel_the_saxon_raiders.png";
			this.stages = 2;
			this.questFoes.Add (typeof(SaxonKnight));
			this.questFoes.Add (typeof(Saxons));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class RescueTheFairMaiden : QuestCard{
		public RescueTheFairMaiden(QuestMatch match) : base(match) {
			this.name = "Rescue the Fair Maiden";
			this.imageFilename = "quest_rescue_the_fair_maiden.png";
			this.stages = 3;
			this.questFoes.Add (typeof(BlackKnight));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class SearchForTheHolyGrail : QuestCard{
		public SearchForTheHolyGrail(QuestMatch match) : base(match) {
			this.name = "Search for the Holy Grail";
			this.imageFilename = "quest_search_for_the_holy_grail.png";
			this.stages = 5;
			this.questFoes.Add (typeof(BlackKnight));
			this.questFoes.Add (typeof(Boar));
			this.questFoes.Add (typeof(Dragon));
			this.questFoes.Add (typeof(EvilKnight));
			this.questFoes.Add (typeof(Giant));
			this.questFoes.Add (typeof(GreenKnight));
			this.questFoes.Add (typeof(Mordred));
			this.questFoes.Add (typeof(RobberKnight));
			this.questFoes.Add (typeof(SaxonKnight));
			this.questFoes.Add (typeof(Saxons));
			this.questFoes.Add (typeof(Thieves));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class SearchForTheQuestingBeast : QuestCard{
		public SearchForTheQuestingBeast(QuestMatch match) : base(match) {
			this.name = "Search for the Questing Beast";
			this.imageFilename = "quest_search_for_the_questing_beast.png";
			this.stages = 4;
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class SlayTheDragon : QuestCard{
		public SlayTheDragon(QuestMatch match) : base(match) {
			this.name = "Slay the Dragon";
			this.imageFilename = "quest_slay_the_dragon.png";
			this.stages = 3;
			this.questFoes.Add (typeof(Dragon));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class TestOfTheGreenKnight : QuestCard{
		public TestOfTheGreenKnight(QuestMatch match) : base(match) {
			this.name = "Test of the Green Knight";
			this.imageFilename = "quest_test_of_the_green_knight.png";
			this.stages = 4;
			this.questFoes.Add (typeof(GreenKnight));
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
	public class VanquishKingArthursEnemies : QuestCard{
		public VanquishKingArthursEnemies(QuestMatch match) : base(match) {
			this.name = "Vanquish King Arthur's Enemies";
			this.imageFilename = "quest_vanquish_king_arthurs_enemies.png";
			this.stages = 3;
		}
		public override void run(){
			this.match.CurrentStory = this;
		}
	}
}
	