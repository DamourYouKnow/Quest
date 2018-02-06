using System;
using System.Collections.Generic;
using Quest.Core.Cards;
using Quest.Core.Players;
using Utils;

namespace Quest.Core {
    public static class Constants {
        public const int MaxHandSize = 12;
    }

    public class QuestMatch : Subject{
        private List<Player> players;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;
        private StoryCard currentStory;
        private Logger logger;

        public QuestMatch(Logger logger=null) {
            this.players = new List<Player>();
            this.storyDeck = new StoryDeck(this);
            this.adventureDeck = new AdventureDeck(this);
            this.discardPile = new DiscardPile(this);
			this.currentStory = null;
            this.logger = logger;
            this.Log("Creating new Quest match");
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

		public StoryCard CurrentStory {
			get { return this.currentStory; }
			set { this.currentStory = value; }
		}

        public void AddPlayer(Player player) {
            this.players.Add(player);
            this.Log("Added player " + player.Username + " to Quest match");
        }

        public void Setup() {
            // Deal startingHandSize adventure cards to each player.
            foreach (Player player in this.players) {
                player.Draw(this.adventureDeck, Constants.MaxHandSize);
            }

            this.Log("Setup Quest match.");
        }

        public void Log(string message) {
            if (this.logger != null) {
                this.logger.Log(message);
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

        public virtual void Add(Card card) {
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
			if (target.cards.Contains (card)) {
				this.cards.Remove(card);
			}
        }
    }

    /// <summary>
    /// Battle area on a game board.
    /// </summary>
    public class BattleArea : CardArea {
		public virtual int BattlePoints(){
			int total = 0;
			foreach (var item in cards) {
				AdventureCard card = item as AdventureCard;
				total += card.BattlePoints;
			}
			return total;
		}
    }

	/// <summary>
	/// Area for quest stages.
	/// </summary>
	public class QuestArea : BattleArea {
		private Card mainCard;
		public Card MainCard {
			get{ return mainCard; }
		}

		public QuestArea(List<Card> stageCards){
			this.mainCard = null;
			this.cards = stageCards;
		}
		public override void Add(Card card) {
			if (mainCard == null) {
				if (card is FoeCard
					|| card is TestCard){
					mainCard = card;
				}
			} 
			else if (!(card is FoeCard)
				&& !(card is TestCard)){
				base.Add (card);
			}
		}
		public override int BattlePoints(){
			int total = base.BattlePoints ();
			if (mainCard != null
				&& mainCard is FoeCard) {
				FoeCard foe = mainCard as FoeCard;
				total += foe.BattlePoints;
			}
			return total;
		}

	}

    /// <summary>
    /// Card hand beloning to a player.
    /// </summary>
    public class Hand : CardArea {

    }
}