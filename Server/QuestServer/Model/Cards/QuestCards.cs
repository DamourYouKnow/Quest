using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class QuestCard: InteractiveStoryCard {
		protected int numStages;
		protected int currentStage;
		protected Player sponsor;
		protected List<QuestArea> stages;
        protected QuestArea stageBuilder;
		protected List<Type> questFoes;
        protected Dictionary<Player, Stack<List<BattleCard>>> battleHistory;
        private int nextParticipant = 0;

        public QuestCard(QuestMatch match) : base(match) {
            this.questFoes = new List<Type>();
            this.currentStage = 1;
            this.sponsor = null;
            this.stages = new List<QuestArea>();
            this.battleHistory = new Dictionary<Player, Stack<List<BattleCard>>>();
        }

        public List<QuestArea> Stages {
            get { return this.stages; }
			set { this.stages = value; }
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

        public QuestArea StageBuilder {
            get { return this.stageBuilder; }
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

        public void AddStage(QuestArea area) { 
            if (this.stages.Count >= this.numStages) throw new Exception("Quest stage limit exceeded");
            this.stages.Add(area);
            this.match.Log(this.sponsor.Username + " adding stage with " + Utils.Stringify.CommaList(area.Cards));
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

        public override void RequestNextParticipant() {
            Player player = this.match.Players[this.nextParticipant];
            this.nextParticipant++;

            if (player == this.sponsor) {
                this.RequestNextParticipant();
                return;
            }
            
            if (player.Behaviour is HumanPlayer) {
                // Send participation request to player through sockets.
                this.match.Controller.PromptPlayer(player,
                                                   "request_quest_participation",
                                                   "Would you like to participate in " + this.name,
                                                   image: this.imageFilename);
            }
            else if (player.Behaviour != null) {
                // Use strategies to determine player participation.
                this.ParticipationResponse(player,
                                 player.Behaviour.ParticipateInQuest(this, player.Hand));
            }
        }

        public override void ParticipationResponse(Player player, bool participating) {
            this.responded.Add(player);
            if (participating) {
                this.match.Controller.Message(this.match, player.Username + " participating in " + this.name);
                this.participants.Add(player);
                player.Draw(this.match.AdventureDeck);
                this.battleHistory.Add(player, new Stack<List<BattleCard>>());
            }
            else {
                this.match.Controller.Message(this.match, player.Username + " not participating in" + this.name);
            }

            if (this.responded.Count == this.match.Players.Count) {
                this.SetupNextStage();
            } else {
                this.RequestNextParticipant();
            }
        }

        public void SetupNextStage() {
            if (this.participants.Count == 0) {
                this.Resolve();
                return;
            }

            if (this.sponsor.Behaviour is HumanPlayer) {
                this.stageBuilder = new QuestArea();
                this.match.Controller.RequestStage(this.sponsor);
            }
            else if (this.sponsor.Behaviour != null) {
                // Player behaviour functions for individual stage setup.
                List<AdventureCard>[] stages = this.sponsor.Behaviour.SetupQuest(this, this.sponsor.Hand);
                foreach (List<AdventureCard> stage in stages) {
                    if (stage.Count == 1 && stage[0] is TestCard) {
                        this.AddTestStage((TestCard)stage[0]);
                    }
                    else {
                        FoeCard foe = (FoeCard)stage.Find(x => x is FoeCard);
                        List<WeaponCard> weapons = stage.FindAll(x => x is WeaponCard).Cast<WeaponCard>().ToList();
                        this.AddFoeStage(foe, weapons);
                    }
                }
            }
        }

        public void StageResponse() {
            if (this.stages.Count == this.StageCount) {
                this.RequestPlays();
            } else {
                this.SetupNextStage();
            }
        }

        public override void RequestPlays() {
            // FIXME: Does this require adding the main card to the list?
            this.match.Controller.UpdateOtherArea(this.match, this.stages[this.currentStage - 1].Cards);

            if (this.participants.Count == 0) {
                this.Resolve();
                return;
            }

            foreach (Player participant in this.participants) {
                if (participant.Behaviour is HumanPlayer) {
                    // Send play request to player through sockets.
                    this.match.Controller.PromptPlayer(participant,
                                                       "request_play_cards",
                                                       "Please play your cards");

                }
                else if (participant.Behaviour != null){
                    // Use AI strategy to determine play then Set player to played.
                    List<BattleCard> cards = participant.Behaviour.PlayCardsInQuest(this, participant.Hand);
                    participant.Play(cards);
                    this.AddPlayed(participant);
                }
            }
        }

        // We need some extra code to wrap around the sponsorship requests.
        public void RequestSponsorship() {
            Player currentPlayer = this.match.CurrentPlayer;

            if (currentPlayer.Behaviour is HumanPlayer) {
                // If human send prompt request.
                this.match.Controller.PromptPlayer(currentPlayer,
                                                   "request_quest_sponsor",
                                                   "Would you like to sponsor " + this.name,
                                                   image: this.imageFilename);
            }
            else  if (currentPlayer.Behaviour != null) {
                // Otherwise decide with strategy.
                bool sponsor = currentPlayer.Behaviour.SponsorQuest(this, currentPlayer.Hand);
                this.SponsorshipResponse(currentPlayer, sponsor);
            }
        }

        /// <summary>
        /// This should be called when a player sponsors a quest.
        /// If sponsor if true, then the stages should be added before calling this.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="sponsor"></param>
        public void SponsorshipResponse(Player player, bool sponsor) {
            if (sponsor) {
                this.match.Controller.Message(this.match, player.Username + " sponsored " + this.name);
                this.match.Log("Quest sponsored");
                this.sponsor = player;
                this.responded.Add(player);
                this.RequestNextParticipant();
            }
            else {
                this.match.Controller.Message(this.match, player.Username + " did not sponsor " + this.name);
                this.match.Log("Quest not sponsored");
                this.match.Controller.EndStory(this.match);
            }
        }

        public override void Run() {
            this.RequestSponsorship();
        }

		public override void Resolve(){
            if (this.stages.Count == 0) {
                this.match.Log("Quest ending since nobody sponsored");

                int numDraw = 0;
                foreach (var item in this.stages) {
                    numDraw += item.Count;
                }
                numDraw += this.numStages;
                this.sponsor.Draw(this.match.AdventureDeck, numDraw);

                this.match.Controller.EndStory(this.match);
                return;
            }

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

            if (this.participants.Count > 0) {
                this.match.Log(Utils.Stringify.CommaList<Player>(this.participants) + " have won stage " + this.currentStage);
            }
            else {
                this.match.Log("Stage " + this.currentStage + " has no winners");
            }

            //If no more stages or no more players, resolve quest.
            if (this.participants.Count == 0 || this.currentStage > this.numStages) {
				foreach (var p in this.participants) {
					p.Rank.AddShields (this.numStages);
				}
				int numDraw = 0;
				foreach (var item in this.stages) {
					numDraw += item.Count;
				}
				numDraw += this.numStages;
				this.sponsor.Draw (this.match.AdventureDeck, numDraw);
                this.match.Controller.EndStory(this.match);
            }
			else {
				foreach (Player p in (this.match.CurrentStory as QuestCard).participants) {
					p.Draw (this.match.AdventureDeck);
				}

                this.participated.Clear();

                // Request plays for next round.
                this.RequestPlays();
			}
		}
	}

	public class BoarHunt : QuestCard{
		public BoarHunt(QuestMatch match) : base(match) {
			this.name = "Boar Hunt";
			this.imageFilename = "quest_boar_hunt";
			this.numStages = 2;
			this.questFoes.Add (typeof(Boar));
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
	}

	public class JourneyThroughTheEnchantedForest : QuestCard{
		public JourneyThroughTheEnchantedForest(QuestMatch match) : base(match) {
			this.name = "Journey Through The Enchanted Forest";
			this.imageFilename = "quest_journey_through_the_enchanted_forest";
			this.numStages = 3;
			this.questFoes.Add (typeof(EvilKnight));
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
	}

	public class RescueTheFairMaiden : QuestCard{
		public RescueTheFairMaiden(QuestMatch match) : base(match) {
			this.name = "Rescue The Fair Maiden";
			this.imageFilename = "quest_rescue_the_fair_maiden";
			this.numStages = 3;
			this.questFoes.Add (typeof(BlackKnight));
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
	}

	public class SearchForTheQuestingBeast : QuestCard{
		public SearchForTheQuestingBeast(QuestMatch match) : base(match) {
			this.name = "Search For The Questing Beast";
			this.imageFilename = "quest_search_for_the_questing_beast";
			this.numStages = 4;
		}
	}

	public class SlayTheDragon : QuestCard{
		public SlayTheDragon(QuestMatch match) : base(match) {
			this.name = "Slay The Dragon";
			this.imageFilename = "quest_slay_the_dragon";
			this.numStages = 3;
			this.questFoes.Add (typeof(Dragon));
		}
	}

	public class TestOfTheGreenKnight : QuestCard{
		public TestOfTheGreenKnight(QuestMatch match) : base(match) {
			this.name = "Test Of The Green Knight";
			this.imageFilename = "quest_test_of_the_green_knight";
			this.numStages = 4;
			this.questFoes.Add (typeof(GreenKnight));
		}
	}

	public class VanquishKingArthursEnemies : QuestCard{
		public VanquishKingArthursEnemies(QuestMatch match) : base(match) {
			this.name = "Vanquish King Arthur's Enemies";
			this.imageFilename = "quest_vanquish_king_arthurs_enemies";
			this.numStages = 3;
		}
	}
}
	