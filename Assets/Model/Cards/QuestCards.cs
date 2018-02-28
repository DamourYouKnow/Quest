using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class QuestCard:StoryCard {
		protected int numStages;
		protected int currentStage;
		protected Player sponsor;
		protected List<Player> participants;
		protected List<QuestArea> stages;
		protected List<Type> questFoes;
        protected Dictionary<Player, Stack<List<BattleCard>>> battleHistory;

        public QuestCard(QuestMatch match) : base(match) {
            this.questFoes = new List<Type>();
            this.currentStage = 1;
            this.sponsor = null;
            this.stages = new List<QuestArea>();
            this.participants = new List<Player>();
            this.battleHistory = new Dictionary<Player, Stack<List<BattleCard>>>();
        }

        public List<QuestArea> Stages {
            get { return this.stages; }
        }

		public List<Player> Participants {
			get { return this.participants; }
            set { this.participants = value; }
		}

		public int CurrentStage {
			get { return this.currentStage; }
		}
		
		public int StageCount {
			get { return this.numStages; }
		}

		public Player Sponsor {
			get { return this.sponsor; }
			set { this.sponsor = value; }
		}

		public List<Type> QuestFoes {
			get { return this.questFoes; }
		}

        public void AddParticipant(Player player) {
            this.match.Log("Added " + player.ToString() + " as " + this.ToString() + " participant");
            this.participants.Add(player);
            this.battleHistory.Add(player, new Stack<List<BattleCard>>());
        }

        public void AddFoeStage(FoeCard foe, List<WeaponCard> weapons = null) {
            if (this.stages.Count >= this.numStages) throw new Exception("Quest stage limit exceeded");

            QuestArea area = new QuestArea(foe);
            this.sponsor.Hand.Transfer(area, foe);
            if (weapons != null) {
                foreach (WeaponCard weapon in weapons) {
                    this.sponsor.Hand.Transfer(area, weapon);
                }
            }

            this.stages.Add(area);

            string stageString = foe.ToString();
            if (weapons != null && weapons.Count > 0) stageString += " with weapons " + Utils.Stringify.CommaList(weapons);
            this.match.Log(this.sponsor.Username + " adding stage " + stageString);
        }

        public void AddTestStage(TestCard test) {
            if (this.stages.Count >= this.numStages) throw new Exception("Quest stage limit exceeded");

            QuestArea area = new QuestArea(test);
            this.sponsor.Hand.Transfer(area, test);
            this.stages.Add(area);
            this.match.Log(this.sponsor.Username + " adding stage " + test.ToString() + " to " + this.name);
        }

        public QuestArea GetStage(int stageNumber) {
            return this.stages[stageNumber - 1];
        }

        public List<BattleCard> GetLastHistory(Player player) {
            Stack<List<BattleCard>> historyStack = this.battleHistory[player];
            if (historyStack.Count > 0) {
                return new List<BattleCard>(historyStack.Peek());
            }
            return new List<BattleCard>();
        }

        public List<BattleCard> GetFullHistory(Player player) {
            List<BattleCard> historyList = new List<BattleCard>();
            Stack<List<BattleCard>> historyStack = this.battleHistory[player];
            foreach (List<BattleCard> cards in historyStack) {
                historyList.AddRange(cards);
            }
            return historyList;
        }

        public void ForceStage(int stageNumber) {
            this.currentStage = stageNumber;
        }
		public void requestSponsor(){
			this.match.Log ("Requesting Sponsor");
			this.match.State = MatchState.REQUEST_SPONSOR;
			this.match.Wait ();
		}
		public void requestParticipation(){
			this.match.Log ("Requesting Participants");
			this.match.State = MatchState.REQUEST_PARTICIPANTS;

			int i = 0;
			for (; i < this.match.Players.Count; i++) {
				if (this.match.Players [i] == this.Sponsor) {
					break;
				}
			}

			this.match.PromptingPlayer = (i + 1)%this.match.Players.Count;
			this.match.Wait ();
		}
		public void requestStage(){
			this.match.State = MatchState.REQUEST_STAGE;
			this.match.Wait ();
		}
		public void RunStage(){
			this.match.State = MatchState.RUN_STAGE;
			this.match.Wait ();
		}
		public override void Run() {
			if (this.sponsor == null) {
				this.requestSponsor ();
			}
			else if (this.stages.Count < this.numStages) {
				requestStage ();
			}
			else {
				int i = 0;
				for (; i < this.match.Players.Count; i++) {
					if (this.match.Players [i] == this.Sponsor) {
						break;
					}
				}
				this.match.PromptingPlayer = (i + 1)%this.match.Players.Count;
				foreach (Player p in this.match.Players){
					if (p != this.sponsor){
						p.Draw (this.match.AdventureDeck, 1);
					}
				}
				this.currentStage = 1;
				this.RunStage ();
			}
			/*
            // Player behaviour functions for individual stage setup.
            List<AdventureCard>[] stages = currentPlayer.Behaviour.SetupQuest(this, this.sponsor.Hand);
            foreach (List<AdventureCard> stage in stages) {
                if (stage.Count == 1 && stage[0] is TestCard) {
                    this.AddTestStage((TestCard)stage[0]);
                } else {
                    FoeCard foe = (FoeCard)stage.Find(x => x is FoeCard);
                    List<WeaponCard> weapons = stage.FindAll(x => x is WeaponCard).Cast<WeaponCard>().ToList();
                    this.AddFoeStage(foe, weapons);
                }
            }
           
            while (this.currentStage <= this.numStages) {
                // Participants play cards.
                foreach (Player participant in this.participants) {
                    participant.Play(participant.Behaviour.PlayCardsInQuest(this, participant.Hand));
                }

                // Resolve.
                List<Player> winners = this.ResolveStage();
                this.match.Log(Utils.Stringify.CommaList<Player>(winners) + " have won stage " + this.numStages);
                this.currentStage++;
            }
            
			*/
            // TODO: Clean up everything.
		}

		public List<Player> ResolveStage(){
			List<Player> winners = new List<Player>();
            if (this.stages[currentStage - 1].MainCard is TestCard) {
                // TODO: Implement Test stage.
                throw new NotImplementedException();
            }
            if (this.stages [currentStage-1].MainCard is FoeCard) {
				foreach (var p in participants) {
                    // Update history.
                    this.battleHistory[p].Push(p.BattleArea.BattleCards);

					if (p.BattleArea.BattlePoints() >= this.stages [currentStage-1].BattlePoints()) {
						winners.Add (p);
					}

                    // Discard weapons.
                    List<Card> discardWeapons = p.BattleArea.Cards.FindAll(x => x is WeaponCard);
                    p.BattleArea.Transfer(p.Hand, discardWeapons);
                    p.Discard(discardWeapons);
				}
			}
			this.participants = winners;
			this.currentStage += 1;

			//If no more stages or no more players, resolve quest.
			if (this.participants.Count == 0
			    || this.currentStage > this.numStages) {
				foreach (var p in this.participants) {
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
			else {
				this.match.State = MatchState.RESOLVE_STAGE;
				this.match.Wait ();
			}
			return winners;
		}
	}

	public class BoarHunt : QuestCard{
		public BoarHunt(QuestMatch match) : base(match) {
			this.name = "Boar Hunt";
			this.imageFilename = "quest_boar_hunt";
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
			this.imageFilename = "quest_defend_the_queens_honor";
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
			this.imageFilename = "quest_journey_through_the_enchanted_forest";
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
			this.imageFilename = "quest_repel_the_saxon_raiders";
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
			this.imageFilename = "quest_rescue_the_fair_maiden";
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
			this.imageFilename = "quest_search_for_the_holy_grail";
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
			this.imageFilename = "quest_slay_the_dragon";
			this.numStages = 3;
			this.questFoes.Add (typeof(Dragon));
		}

		public override void Run(){
			base.Run ();
		}
	}

	public class TestOfTheGreenKnight : QuestCard{
		public TestOfTheGreenKnight(QuestMatch match) : base(match) {
			this.name = "Test Of The Green Knight";
			this.imageFilename = "quest_test_of_the_green_knight";
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
			this.imageFilename = "quest_vanquish_king_arthurs_enemies";
			this.numStages = 3;
		}

		public override void Run(){
			base.Run ();
		}
	}
}
	