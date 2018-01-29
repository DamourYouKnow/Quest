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
            return r1.TotalShields() < r2.TotalShields();
        }

        public static bool operator>(PlayerRank r1, PlayerRank r2) {
            return r1.TotalShields() > r2.TotalShields();
        }

        public static bool operator<=(PlayerRank r1, PlayerRank r2) {
            return r1.TotalShields() <= r2.TotalShields();
        }

        public static bool operator>=(PlayerRank r1, PlayerRank r2) {
            return r1.TotalShields() >= r2.TotalShields();
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

        public Hand Hand {
            get { return hand; }
        }

        public PlayerRank Rank {
            get { return this.rank; }
        }
	
        public void Draw(Deck deck, int count=1) {
            for (int i = 0; i < count; i++) {
                this.Hand.Add(deck.Draw());
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
        }
		
		public void Play(Card card){
			this.hand.Transfer(battleArea, card);
			//will need to check if a card is playable or not
			//(that might be handled elsewhere, not in this function (not sure))
		}

        public static List<Player> LowestShields(List<Player> players) {
            List<Player> minList = new List<Player>();

            int min = Int32.MaxValue;
            foreach (Player player in players) {
                if (player.rank.TotalShields() < min) {
                    min = player.rank.TotalShields();
                }
            }

            foreach (Player player in players) {
                if (player.Rank.TotalShields() == min) {
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
                if (player.Rank.TotalShields() == max) {
                    maxList.Add(player);
                }
            }

            return maxList;
        }
    }

    internal abstract class PlayerBehaviour {
        public abstract void HandleTurn();
    }

    internal class HumanPlayer : PlayerBehaviour {
        public override void HandleTurn() {
            throw new NotImplementedException();
        }
    }

    internal class SimpleBot : PlayerBehaviour {
        public override void HandleTurn() {
            throw new NotImplementedException();
        }
    }
}
