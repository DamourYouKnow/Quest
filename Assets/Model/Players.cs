using System;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public enum PlayerRank {
        Squire,
        Knight,
        ChampionKnight,
        KnightOfTheRoundTable
    };

    public class Player {
        private QuestMatch match;
        private string username;
        private PlayerBehaviour behaviour;
        private RankCard rankCard;
        private int shields;
        private Hand hand;
		private BattleArea battleArea;

        public Player(QuestMatch match, string username) {
            this.match = match;
            this.username = username;
            this.rankCard = new RankCard(match);
            this.shields = 0;
            this.hand = new Hand();
			this.battleArea = new BattleArea();
        }

        public string Username {
            get { return username; }
        }

        public Hand Hand {
            get { return hand; }
        }

        public void Promote() {

        }

        public void Draw(Deck deck, int count=1) {
            for (int i = 0; i < count; i++) {
                this.Hand.Add(deck.Draw());
            }
			
			//not sure how to implement this 'limit' right now.
			//player needs to choose whether to discard or play,
			//if play, players needs to choose which cards,
			//and some cards are unplayable
			if (this.hand.Count > maxHandSize){
				for (int i = this.hand.Count; i>maxHandSize; i--){
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
