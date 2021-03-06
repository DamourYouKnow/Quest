﻿using System;
using System.Collections.Generic;

using Quest.Utils;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class Card : SerializableObject {
        protected string name;
        protected string imageFilename;
        protected QuestMatch match;

        public Card(QuestMatch match) {
            this.Converter.Json = new CardJsonConversion();
			this.match = match;
        }

		public string Name {
			get { return this.name; }
		}

		public string ImageFilename {
			get { return this.imageFilename; }
		}

		public QuestMatch Match {
			get { return match; }
		}

        public virtual int BidValue {
            get { return 1; }
        }

        public override String ToString() {
            return this.name;
        }
    }

	public abstract class AdventureCard : Card {
        protected int freeBids = 0;

        public AdventureCard(QuestMatch match) : base(match) {

        }

		public int FreeBids {
			get { return this.freeBids; }
		}
    }

    public abstract class BattleCard : AdventureCard {
        protected int battlePoints = 0;

        public BattleCard(QuestMatch match) : base(match) {

        }

        public virtual int BattlePoints {
            get { return this.battlePoints; }
        }
    }

    public abstract class StoryCard: Card {
        public StoryCard(QuestMatch match) : base(match) {

        }
		public abstract void Run();
    }

    public abstract class InteractiveStoryCard : StoryCard {
        protected List<Player> participants;
        protected List<Player> responded;
        protected List<Player> participated;

        public InteractiveStoryCard(QuestMatch match) : base(match) {
            this.participants = new List<Player>();
            this.responded = new List<Player>();
            this.participated = new List<Player>();
        }

        public List<Player> Participants {
            get { return this.participants; }
        }

        public override void Run() {
            this.RequestNextParticipant();
        }

        /// <summary>
        /// Send prompts to players requesting their participation.
        /// </summary>
        public abstract void RequestNextParticipant();

        /// <summary>
        /// Send prompts to players requesting their plays.
        /// </summary>
        public abstract void RequestPlays();

        /// <summary>
        /// Resolve story after all players have played.
        /// </summary>
        public abstract void Resolve();

        /// <summary>
        /// Call this function when a player has responsed to a participartion request for a story.
        /// </summary>
        ///
        /// <param name="player">Player responding to request.</param>
        /// <param name="participating">Whether player is participating.</param>
        public virtual void ParticipationResponse(Player player, bool participating) {
            this.responded.Add(player);
            if (participating) {
                this.match.Controller.Message(this.match, player.Username + " participating in " + this.name);
                this.participants.Add(player);
            } else {
                this.match.Controller.Message(this.match, player.Username + " not participating in " + this.name);
            }

            if (this.responded.Count == this.match.Players.Count) {
                this.RequestPlays();
            }
        }

        /// <summary>
        /// Call this funtion when a player has made their play in a story.
        /// </summary>
        ///
        /// <param name="player">Player that was made their play.</param>
        public void AddPlayed(Player player) {
            if (!this.participated.Contains(player)) this.participated.Add(player);

            if (this.participated.Count == this.participants.Count) {
                this.Resolve();
            }
        }
    }

    public class Amour : BattleCard {
        public Amour(QuestMatch match) : base(match) {
            this.name = "Amour";
            this.imageFilename = "amour";
            this.battlePoints = 10;
            this.freeBids = 1;
        }
    }

    public abstract class RankCard : Card {
        protected Rank rank;
        protected int battlePoints;

        public RankCard(QuestMatch match) : base(match) {

        }

        public int BattlePoints {
            get { return this.battlePoints; }
        }

        public Rank Rank {
            get { return this.rank; }
			set { this.rank = value;}
        }
    }

    public class Squire : RankCard {
        public Squire(QuestMatch match) : base(match) {
            this.name = "Squire";
            this.imageFilename = "rank_squire";
            this.rank = Rank.Squire;
            this.battlePoints = 5;
        }
    }

    public class Knight : RankCard {
        public Knight(QuestMatch match) : base(match) {
            this.name = "Knight";
            this.imageFilename = "rank_knight";
            this.rank = Rank.Knight;
            this.battlePoints = 10;
        }
    }

    public class ChampionKnight : RankCard {
        public ChampionKnight(QuestMatch match) : base(match) {
            this.name = "Champion Knight";
            this.imageFilename = "champion_knight";
            this.rank = Rank.ChampionKnight;
            this.battlePoints = 20;
        }
    }

    public class KnightOfTheRoundTable : RankCard {
        public KnightOfTheRoundTable(QuestMatch match) : base(match) {
            this.name = "Knight of the Round Table";
            this.imageFilename = null;
            this.rank = Rank.KnightOfTheRoundTable;
            this.battlePoints = 0;
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
        }

        protected abstract void Init();

        public int DeckSize {
            get { return this.deckSize; }
        }

        public int Count {
            get { return this.cards.Count; }
        }

        public Card Draw() {
            // Check if out of cards.
            if (this.cards.Count == 0) {
                this.Init();
                this.Shuffle();
            }

            Card drawn = this.cards.Pop();
            return drawn;
        }

        public void Push(Card card) {
            this.cards.Push(card);
        }

		public string Peek(Deck deck){
			Card cardn = deck.cards.Peek ();
			return cardn.ToString ();
		}

        public void Shuffle() {
            this.match.Log("Shuffling " + this.ToString());
            List<Card> shuffleList = new List<Card>(this.cards);
            Utils.Random.Shuffle<Card>(shuffleList);
            this.cards = new Stack<Card>(shuffleList);
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

        public override string ToString() {
            return this.GetType().ToString();
        }
    }

    public class StoryDeck : Deck {
        public StoryDeck(QuestMatch match) : base(match) {

        }

		protected override void Init() {
			this.cards.Push (new SearchForTheHolyGrail (this.match));
			this.cards.Push (new TestOfTheGreenKnight (this.match));
			this.cards.Push (new SearchForTheQuestingBeast (this.match));
			this.cards.Push (new DefendTheQueensHonor (this.match));
			this.cards.Push (new RescueTheFairMaiden (this.match));
			this.cards.Push (new JourneyThroughTheEnchantedForest (this.match));
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new VanquishKingArthursEnemies (this.match));
			}
			this.cards.Push (new SlayTheDragon (this.match));
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new BoarHunt (this.match));
			}
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new RepelTheSaxonRaiders (this.match));
			}
			this.cards.Push (new TournamentAtCamelot (this.match));
			this.cards.Push (new TournamentAtOrkney (this.match));
			this.cards.Push (new TournamentAtTintagle (this.match));
			this.cards.Push (new TournamentAtYork (this.match));
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new RecognitionEvent (this.match));
			}
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new QueensFavourEvent (this.match));
			}
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new CourtCalledEvent (this.match));
			}
			this.cards.Push (new PoxEvent (this.match));
			this.cards.Push (new PlagueEvent (this.match));
			this.cards.Push (new ChivalrousDeedEvent (this.match));
			this.cards.Push (new ProsperityEvent (this.match));
			this.cards.Push (new CallToArmsEvent (this.match));

			//this.shuffle(); //comment out for testing deck
		}
    }

    public class AdventureDeck : Deck {
        public AdventureDeck(QuestMatch match) : base(match) {

        }

        protected override void Init() {
            // Create ally cards.
            this.cards.Push(new KingArthur(this.match));
            this.cards.Push(new KingPellinore(this.match));
            this.cards.Push(new SirGalahad(this.match));
            this.cards.Push(new SirGawain(this.match));
            this.cards.Push(new SirLancelot(this.match));
            this.cards.Push(new SirPercival(this.match));
			this.cards.Push(new SirTristan(this.match));
			for (int i = 0; i < 11; i++) {
				this.cards.Push (new Horse (this.match));
			}
			for (int i = 0; i < 16; i++) {
				this.cards.Push (new Sword (this.match));
			}
			for (int i = 0; i < 6; i++) {
				this.cards.Push (new Dagger (this.match));
			}
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new Excalibur (this.match));
			}
			for (int i = 0; i < 6; i++) {
				this.cards.Push (new Lance (this.match));
			}
			for (int i = 0; i < 8; i++) {
				this.cards.Push (new BattleAx (this.match));
			}
			this.cards.Push (new Dragon (this.match));
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new Giant (this.match));
			}
			for (int i = 0; i < 4; i++) {
				this.cards.Push (new Mordred (this.match));
			}
			for (int i = 0; i < 2; i++) {
				this.cards.Push (new GreenKnight (this.match));
			}
			for (int i = 0; i < 3; i++) {
				this.cards.Push (new BlackKnight (this.match));
			}
			for (int i = 0; i < 6; i++) {
				this.cards.Push (new EvilKnight (this.match));
			}
			for (int i = 0; i < 8; i++) {
				this.cards.Push (new SaxonKnight (this.match));
			}
			for (int i = 0; i < 7; i++) {
				this.cards.Push (new RobberKnight (this.match));
			}
			for (int i = 0; i < 5; i++) {
				this.cards.Push (new Saxons (this.match));
			}
			for (int i = 0; i < 4; i++) {
				this.cards.Push (new Boar (this.match));
			}
			for (int i = 0; i < 8; i++) {
				this.cards.Push (new Thieves (this.match));
			}
			for (int i = 0; i < 8; i++) {
				this.cards.Push (new Amour (this.match));
			}

			//this.shuffle(); //comment out for testing deck
		}

    }

    public class DiscardPile : Deck {
        public DiscardPile(QuestMatch match) : base(match) {

        }

        protected override void Init() {
            return;
        }
    }
}
