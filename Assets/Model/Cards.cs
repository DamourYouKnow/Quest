using System;
using System.Collections.Generic;

namespace Quest.Core {
    public abstract class Card {
        protected string name;
        protected string imageFilename;
    }

    public abstract class BattleCard : Card {
        // For cards that use battle points (ally, foe, amour, weapon).
        protected int battlePoints; // Change type to struct later to support sponsored cards.
    }

    public abstract class StoryCard: Card {

    }

    public class AllyCard : BattleCard {

    }

    public class FoeCard : BattleCard {

    }

    public class WeaponCard : BattleCard {

    }

    public class AmourCard : BattleCard {

    }

    public class RankCard : BattleCard {
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
    public class Deck {
        protected Stack<Card> cards = new Stack<Card>();

        public int Count {
            get { return this.cards.Count; }
        }

        public Card Draw() {
            return this.cards.Pop();
        }

        public void Push(Card card) {
            this.cards.Push(card);
        }

        public void Shuffle() {
            List<Card> shuffleList = new List<Card>(this.cards);
            Utils.Random.Shuffle<Card>(shuffleList);
            this.cards = new Stack<Card>(shuffleList);
        }
    }

    public class CardDealer {
        private Deck deck;

        public CardDealer(Deck deck) {
            this.deck = deck;
        }

        // TODO: Consider changing Hand to Player later on.
        public void Deal(List<Hand> hands, int count) {
            count = Math.Max(count, this.deck.Count - count);
            this.deck.Shuffle();

            foreach (Hand hand in hands) {
                for (int i = 1; i <= count; i++) {
                    hand.Add(this.deck.Draw());
                }
            }
        }

        public void Deal(Hand hand, int count) {
            count = Math.Max(count, this.deck.Count - count);
            this.deck.Shuffle();

            for (int i = 1; i <= count; i++) {
                hand.Add(this.deck.Draw());
            }
        }
    }
}

