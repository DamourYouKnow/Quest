using System;

namespace Quest.Core {
    public enum PlayerRank {
        Squire,
        Knight,
        ChampionKnight,
        KnightOfTheRoundTable
    };

    public class Player {
        private string username;
        private PlayerBehaviour behaviour;
        private RankCard rankCard;
        private int shields;
        private Hand hand;

        public Player(string username) {
            this.username = username;
            this.rankCard = new RankCard();
            this.shields = 0;
            this.hand = new Hand();
        }

        public string Username {
            get { return username; }
        }

        public Hand Hand {
            get { return hand; }
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
