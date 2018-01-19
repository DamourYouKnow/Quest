using System;
using System.Collections.Generic;

namespace Quest.Core {
    public abstract class Card {
        protected string name;
    }

    public abstract class BattleCard : Card {
        // For cards that use battle points (ally, foe, amour, weapon).
        protected int battlePoints; // Change type to struct later to support sponsored cards.
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

    }

    public class EventCard : Card {
     
    }

    public class TournamentCard : Card {

    }

    public class TestCard : Card {

    }

    /// <summary>
    /// Deck of cards of a specific type.
    /// </summary>
    public class Deck {
        protected Stack<Card> cards = new Stack<Card>();

        public int Count {
            get { return cards.Count; }
        }

        public Card Draw() {
            return cards.Pop();
        }

        public void Push(Card card) {
            cards.Push(card);
        }

        public void Shuffle() {
            List<Card> shuffleList = new List<Card>(cards);
            Utils.Random.Shuffle<Card>(shuffleList);
            cards = new Stack<Card>(shuffleList);
        }
    }

    public class CardDealer {
        private Deck deck;

        public CardDealer(Deck deck) {
            this.deck = deck;
        }

        // TODO: Consider changing Hand to Player later on.
        public void Deal(List<Hand> hands, int count) {
            count = Math.Max(count, deck.Count - count);
            deck.Shuffle();

            foreach (Hand hand in hands) {
                for (int i = 1; i <= count; i++) {
                    hand.Add(deck.Draw());
                }
            }
        }

        public void Deal(Hand hand, int count) {
            count = Math.Max(count, deck.Count - count);
            deck.Shuffle();

            for (int i = 1; i <= count; i++) {
                hand.Add(deck.Draw());
            }
        }
    }
}

