﻿using System;
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
            // TODO.
            get {
                if (this.rank.Value == Players.Rank.Squire) return new Squire(this.match);
                if (this.rank.Value == Players.Rank.Knight) return new Knight(this.match);
                if (this.rank.Value == Players.Rank.ChampionKnight) return new ChampionKnight(this.match);
                return new KnightOfTheRoundTable(this.match);
            }
        }

        public BattleArea BattleArea{
            get { return battleArea; }
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

    public abstract class PlayerBehaviour {
        // TODO: Do we want to abstract quests, tests, and tournaments away from their cards?
        public abstract bool ParticipateInTournament(TournamentCard tournamentCard);
        public abstract List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player);
        public abstract bool SponsorQuest(QuestCard questCard, Hand hand);
        public abstract void SetupQuest(QuestCard questCard, Hand hand);
        public abstract bool ParticipateInQuest(QuestCard questCard, Hand hand);
        public abstract List<AdventureCard> NextBid(TestCard testCard, Hand hand);
        public abstract List<Card> DiscardAfterWinningTest();
		public abstract List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand);

        protected static AdventureCard strongestCard(List<BattleCard> cards) {
            int maxBattlePoints = 0;
            BattleCard maxCard = null;
            foreach (BattleCard card in cards) {
                if (card.BattlePoints > maxBattlePoints) {
                    maxCard = card;
                    maxBattlePoints = card.BattlePoints;
                }
            }
            return maxCard;
        }

        protected bool hasDuplicate(List<BattleCard> cards, BattleCard card) {
            foreach (Card c in cards) {
                if (card.GetType() == c.GetType()) {
                    return true;
                }
            }
            return false;
        }

        // for AI deciding to sponsor quest
        public bool promotableThroughQuest(Player player, QuestCard questCard) {
            return (player.Rank.ShieldsToVictory() <= questCard.StageCount);
        }
		
		// assuming battleCards is sorted, starting from weakest
		public List<BattleCard>[] QuestStageCards(List<BattleCard> battleCards, int size){
			List<BattleCard>[] cardsToPlay = new List<BattleCard>[size];
			List<Amour> yourAmours = new List<Amour>();
			List<AllyCard> yourAllies = new List<AllyCard>();
			List<WeaponCard> yourWeapons = new List<WeaponCard>();
			
			//split your cards up into amour, allies, and weapons
			foreach(BattleCard card in battleCards){
				if(card is Amour){
					yourAmours.Add((Amour)card);
				}
				else if(card is AllyCard){
					yourAllies.Add((AllyCard)card);
				}
				else if(card is WeaponCard){
					yourWeapons.Add((WeaponCard)card);
				}
			}
			//cards to use for the first stage
			if(yourAmours.Count >= 1){
				cardsToPlay[0].Add(yourAmours[0]);
				yourAmours.Remove(yourAmours[0]);
			}
			else if(yourAllies.Count >= 1){
				cardsToPlay[0].Add(yourAllies[0]);
				yourAllies.Remove(yourAllies[0]);
			}
			else if(yourWeapons.Count >= 1){
				cardsToPlay[0].Add(yourWeapons[0]);
				yourWeapons.Remove(yourWeapons[0]);
			}
			//start at stage 2
			for(int i = 1; i < size; i++){
				int previousBP = 0;
				int currentBP = 0;
				foreach(BattleCard card in cardsToPlay[i-1]){
					previousBP += card.BattlePoints;
				}
				while(currentBP - previousBP <= 10){
					if(yourAmours.Count >= 1){
						cardsToPlay[i].Add(yourAmours[0]);
						yourAmours.Remove(yourAmours[0]);
					}
					else if(yourAllies.Count >= 1){
						cardsToPlay[i].Add(yourAllies[0]);
						yourAllies.Remove(yourAllies[0]);
					}
					else if(yourWeapons.Count >= 1){
						cardsToPlay[i].Add(yourWeapons[0]);
						yourWeapons.Remove(yourWeapons[0]);
					}
					else if((yourAmours.Count == 0)
						&&(yourAllies.Count == 0)
						&&(yourWeapons.Count == 0)){
							return null;
						}
				}
			}
			return cardsToPlay;
		}
    }

    internal class HumanPlayer : PlayerBehaviour {
		public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
		
        public override List<Card> DiscardAfterWinningTest() {
            throw new NotImplementedException();
        }

        public override List<AdventureCard> NextBid(TestCard testCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            throw new NotImplementedException();
        }

        public override List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player) {
            throw new NotImplementedException();
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override void SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
    }
}
