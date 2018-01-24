using System;
using System.Collections.Generic;

namespace Quest.Core {
    public abstract class BattleBehaviour {

    }

    public abstract class Card {
        protected string name;
        protected string imageFilename;
        protected int battlePoints;
    }

    public abstract class AdventureCard : Card {
       
    }

    public abstract class StoryCard: Card {

    }

    public class AllyCard : AdventureCard {

    }

    public class FoeCard : AdventureCard {
    
    }

    public class WeaponCard : AdventureCard {

    }

    public class AmourCard : AdventureCard {

    }

    public class RankCard : AdventureCard {
        private PlayerRank rank;

        public PlayerRank Rank {
            get { return this.rank; }
        }
    }

    public class EventCard : Card {
     
    }

    public class TournamentCard : Card {

    }

    public class TestCard : Card {
        private int minBid;
    }

    /// <summary>
    /// Deck of cards of a specific type.
    /// </summary>
    public abstract class Deck {
        protected Stack<Card> cards = new Stack<Card>();

        public Deck() {
            this.Init();
            this.shuffle();
        }

        public abstract void Init();

        public int Count {
            get { return this.cards.Count; }
        }

        public Card Draw() {
            return this.cards.Pop();
        }

        public void Push(Card card) {
            this.cards.Push(card);
        }

        /// <summary>
        /// Deals count number of cards from this deck to a player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="count"></param>
        public void Deal(Player player, int count) {
            count = Math.Min(count, this.cards.Count);
            for (int i = 0; i < count; i++) {
                player.Hand.Add(this.Draw());
            }
        }

        protected void shuffle() {
            List<Card> shuffleList = new List<Card>(this.cards);
            Utils.Random.Shuffle<Card>(shuffleList);
            this.cards = new Stack<Card>(shuffleList);
        }
    }

    public class RankDeck : Deck {
        public const int deckSize = 12;

        public override void Init() {
            // TODO: Init deck with proper cards.
            for (int i = 0; i < deckSize; i++) {
                this.cards.Push(new RankCard());
            }
        }
    }

    public class StoryDeck : Deck {
        public const int deckSize = 28;

        public override void Init() {
            // TODO: Init deck with proper cards.
            for (int i = 0; i < deckSize; i++) {
                this.cards.Push(new TestCard());
            }
        }
    }

    public class AdventureDeck : Deck {
        public const int deckSize = 125;

        public override void Init() {
            // TODO: Init deck with proper cards.
            for (int i = 0; i < deckSize; i++) {
                this.cards.Push(new FoeCard());
            }
        }
    }

    public class DiscardPile : Deck {
        public override void Init() {
            return;
        }
    }
}

