using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public enum Rank {
        Squire,
        Knight,
        ChampionKnight,
        KnightOfTheRoundTable
    };

    public class PlayerRank {
        private Player player;
        private List<RankNode> ranks;
        private int currentRank;
        private int shields;

        public PlayerRank(Player player = null) {
            this.player = player;
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

        public RankCard RankCard {
            get { return this.ranks[currentRank].RankCard; }
        }

        public int Shields {
            get { return this.shields; }
        }

        public override string ToString() {
            return this.RankCard.ToString();
        }

        public void AddShields(int count) {
            this.player.Match.Log("Adding " + count + " shields to " + this.player.ToString());
 
            while (count > 0) {
                this.addShield();
                count--;
            }
        }

        private void addShield() {
            int count = 1;
            this.shields += count;
            this.player.Match.Log("Adding " + count + " shields to " + this.player.ToString());
            if (this.currentRank == this.ranks.Count - 1) {
                return;
            }

            // Check for promotion.
            int required = this.ranks[currentRank + 1].RequiredShields;
            if (this.shields >= required) {
                currentRank++;
                this.player.Match.Log(this.player.ToString() + " promoted to " + this.ToString());
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

        public int ShieldsToNextPromotion() {
            if (this.currentRank < this.ranks.Count - 1) {
                return this.ranks[this.currentRank + 1].RequiredShields - this.shields;
            } else {
                return 0;
            }
        }

        public int ShieldsToVictory() {
            int requiredShields = this.ShieldsToNextPromotion();
            for (int nextRank = this.currentRank + 2; nextRank < this.ranks.Count - 1; nextRank++) {
                requiredShields += this.ranks[nextRank].RequiredShields;
            }
            return requiredShields;
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

        public RankCard RankCard {
            get {
                if (this.value == Players.Rank.Squire) return new Squire(null);
                if (this.value == Players.Rank.Knight) return new Knight(null);
                if (this.value == Players.Rank.ChampionKnight) return new ChampionKnight(null);
                return new KnightOfTheRoundTable(null);
            }
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
		private PlayerArea battleArea;

        public Player(string username, QuestMatch match=null) {
            this.match = match;
            this.username = username;
            this.rank = new PlayerRank(this);
            this.hand = new Hand(this);
			this.battleArea = new PlayerArea();
        }

		public string Username {
			get { return username; }
		}

		public QuestMatch Match {
			get { return match; }
		}

        public PlayerBehaviour Behaviour {
            get { return this.behaviour; }
            set { this.behaviour = value; }
        }

        public Hand Hand {
            get { return hand; }
        }

        public PlayerRank Rank {
            get { return this.rank; }
        }

        public RankCard RankCard {
            get { return this.rank.RankCard; }
        }

        public PlayerArea BattleArea{
            get { return battleArea; }
        }

        public override string ToString() {
            return this.username;
        }
	
        public void Draw(Deck deck, int count=1) {
            for (int i = 0; i < count; i++) {
                Card drawnCard = deck.Draw();
                this.Hand.Add(drawnCard);
                this.match.Log("Player " + this.username + " drew " + drawnCard.ToString());
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

        public int BattlePointsInPlay() {
            return this.battleArea.BattlePoints() + this.RankCard.BattlePoints;
        }

        public Boolean CardInPlay(Card card) {
            return this.battleArea.Cards.Contains(card);
        }

        public Boolean CardInHand(Card card) {
            return this.hand.Cards.Contains(card);
        }
		
		public void Play(BattleCard card){
			this.hand.Transfer(this.battleArea, card);
            //will need to check if a card is playable or not
            //(that might be handled elsewhere, not in this function (not sure))

            this.match.Log("Player " + this.username + " played " + card.ToString());
		}

        public void Play(List<BattleCard> cards) {
            this.hand.Transfer(this.battleArea, cards.Cast<Card>().ToList());
            this.match.Log("Player " + this.username + " played " + Utils.Stringify.CommaList(cards));
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
}
