using System;
using System.Collections.Generic;

namespace Quest.Core {

    public class GameManager {
        private Board board;
        private List<Player> players;
        
        public GameManager() {
            this.board = new Board();
            this.players = new List<Player>();
        }

        public Board Board {
            get { return this.Board; }
        }

        public void AddPlayer(Player player) {
            this.players.Add(player);
        }

        public void Setup() {
            foreach (Player player in this.players) {

            }
        }
    }


    /// <summary>
    /// Play area.
    /// </summary>
    public class Board {
        private Deck rankDeck;
        private Deck storyDeck;
        private Deck adventureDeck;
        private Deck discardPile;

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
            get { return this.rankDeck; }
        }

        public Deck AdventureDeck {
            get { return this.rankDeck; }
        }

        public Deck DiscardPile {
            get { return this.rankDeck; }
        }
    }

    // Area containing a collection of cards.
    public abstract class CardArea {
        protected List<Card> cards = new List<Card>();

        public int Count {
            get { return this.cards.Count; }
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