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

        public Player(QuestMatch match, string username) {
            this.match = match;
            this.username = username;
            this.rankCard = new RankCard(match);
            this.shields = 0;
            this.hand = new Hand();
        }

        public string Username {
            get { return username; }
        }

        public Hand Hand {
            get { return hand; }
        }

        public void Promote() {

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
