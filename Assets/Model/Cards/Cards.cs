using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class Card {
        protected string name;
        protected string imageFilename;
        protected int battlePoints;
        protected QuestMatch match;

        public Card(QuestMatch match) {

        }
    }

    public abstract class AdventureCard : Card {
        public AdventureCard(QuestMatch match) : base(match) {

        }
    }

    public abstract class StoryCard: Card {
        public StoryCard(QuestMatch match) : base(match) {

        }
    }
	
    public class WeaponCard : AdventureCard {
        public WeaponCard(QuestMatch match) : base(match) {

        }
    }

    public class AmourCard : AdventureCard {
        public AmourCard(QuestMatch match) : base(match) {

        }
    }

    public class RankCard : Card {
        private PlayerRank rank;

        public RankCard(QuestMatch match) : base(match) {

        }

        public PlayerRank Rank {
            get { return this.rank; }
        }
    }

    public class EventCard : StoryCard {
        public EventCard(QuestMatch match) : base(match) {

        }
    }

    public class TournamentCard : StoryCard {
        public TournamentCard(QuestMatch match) : base(match) {

        }
    }

    public class TestCard : StoryCard {
        private int minBid;

        public TestCard(QuestMatch match) : base(match) {

        }
    }

    /// <summary>
    /// Deck of cards of a specific type.
    /// </summary>
    public abstract class Deck {
        protected QuestMatch match;
        protected int deckSize;
        protected Stack<Card> cards = new Stack<Card>();

        public Deck(QuestMatch match) {
            this.match = match;
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
        public StoryDeck(QuestMatch match) : base(match) {

        }

        public override void Init() {
            // TODO: Init deck with proper cards.
            for (int i = 0; i < 50; i++) {
                this.cards.Push(new TestCard(this.match));
            }
        }
    }

    public class AdventureDeck : Deck {
        public AdventureDeck(QuestMatch match) : base(match) {

        }

        public override void Init() {
            // Create ally cards.
            this.cards.Push(new KingArthur(this.match));
            this.cards.Push(new KingPellinore(this.match));
            this.cards.Push(new SirGalahad(this.match));
            this.cards.Push(new SirGawain(this.match));
            this.cards.Push(new SirLancelot(this.match));
            this.cards.Push(new SirPercival(this.match));
            this.cards.Push(new SirTristan(this.match));

            // TODO: Init deck with proper cards.
            for (int i = 0; i < 50; i++) {
                this.cards.Push(new BlackKnight(this.match));
            }
        }
    }

    public class DiscardPile : Deck {
        public DiscardPile(QuestMatch match) : base(match) {

        }

        public override void Init() {
            return;
        }
    }
}

