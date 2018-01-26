using System;
using System.Collections.Generic;

namespace Quest.Core {
    public static class Constants {
        public const int MaxHandSize = 12;
    }

    public class GameManager {
        private Logger logger;
        private Board board;
        private List<Player> players;
        
        public GameManager() {
            this.logger = new Logger();
            this.board = new Board();
            this.players = new List<Player>();
        }

        public Board Board {
            get { return this.Board; }
        }

        public void AddPlayer(Player player) {
            this.players.Add(player);
            this.logger.Log("Player " + player.Username + " added to game");
        }

        public void Setup() {
            // Deal startingHandSize adventure cards to each player.
            foreach (Player player in this.players) {
                this.board.AdventureDeck.Deal(player, Constants.MaxHandSize);
            }
        }
    }


    /// <summary>
    /// Play area.
    /// </summary>
    public class Board {
        private RankDeck rankDeck;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;

        public Board() {
            this.rankDeck = new RankDeck();
            this.storyDeck = new StoryDeck();
            this.adventureDeck = new AdventureDeck();
            this.discardPile = new DiscardPile();
        }

        public Deck RankDeck {
            get { return this.rankDeck; }
        }

        public Deck StoryDeck {
            get { return this.storyDeck; }
        }

        public Deck AdventureDeck {
            get { return this.adventureDeck; }
        }

        public Deck DiscardPile {
            get { return this.discardPile; }
        }
    }

    // Area containing a collection of cards.
    public abstract class CardArea {
        protected List<Card> cards = new List<Card>();

        public int Count {
            get { return this.cards.Count; }
        }

        public List<Card> Cards {
            get { return this.cards; }
        }

        public void Add(Card card) {
            this.cards.Add(card);
        }

        public void Transfer(CardArea target, Card card) {
            target.cards.Add(card);
            this.cards.Remove(card);
        }
    }

    /// <summary>
    /// Battle area on a game board.
    /// </summary>
    public class BattleArea : CardArea {

    }

    /// <summary>
    /// Card hand beloning to a player.
    /// </summary>
    public class Hand : CardArea {

    }
}