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

	public enum MatchState {
		INIT,
		START_GAME,
		START_TURN
	};
    public class QuestMatch : Subject{
        private List<Player> players;
        private int currentPlayer;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;
        private StoryCard currentStory;
        private Logger logger;
        private bool waiting;
		private MatchState state;

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
			this.state = MatchState.INIT;
        }

		public bool Waiting {
			get { return this.waiting; }
		}

		public MatchState State {
			get { return this.state; }
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
                List<Player> retList = this.players;
                retList.Remove(this.CurrentPlayer);
                return retList;
            }
        }

        /// <summary>
        /// Called by logic to wait for a response from the UI.
        /// </summary>
        public void Wait() {
			this.waiting = true;
			/*
            Thread waitThread = new Thread(new ThreadStart(Wait));
            waitThread.Start();
            waitThread.Join();
            */
        }

        private void waitTask() {
			/*
            while (this.waiting) {
                Thread.Sleep(100);
            }
            */
        }

        /// <summary>
        /// Called by the UI to return control.
        /// </summary>
        public void Continue() {
            this.waiting = false;
        }

        public void RunGame() {
            this.Log("Running game...");
            while (!this.hasWinner()) {
                this.NextTurn();
            }

		    List<Player> winner = this.getWinners();
			this.Log(Utils.Stringify.CommaList<Player>(winner) + " has won the game");
        }

        public void NextTurn() {
            if (this.currentPlayer + 1 >= this.players.Count) {
                this.currentPlayer = 0;
            }
            Player nextPlayer = this.players[this.currentPlayer];
            this.Log("Starting " + nextPlayer.ToString() + "'s turn");
			this.state = MatchState.START_TURN;
            //this.Wait ();
            this.NextStory();
        }

        public void NextStory() {
            StoryCard story = (StoryCard)this.storyDeck.Draw();
            this.Log("Story " + story.ToString() + " drawn");
            this.currentStory = story;

            try {
                story.Run();
            }
            catch (NotImplementedException) {
                this.Log("Feature not implemented");
            }
            catch (Exception e) {
                this.Log(e.Message);
                this.Log(e.StackTrace);
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
            this.storyDeck.Shuffle();
            this.adventureDeck.Shuffle();

            // Deal startingHandSize adventure cards to each player.
            foreach (Player player in this.players) {
				player.Draw(this.adventureDeck, Constants.MaxHandSize);
            }
			this.state = MatchState.START_TURN;
			this.Wait ();
            this.Log("Setup Quest match complete.");
        }

        public void Log(string message) {
            if (this.logger != null) {
                this.logger.Log(message);
            }
        }

        private List<Player> getWinners() {
            List<Player> winners = new List<Player>();
            foreach (Player player in this.players) {
                if (player.Rank.Value == Rank.KnightOfTheRoundTable) {
                    winners.Add(player);
                }
            }
            return winners;
        }

        private bool hasWinner() {
            return this.getWinners().Count > 0;
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

        public List<T> GetCards<T>() {
            List<T> retList = new List<T>();
            foreach (Card card in this.cards) {
                if (card is T) {
                    retList.Add((T)(object)card);
                } 
            }
            return retList;
        }

        public List<T> GetDistinctCards<T>() {
            List<T> retList = new List<T>();
            HashSet<string> cardNames = new HashSet<string>();
            foreach (Card card in this.cards) {
                if (card is T && !cardNames.Contains(card.Name)) {
                    retList.Add((T)(object)card);
                    cardNames.Add(card.Name);
                }
            }
            return retList;
        }

        public bool ContainsCopy(Card card) {
            foreach (Card c in this.cards) {
                if (c.Name == card.Name) {
                    return true;
                }
            }
            return false;
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

        public override string ToString() {
            return Utils.Stringify.CommaList<Card>(this.cards);
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
        private Player player;

        public Player Player {
            get { return this.player; }
        }

        public Hand(Player player) : base() {
            this.player = player;
        }
    }
}