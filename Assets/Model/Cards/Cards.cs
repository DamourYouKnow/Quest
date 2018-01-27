using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class Card {
        protected string name;
        protected string imageFilename;
        protected int battlePoints;

        public Card() {

        }

        public Card(string name, string imageFilename, int battlePoints) {
            this.name = name;
            this.imageFilename = imageFilename;
            this.battlePoints = battlePoints;
        }

        public Card(string name, string imageFilename) {
            this.name = name;
            this.imageFilename = imageFilename;
            this.battlePoints = 0;
        }
    }

    public abstract class AdventureCard : Card {
        public AdventureCard() {

        }

        public AdventureCard(string name, string imageFilename, int battlePoints) 
            :base(name, imageFilename, battlePoints) {
            
        }

        public AdventureCard(string name, string imageFilename)
            : base(name, imageFilename) {

        }
    }

    public abstract class StoryCard: Card {
        public StoryCard() {

        }

        public StoryCard(string name, string imageFilename)
            :base(name, imageFilename) { 

        }
    }

	/*
	//implemented in FoeCards.cs (leaving this here for now just in case)
    public class FoeCard : AdventureCard {
        public FoeCard(string name, string imageFilename, int battlePoints)
            : base(name, imageFilename, battlePoints) {

        }
    }
	*/
	
    public class WeaponCard : AdventureCard {
        public WeaponCard(string name, string imageFilename, int battlePoints)
            : base(name, imageFilename, battlePoints) {

        }
    }

    public class AmourCard : AdventureCard {
        public AmourCard(string name, string imageFilename, int battlePoints)
            : base(name, imageFilename, battlePoints) {

        }
    }

    public class RankCard : Card {
        private PlayerRank rank;

        public RankCard(string name, string imageFilename)
            : base(name, imageFilename) {

        }

        public PlayerRank Rank {
            get { return this.rank; }
        }
    }

    public class EventCard : StoryCard {
        public EventCard(string name, string imageFilename)
            : base(name, imageFilename) {

        }
    }

    public class TournamentCard : StoryCard {
        public TournamentCard(string name, string imageFilename)
            : base(name, imageFilename) {

        }
    }

    public class TestCard : StoryCard {
        private int minBid;

        public TestCard(string name, string imageFilename)
            : base(name, imageFilename) {

        }
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
                this.cards.Push(new TestCard(null, null));
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
                this.cards.Push(new FoeCard(null, null, 1));
            }
        }
    }

    public class DiscardPile : Deck {
        public override void Init() {
            return;
        }
    }
}

