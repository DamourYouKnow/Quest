using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class TournamentCard : InteractiveStoryCard {
		protected int bonusShields;
        protected int maxParticipants;

		public TournamentCard(QuestMatch match) : base(match) {
            this.participants = new List<Player>(match.Players);
            this.maxParticipants = this.participants.Count;
		}

        public int Shields {
			get { return this.participants.Count + this.bonusShields; }
        }

        public override void RequestNextParticipant() {
            foreach  (Player player in this.match.Players) {
                if (player.Behaviour is HumanPlayer) {
                    this.match.Controller.PromptPlayer(player,
                                                       "request_tournament_participation",
                                                       "Would you like to participate in " + this.name,
                                                       image: this.imageFilename);
                }
                else if (player.Behaviour != null) {
                    this.ParticipationResponse(player,
                                               player.Behaviour.ParticipateInTournament(this));
                }
            }
        }

        public override void RequestPlays() {
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
                else if (participant.Behaviour != null) {
                    // Use AI strategy to determine play then Set player to played.
                    List<BattleCard> cards = participant.Behaviour.PlayCardsInTournament(this, participant);
                    participant.Play(cards);
                    this.AddPlayed(participant);
                }
            }
        }

        public override void Resolve() {
            List<Player> winners = this.getWinners();
            if (winners.Count == 0) {
							this.match.Controller.Message(this.match, "No winners for " + this.name);
                this.match.Log("No winners for " + this.name);

            }
            else {
                Player winner = winners[0];
								this.match.Controller.Message(this.match, winner.Username + " has won " + this.name + " with " + Utils.Stringify.CommaList<Card>(winner.BattleArea.Cards));
                this.match.Log(winner.Username + " has won " + this.name);
                winner.Rank.AddShields(this.bonusShields + this.participants.Count);
            }

          	foreach (Player p in participants) {
            	// Discard weapons.
	            List<Card> discardWeapons = p.BattleArea.Cards.FindAll(x => x is WeaponCard);
	            p.BattleArea.Transfer(p.Hand, discardWeapons);
	            p.Discard(discardWeapons);
							// Discard amours.
							List<Card> discardAmours = p.BattleArea.Cards.FindAll(x => x.Name == "Amour");
							p.BattleArea.Transfer(p.Hand, discardAmours);
							p.Discard(discardAmours);
							this.match.Controller.UpdatePlayerArea(p);
						}

            this.match.Controller.UpdatePlayers(this.match);
            this.match.Controller.EndStory(this.match);
        }

        private List<Player> getWinners() {
            return Player.HighestBattlePoints(this.participants);
        }
	}

	public class TournamentAtCamelot : TournamentCard {
		public TournamentAtCamelot(QuestMatch match) : base(match) {
			this.name = "Tournament At Camelot";
			this.imageFilename = "tournament_at_camelot";
			this.bonusShields = 3;
		}
	}

	public class TournamentAtOrkney : TournamentCard {
		public TournamentAtOrkney(QuestMatch match) : base(match) {
			this.name = "Tournament At Orkney";
			this.imageFilename = "tournament_at_orkney";
			this.bonusShields = 2;
		}
	}

	public class TournamentAtTintagle : TournamentCard {
		public TournamentAtTintagle(QuestMatch match) : base(match) {
			this.name = "Tournament At Tintagel";
			this.imageFilename = "tournament_at_tintagel";
			this.bonusShields = 1;
		}
	}

	public class TournamentAtYork : TournamentCard {
		public TournamentAtYork(QuestMatch match) : base(match) {
			this.name = "Tournament At York";
			this.imageFilename = "tournament_at_York";
			this.bonusShields = 0;
		}
	}
}
