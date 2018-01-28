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
        }

        public void Discard(Card card) {
            this.hand.Remove(card);
            this.match.DiscardPile.Push(card);
        }
		
		public void Play(Card card){
			this.hand.Transfer(battleArea, card);
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
