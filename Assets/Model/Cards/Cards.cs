using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class Card {
        protected string name;
        protected string imageFilename;
        protected int battlePoints;
        protected QuestMatch match;
    }

    public abstract class AdventureCard : Card {
        public AdventureCard() {

        }
    }

    public abstract class StoryCard: Card {

    }
	
    public class WeaponCard : AdventureCard {

    }

    public class AmourCard : AdventureCard {

    }

    public class RankCard : Card {
        private PlayerRank rank;

        public PlayerRank Rank {
            get { return this.rank; }
        }
    }

    public class EventCard : StoryCard {

    }

    public class TournamentCard : StoryCard {

    }

    public class TestCard : StoryCard {
        private int minBid;

    }

    /// <summary>
    /// Deck of cards of a specific type.
    /// </summary>
    public abstract class Deck {
        protected int deckSize;
        protected Stack<Card> cards = new Stack<Card>();

        public Deck() {
            this.Init();
            this.deckSize = this.cards.Count;
            this.shuffle();
        }

        public abstract void Init();

        public int DeckSize {
            get { return this.deckSize; }
        }

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

    public class StoryDeck : Deck {
        public override void Init() {
            // TODO: Init deck with proper cards.
            for (int i = 0; i < 50; i++) {
                this.cards.Push(new TestCard());
            }
        }
    }

    public class AdventureDeck : Deck {
        public override void Init() {
            // Create ally cards.
            this.cards.Push(new KingArthur());
            this.cards.Push(new KingPellinore());
            this.cards.Push(new SirGalahad());
            this.cards.Push(new SirGawain());
            this.cards.Push(new SirLancelot());
            this.cards.Push(new SirPercival());
            this.cards.Push(new SirTristan());

            // TODO: Init deck with proper cards.
            for (int i = 0; i < 50; i++) {
                this.cards.Push(new BlackKnight());
            }
        }
    }

    public class DiscardPile : Deck {
        public override void Init() {
            return;
        }
    }
}

