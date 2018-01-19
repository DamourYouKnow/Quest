using System;
using System.Collections.Generic;

namespace Quest.Core {
    /// <summary>
    /// Play area.
    /// </summary>
    public class Board {

    }

    // Area containing a collection of cards.
    public abstract class CardArea {
        protected List<Card> cards = new List<Card>();

        public int Count {
            get { return cards.Count; }
        }

        public void Add(Card card) {
            cards.Add(card);
        }

        public void Transfer(CardArea target, Card card) {
            target.cards.Add(card);
            cards.Remove(card);
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