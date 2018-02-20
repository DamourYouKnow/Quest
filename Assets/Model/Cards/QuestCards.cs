using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class QuestCard:StoryCard {
		protected int numStages;
		protected int currentStage;
		protected Player sponsor;
		protected List<Player> questingPlayers;
		protected List<QuestArea> stages;
		protected List<Type> questFoes;

		public List<QuestArea> Stages {
			get { return this.stages; }
			set {
				if (value.Count == numStages) {
					this.stages = value;
				} else {
					this.match.Log("Invalid number of stages.");
				}
			}
		}
		public List<Player> QuestingPlayers {
			get { return this.questingPlayers; }
			set { this.questingPlayers = value; }
		}
		public int CurrentStage {
			get { return this.currentStage; }
		}
		public Player Sponsor {
			get { return this.sponsor; }
			set { this.sponsor = value; }
		}
		public List<Type> QuestFoes {
			get { return this.questFoes; }
		}

		public QuestCard(QuestMatch match):base(match) {
			this.questFoes = new List<Type> ();
			this.currentStage = 0;
			this.sponsor = null;
			this.questingPlayers = null;
		}
		public override void Run (){
			if (this.sponsor == null ||
			   this.questingPlayers == null ||
			   this.stages == null) {

				this.sponsor = null;
				this.questingPlayers = null;
				this.stages = null;

				this.match.CurrentStory = this;
			}
			else {
				this.currentStage = 1;
			}
		}
		public List<Player> ResolveStage(){
			List<Player> winners = new List<Player>();
			//TODO: Implement Test stage.
			if (this.stages [currentStage-1].MainCard is FoeCard) {
				foreach (var p in questingPlayers) {
					if (p.BattleArea.BattlePoints() >= this.stages [currentStage-1].BattlePoints()) {
						winners.Add (p);
					}
					p.BattleArea.Cards.Clear ();
				}
			}
			this.questingPlayers = winners;
			this.currentStage += 1;

			//If no more stages or no more players, resolve quest.
			if (this.questingPlayers.Count == 0
				|| this.currentStage > this.numStages) {
				foreach (var p in this.questingPlayers) {
					p.Rank.AddShields (this.numStages);
				}
				int numDraw = 0;
				foreach (var item in this.stages) {
					numDraw += item.Count;
					this.match.Log (numDraw.ToString ());
				}
				numDraw += this.numStages;
				this.match.Log (numDraw.ToString ());
				this.sponsor.Draw (this.match.AdventureDeck, numDraw);
			}
			return winners;
		}
	}
	public class BoarHunt : QuestCard{
		public BoarHunt(QuestMatch match) : base(match) {
			this.name = "Boar Hunt";
			this.imageFilename = "quest_boar_hunt.png";
			this.numStages = 2;
			this.questFoes.Add (typeof(Boar));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class DefendTheQueensHonor : QuestCard{
		public DefendTheQueensHonor(QuestMatch match) : base(match) {
			this.name = "Defend The Queen's Honor";
			this.imageFilename = "quest_defend_the_queens_honor.png";
			this.numStages = 4;
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
		public override void Run(){
			base.Run ();
		}
	}
	public class JourneyThroughTheEnchantedForest : QuestCard{
		public JourneyThroughTheEnchantedForest(QuestMatch match) : base(match) {
			this.name = "Journey Through The Enchanted Forest";
			this.imageFilename = "quest_journey_through_the_enchanted_forest.png";
			this.numStages = 3;
			this.questFoes.Add (typeof(EvilKnight));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class RepelTheSaxonRaiders : QuestCard{
		public RepelTheSaxonRaiders(QuestMatch match) : base(match) {
			this.name = "Repel The Saxon Raiders";
			this.imageFilename = "quest_repel_the_saxon_raiders.png";
			this.numStages = 2;
			this.questFoes.Add (typeof(SaxonKnight));
			this.questFoes.Add (typeof(Saxons));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class RescueTheFairMaiden : QuestCard{
		public RescueTheFairMaiden(QuestMatch match) : base(match) {
			this.name = "Rescue The Fair Maiden";
			this.imageFilename = "quest_rescue_the_fair_maiden.png";
			this.numStages = 3;
			this.questFoes.Add (typeof(BlackKnight));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class SearchForTheHolyGrail : QuestCard{
		public SearchForTheHolyGrail(QuestMatch match) : base(match) {
			this.name = "Search For The Holy Grail";
			this.imageFilename = "quest_search_for_the_holy_grail.png";
			this.numStages = 5;
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
		public override void Run(){
			base.Run ();
		}
	}
	public class SearchForTheQuestingBeast : QuestCard{
		public SearchForTheQuestingBeast(QuestMatch match) : base(match) {
			this.name = "Search For The Questing Beast";
			this.imageFilename = "quest_search_for_the_questing_beast";
			this.numStages = 4;
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class SlayTheDragon : QuestCard{
		public SlayTheDragon(QuestMatch match) : base(match) {
			this.name = "Slay The Dragon";
			this.imageFilename = "quest_slay_the_dragon.png";
			this.stages = new List<QuestArea> (3);
			this.questFoes.Add (typeof(Dragon));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class TestOfTheGreenKnight : QuestCard{
		public TestOfTheGreenKnight(QuestMatch match) : base(match) {
			this.name = "Test Of The Green Knight";
			this.imageFilename = "quest_test_of_the_green_knight.png";
			this.numStages = 4;
			this.questFoes.Add (typeof(GreenKnight));
		}
		public override void Run(){
			base.Run ();
		}
	}
	public class VanquishKingArthursEnemies : QuestCard{
		public VanquishKingArthursEnemies(QuestMatch match) : base(match) {
			this.name = "Vanquish King Arthur's Enemies";
			this.imageFilename = "quest_vanquish_king_arthurs_enemies.png";
			this.numStages = 3;
		}
		public override void Run(){
			base.Run ();
		}
	}
}
	