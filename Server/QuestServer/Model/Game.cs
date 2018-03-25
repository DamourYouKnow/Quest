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
        START_TURN,
        RUN_STORY,
        REQUEST_SPONSOR,
        REQUEST_PARTICIPANTS,
        REQUEST_STAGE,
        RUN_STAGE,
        RESOLVE_STAGE,
        RESOLVE_QUEST,
        END_STORY,
        START_TOURNAMENT,
		PLAY_TOURNAMENT
    };
    public class QuestMatch {
        private List<Player> players;
        private int currentPlayer;
        private int promptingPlayer;
        private StoryDeck storyDeck;
        private AdventureDeck adventureDeck;
        private DiscardPile discardPile;
        private StoryCard currentStory;
        private Logger logger;

        public QuestMatch(Logger logger = null) {
            this.players = new List<Player>();
            this.currentPlayer = 0;
            this.storyDeck = new StoryDeck(this);
            this.adventureDeck = new AdventureDeck(this);
            this.discardPile = new DiscardPile(this);
            this.currentStory = null;
            this.logger = logger;
            this.Log("Creating new Quest match");
        }

		public int PromptingPlayer {
			get { return this.promptingPlayer; }
			set { this.promptingPlayer = value; }
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
        public int CurrentPlayerNum {
            get { return this.currentPlayer; }
            set { this.currentPlayer = value; }
        }

        public List<Player> OtherPlayers {
            get {
                List<Player> retList = new List<Player>(this.players);
                retList.Remove(this.CurrentPlayer);
                return retList;
            }
        }

        public void RunGame() {
            this.Log("Running game...");

			while (!this.hasWinner ()) {
				this.NextTurn ();
			}

			List<Player> winner = this.getWinners ();
			this.Log (Utils.Stringify.CommaList<Player> (winner) + " has won the game");
        }

        public void NextTurn() {
            Player nextPlayer = this.players[this.currentPlayer];
            this.Log("Starting " + nextPlayer.ToString() + "'s turn");
			this.NextStory();
			this.currentPlayer = (this.currentPlayer + 1)%this.players.Count;
        }

        public void NextStory() {
            StoryCard story = (StoryCard)this.storyDeck.Draw();
            this.Log("Story " + story.ToString() + " drawn");
			this.currentStory = story;
			try{
				story.Run();
			}
			catch(NotImplementedException){
				this.Log ("Feature not implemented");
			}
			catch (Exception e){
				this.Log (e.Message);
				this.Log (e.StackTrace);
			}
        }

        public void RunStory() {
            try {
                CurrentStory.Run();

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

        public Player PlayerWithCardOnBoard(Card card) {
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

        public void Setup(bool shuffleDecks = true) {
            if (shuffleDecks) {
                this.storyDeck.Shuffle();
                this.adventureDeck.Shuffle();
            }

            // Deal startingHandSize adventure cards to each player.
            foreach (Player player in this.players) {
                while (player.Hand.Count < Constants.MaxHandSize) {
                    player.Draw(this.adventureDeck);
                }
            }

            this.Log("Setup Quest match complete.");
        }

		public void AttachLogger(Logger logger) {
			this.logger = logger;
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
			set { this.cards = value; }
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

        public virtual void Remove(Card card) {
            if (!this.cards.Contains(card)) {
                throw new Exception("Card not in area.");
            }
            this.cards.Remove(card);
        }

        public virtual void Transfer(CardArea target, Card card) {
			if (target != null && this.cards.Contains(card)) {
                if (target.GetType().Equals(typeof(QuestArea))) {
                    QuestArea qatarget = target as QuestArea;
                    qatarget.Add(card);
                    if (qatarget.cards.Contains(card)) {
                        this.Remove(card);
                    }
                } else {
                    target.Add(card);
                    if (target.cards.Contains(card)) {
                        this.Remove(card);
                    }
                }
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

    public class BattleArea : CardArea {
        public virtual int BattlePoints() {
            int total = 0;
            foreach (BattleCard card in this.BattleCards) {
                total += card.BattlePoints;
            }
            return total;
        }
    }

    /// <summary>
    /// Battle area on a game board.
    /// </summary>
    public class PlayerArea : BattleArea {
		public override void Add(Card card){
			bool canAdd = true;
			foreach(Card ccard in this.cards){
				canAdd = ccard.Name != card.Name;
				if (!canAdd) {
					break;
				}
			}
			if (canAdd && !card.GetType().IsSubclassOf(typeof(TestCard)) && !card.GetType().IsSubclassOf(typeof(FoeCard))) {
				this.cards.Add(card);
			}
		}
    }

	/// <summary>
	/// Area for quest stages.
	/// </summary>
	public class QuestArea : BattleArea {
		private Card mainCard;

		public QuestArea(){
			this.mainCard = null;
		}

		public QuestArea(FoeCard foe){
            this.mainCard = foe;
		}

        public QuestArea(TestCard test) {
            this.mainCard = test;
        }

        public Card MainCard {
            get { return mainCard; }
			set { this.mainCard = value; }
        }

		public override void Add(Card card){
			if (card.GetType().BaseType.Equals(typeof(FoeCard)) || card.GetType().BaseType.Equals(typeof(TestCard))) {
				if (this.mainCard == null) {
					this.mainCard = card;
					this.cards.Add (card);
				}
				else {
					return;
				}
			}
			else {
				bool canAdd = true;
				foreach(Card ccard in this.cards){
					canAdd = ccard.Name != card.Name;
					if (!canAdd) {
						break;
					}
				}
				if (canAdd && !card.GetType().IsSubclassOf(typeof(AllyCard)) && this.mainCard!=null && !this.mainCard.GetType().IsSubclassOf(typeof(TestCard))) {
					this.cards.Add(card);
				}
			}
		}
		public override void Remove(Card card) {
			if (!this.cards.Contains(card)) {
				throw new Exception("Card not in area.");
			}
			this.cards.Remove(card);
			if (this.mainCard == card) {
				this.mainCard = null;
			}
		}
		public override int BattlePoints(){
			int total = 0;
			if (this.mainCard.GetType ().IsSubclassOf (typeof(FoeCard))) {
				foreach (BattleCard card in this.BattleCards) {
					if (!card.GetType ().BaseType.Equals (typeof(FoeCard)) && !card.GetType ().BaseType.Equals (typeof(TestCard))) {
						total += card.BattlePoints;
					}
				}
				total += (this.mainCard as FoeCard).BattlePoints;
				return total;
			}
			else {
				return 0;
			}
		}
	}

	public class TournamentArea : BattleArea {

		public TournamentArea(){
		}
			

		public override void Add(Card card){
			if (!card.GetType().BaseType.Equals(typeof(FoeCard)) || !card.GetType().BaseType.Equals(typeof(AllyCard))) {
				this.cards.Add (card);
			}
			else {
				bool canAdd = true;
				foreach(Card ccard in this.cards){
					canAdd = ccard.Name != card.Name;
					if (!canAdd) {
						break;
					}
				}
			}
		}

		public override void Remove(Card card) {
			if (!this.cards.Contains(card)) {
				throw new Exception("Card not in area.");
			}
			this.cards.Remove(card);
		}
		public override int BattlePoints(){
			int total = 0;
			foreach (BattleCard card in this.BattleCards) {
				total += card.BattlePoints;
			}
			return total;
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