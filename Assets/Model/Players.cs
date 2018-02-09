using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public enum Rank {
        Squire,
        Knight,
        ChampionKnight,
        KnightOfTheRoundTable
    };

    public class PlayerRank {
        private List<RankNode> ranks;
        private int currentRank;
        private int shields;

        public PlayerRank() {
            this.currentRank = 0;
            this.shields = 0;
            ranks = new List<RankNode>();
            ranks.Add(new RankNode(Rank.Squire, 0));
            ranks.Add(new RankNode(Rank.Knight, 5));
            ranks.Add(new RankNode(Rank.ChampionKnight, 7));
            ranks.Add(new RankNode(Rank.KnightOfTheRoundTable, 10));
        }

        public Rank Value {
            get { return this.ranks[currentRank].Value; }
        }

        public int Shields {
            get { return this.shields; }
        }

        public void AddShields(int count) {
            this.shields += count;
            if (this.currentRank == this.ranks.Count - 1) {
                return;
            }

            // Check for promotion.
            int required = this.ranks[currentRank + 1].RequiredShields;
            if (this.shields >= required) {
                currentRank++;
                this.shields = this.shields - required;
            }
        }

        public void RemoveShields(int count) {
            this.shields = Math.Max(0, this.shields - count);
        }

        public int TotalShields() {
            int total = 0;
            for (int i = 0; i < currentRank; i++) {
                total += this.ranks[i].RequiredShields;
            }
            total += this.shields;
            return total;
        }

        public static bool operator<(PlayerRank r1, PlayerRank r2) {
            return r1.currentRank < r2.currentRank;
        }

        public static bool operator>(PlayerRank r1, PlayerRank r2) {
            return r1.currentRank > r2.currentRank;
        }

        public static bool operator<=(PlayerRank r1, PlayerRank r2) {
            return r1.currentRank <= r2.currentRank;
        }

        public static bool operator>=(PlayerRank r1, PlayerRank r2) {
            return r1.currentRank >= r2.currentRank;
        }
    }

    internal class RankNode {
        private Rank value;
        private int requiredShields;

        public RankNode(Rank value, int requiredShields) {
            this.value = value;
            this.requiredShields = requiredShields;
        }

        public Rank Value {
            get { return this.value; }
        }

        public int RequiredShields {
            get { return this.requiredShields; }
        }
    }

    public class Player {
        private QuestMatch match;
        private string username;
        private PlayerBehaviour behaviour;
        private PlayerRank rank;
        private Hand hand;
		private BattleArea battleArea;

        public Player(string username, QuestMatch match=null) {
            this.match = match;
            this.username = username;
            this.rank = new PlayerRank();
            this.hand = new Hand();
			this.battleArea = new BattleArea();
        }

		public string Username {
			get { return username; }
		}

		public QuestMatch Match {
			get { return match; }
		}

        public Hand Hand {
            get { return hand; }
        }

        public PlayerRank Rank {
            get { return this.rank; }
        }
		
		public BattleArea BattleArea{
            get { return battleArea; }
        }

        public RankCard RankCard {
            // TODO.
            get {
                return null;
            }
        }
	
        public void Draw(Deck deck, int count=1) {
            for (int i = 0; i < count; i++) {
                Card drawnCard = deck.Draw();
                this.Hand.Add(drawnCard);
                this.match.Log("Player " + this.username + " drew " + drawnCard.ToString());
            }
			
			//not sure how to fully implement this 'limit' right now.
			//player needs to choose whether to discard or play,
			//if play, players needs to choose which cards,
			//and some cards are unplayable.
			
			if (this.hand.Count > Constants.MaxHandSize){
				for (int i = this.hand.Count; i>Constants.MaxHandSize; i--){
					//discards the most recently drawn cards
					//change later
					Discard(this.hand.Cards[i]);
				}
			}
			
			
        }

        public void Discard(Card card) {
            this.hand.Remove(card);
            this.match.DiscardPile.Push(card);
            this.match.Log("Player " + this.username + " discarded " + card.ToString());
        }

        public void Discard(List<Card> cards) {
            foreach (Card card in cards) {
                this.Discard(card);
            }
        }

        public Boolean CardInPlay(Card card) {
            return this.battleArea.Cards.Contains(card);
        }

        public Boolean CardInHand(Card card) {
            return this.hand.Cards.Contains(card);
        }
		
		public void Play(Card card){
			this.hand.Transfer(battleArea, card);
            //will need to check if a card is playable or not
            //(that might be handled elsewhere, not in this function (not sure))

            this.match.Log("Player " + this.username + " played " + card.ToString());
		}

        public static List<Player> LowestShields(List<Player> players) {
            List<Player> minList = new List<Player>();
            if (players.Count == 0) return minList;

            int min = players[0].Rank.TotalShields();
            foreach (Player player in players) {
                if (player.rank.TotalShields() < min) {
                    min = player.rank.TotalShields();
                }
            }

            foreach (Player player in players) {
                if (player.rank.TotalShields() == min) {
                    minList.Add(player);
                }
            }

            return minList;
        }

        public static List<Player> HighestShields(List<Player> players) {
            List<Player> maxList = new List<Player>();

            int max = 0;
            foreach (Player player in players) {
                if (player.rank.TotalShields() > max) {
                    max = player.rank.TotalShields();
                }
            }

            foreach (Player player in players) {
                if (player.rank.TotalShields() == max) {
                    maxList.Add(player);
                }
            }

            return maxList;
        }

        public static List<Player> LowestRanked(List<Player> players) {
            List<Player> minList = new List<Player>();
            if (players.Count == 0) return minList;

            PlayerRank min = players[0].Rank;
            foreach (Player player in players) {
                if (player.rank < min) {
                    min = player.rank;
                }
            }

            foreach (Player player in players) {
                if (player.rank.Value == min.Value) {
                    minList.Add(player);
                }
            }

            return minList;
        }

        public static List<Player> HighestRanked(List<Player> players) {
            List<Player> maxList = new List<Player>();

            PlayerRank max = new PlayerRank();
            foreach (Player player in players) {
                if (player.rank > max) {
                    max = player.rank;
                }
            }

            foreach (Player player in players) {
                if (player.rank.Value == max.Value) {
                    maxList.Add(player);
                }
            }

            return maxList;
        }
    }

    internal abstract class PlayerBehaviour {
        // TODO: Do we want to abstract quests, tests, and tournaments away from their cards?
        public abstract bool ParticipateInTournament(TournamentCard tournamentCard);
        public abstract List<AdventureCard> PlayCardsInTournament(TournamentCard TournamentCard, Hand hand);
        public abstract bool SponsorQuest(QuestCard questCard);
        public abstract bool ParticipateInQuest(QuestCard questCard);
        public abstract List<Card> NextBid(TestCard testCard, Hand hand);
        public abstract List<Card> DiscardAfterWinningTest();

        protected static AdventureCard StrongestCard(List<AdventureCard> cards) {
            int maxBattlePoints = 0;
            AdventureCard maxCard = null;
            foreach (AdventureCard card in cards) {
                if (card.BattlePoints > maxBattlePoints) {
                    maxCard = card;
                    maxBattlePoints = card.BattlePoints;
                }
            }
            return maxCard;
        }
    }

    internal class HumanPlayer : PlayerBehaviour {
        public override List<Card> DiscardAfterWinningTest() {
            throw new NotImplementedException();
        }

        public override List<Card> NextBid(TestCard testCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInQuest(QuestCard questCard) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            throw new NotImplementedException();
        }

        public override List<AdventureCard> PlayCardsInTournament(TournamentCard TournamentCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool SponsorQuest(QuestCard questCard) {
            throw new NotImplementedException();
        }
    }
}
