using System;
using System.Collections.Generic;
using System.Threading;
using Quest.Core.Cards;
using Quest.Core.Players;
using Utils;

namespace Quest.Core {
    public static class Constants {
        public const int MaxHandSize = 12;
    }

    public class QuestMatch : Subject{
        private List<Player> players;
        private int currentPlayer;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;
        private StoryCard currentStory;
        private Logger logger;
        private bool waiting;

        public QuestMatch(Logger logger=null) {
            this.players = new List<Player>();
            this.currentPlayer = 0;
            this.storyDeck = new StoryDeck(this);
            this.adventureDeck = new AdventureDeck(this);
            this.discardPile = new DiscardPile(this);
			this.currentStory = null;
            this.logger = logger;
            this.Log("Creating new Quest match");
            this.waiting = false;
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

        public Player CurrentPlayer {
            get { return this.players[this.currentPlayer]; }
        }

        public List<Player> OtherPlayers {
            get {
                List<Player> retList = new List<Player>();
                retList.Remove(this.CurrentPlayer);
                return retList;
            }
        }

        public void Wait() {
            throw new NotImplementedException();
        }

        private void waitTask() {
            while (this.waiting) {
                Thread.Sleep(100);
            }
        }

        public void Continue() {
            throw new NotImplementedException();
        }

        public void NextTurn() {
            if (this.currentPlayer + 1 >= this.players.Count) {
                this.currentPlayer = 0;
            }
            Player nextPlayer = this.players[this.currentPlayer];
            this.NextStory();
        }

        public void NextStory() {
            StoryCard story = (StoryCard)this.storyDeck.Draw();
            story.Run();


            if (story is QuestCard) {
                QuestCard quest = (QuestCard)story;
                if (this.CurrentPlayer.Behaviour.SponsorQuest(quest, this.CurrentPlayer.Hand) {
                    // TODO: Move logic somewhere else.
                    quest.Sponsor = this.CurrentPlayer;
                    foreach (Player player in this.OtherPlayers) {
                        if (player.Behaviour.ParticipateInQuest(quest, player.Hand) {
                            quest.AddParticipant(player);
                        }
                    }
                }
            }

            if (story is EventCard) {
                
            }

            if (story is TournamentCard) {

            }
        }

        public void AddPlayer(Player player) {
            this.players.Add(player);
            this.Log("Added player " + player.Username + " to Quest match");
        }

        public Player PlayerWithCard(Card card) {
            foreach (Player player in this.players) {
                if (player.CardInHand(card)) {
                    return player;
                }
            }
            return null;
        }
		
		public Player PlayerWithCardOnBoard(Card card){
			foreach (Player player in this.players) {
                if (player.CardInPlay(card)) {
                    return player;
                }
            }
            return null;
		}
		
        public Player PlayerWithMostBattlePoints() {
            Player maxPlayer = this.players[0];
            foreach (Player player in this.players) {
                if (player.BattlePointsInPlay() > maxPlayer.BattlePointsInPlay()) {
                    maxPlayer = player;
                }
            }
            return maxPlayer;
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

        public virtual int Count {
            get { return this.cards.Count; }
        }

        public List<Card> Cards {
            get { return this.cards; }
        }

        public List<AdventureCard> AdventureCards {
            get { return this.GetCards<AdventureCard>(); }
        }

        public List<BattleCard> BattleCards {
            get { return this.GetCards<BattleCard>(); }
        }

        public List<StoryCard> StoryCards {
            get { return this.GetCards<StoryCard>(); }
        }

        public List<TestCard> TestCards {
            get { return this.GetCards<TestCard>(); }
        }

        public List<T> GetCards<T>() {
            List<T> retList = new List<T>();
            foreach (Card card in this.cards) {
                if (card is T) {
                    retList.Add((T)(object)card);
                } 
            }
            return retList;
        }

        public virtual void Add(Card card) {
            this.cards.Add(card);
        }

        public virtual void Add(List<Card> cards) {
            foreach (Card card in cards) {
                this.Add(card);
            }
        }

        public void Remove(Card card) {
            if (!this.cards.Contains(card)) {
                throw new Exception("Card not in area.");
            }
            this.cards.Remove(card);
        }

        public void Transfer(CardArea target, Card card) {
            if (this.cards.Contains(card)) {
                target.cards.Add(card);
                this.cards.Remove(card);
            }
        }

        public void Transfer(CardArea target, List<Card> cards) {
            cards = new List<Card>(cards); // Stop bad things from happening.
            foreach (Card card in cards) {
                this.Transfer(target, card);
            }
        }
    }

    /// <summary>
    /// Battle area on a game board.
    /// </summary>
    public class BattleArea : CardArea {
		public virtual int BattlePoints(){
			int total = 0;
			foreach (BattleCard card in this.BattleCards) {
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

		public QuestArea(FoeCard foe){
            this.mainCard = foe;
		}

        public QuestArea(TestCard test) {
            this.mainCard = test;
        }

        public Card MainCard {
            get { return mainCard; }
        }
	}

    /// <summary>
    /// Card hand beloning to a player.
    /// </summary>
    public class Hand : CardArea {

    }
}