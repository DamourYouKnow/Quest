using System;
using System.Collections.Generic;
using Quest.Core.Cards;
using Quest.Core.Players;

namespace Quest.Core {
    public static class Constants {
        public const int MaxHandSize = 12;
    }

    public class QuestMatch {
        private List<Player> players;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;

        public QuestMatch() {
            this.players = new List<Player>();
            this.storyDeck = new StoryDeck(this);
            this.adventureDeck = new AdventureDeck(this);
            this.discardPile = new DiscardPile(this);
        }

        public List<Player> Players {
            get { return this.players; }
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

        public void AddPlayer(Player player) {
            this.players.Add(player);
        }

        public void Setup() {
            // Deal startingHandSize adventure cards to each player.
            foreach (Player player in this.players) {
                player.Draw(this.adventureDeck, Constants.MaxHandSize);
            }
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

        public void Remove(Card card) {
            if (!this.cards.Contains(card)) {
                throw new Exception("Card not in area.");
            }
            this.cards.Remove(card);
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