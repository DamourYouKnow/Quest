using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class Card {
        protected string name;
        protected string imageFilename;
        protected QuestMatch match;

        public Card(QuestMatch match) {
			this.match = match;
        }

        public string Name {
            get { return this.name; }
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

    public class Amour : BattleCard {
        public Amour(QuestMatch match) : base(match) {
            this.name = "Amour";
            this.imageFilename = "amour.png";
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
            this.imageFilename = "rank_squire.png";
            this.rank = Rank.Squire;
            this.battlePoints = 5;
        }
    }

    public class Knight : RankCard {
        public Knight(QuestMatch match) : base(match) {
            this.name = "Knight";
            this.imageFilename = "rank_knight.png";
            this.rank = Rank.Knight;
            this.battlePoints = 10;
        }
    }

    public class ChampionKnight : RankCard {
        public ChampionKnight(QuestMatch match) : base(match) {
            this.name = "Champion Knight";
            this.imageFilename = "champion_knight.png";
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
            Card drawn = this.cards.Pop();
            this.match.Log(drawn.ToString() + " was drawn from " + this.ToString());
            return drawn;
        }

        public void Push(Card card) {
            this.cards.Push(card);
        }

		public string Peek(Deck deck){
			Card cardn = deck.cards.Peek ();
			return cardn.ToString ();
		}

        protected void shuffle() {
            this.match.Log("Shuffling " + this.ToString());
            List<Card> shuffleList = new List<Card>(this.cards);
            Utils.Random.Shuffle<Card>(shuffleList);
            this.cards = new Stack<Card>(shuffleList);
        }

        public override string ToString() {
            return this.GetType().ToString();
        }
    }

    public class StoryDeck : Deck {
        public StoryDeck(QuestMatch match) : base(match) {

        }

        protected override void Init() {
			List<StoryCard> story_deck = new List<StoryCard> {
				new SearchForTheHolyGrail (this.match),             //1
				new TestOfTheGreenKnight (this.match),              //2
				new SearchForTheQuestingBeast (this.match),         //3
				new DefendTheQueensHonor (this.match),              //4
				new RescueTheFairMaiden (this.match),               //5
				new JourneyThroughTheEnchantedForest (this.match),  //6
				new VanquishKingArthursEnemies (this.match),        //7
				new SlayTheDragon (this.match),                     //8
				new BoarHunt (this.match),                          //9
				new RepelTheSaxonRaiders (this.match),              //10
				new TournamentAtCamelot(this.match),                //11
				new TournamentAtOrkney(this.match),                 //12
				new TournamentAtTintagle(this.match),               //13
				new TournamentAtYork(this.match),                   //14
				new RecognitionEvent(this.match),                   //15
				new QueensFavourEvent(this.match),                  //16
				new CourtCalledEvent(this.match),                   //17
				new PoxEvent(this.match),                           //18
				new PlagueEvent(this.match),                        //19
				new ChivalrousDeedEvent(this.match),                //20
				new ProsperityEvent(this.match),                    //21
				new CallToArmsEvent(this.match)                     //22
			};


			List<int> deck_quantity_list = new List<int>{ 
				1,  //1
				1,  //2
				1,  //3
				1,  //4
				1,  //5
				1,  //6
				2,  //7
				1,  //8
				2,  //9
				2,  //10
				1,  //11
				1,  //12
				1,  //13
				1,  //14
				2,  //15
				2,  //16
				2,  //17
				1,  //18
				1,  //19
				1,  //20
				1,  //21
				1}; //22

			for (int i = 0; i < story_deck.Count; i++) {
				int deck_quantity = deck_quantity_list[i];
				for (int j = 0; j < deck_quantity; j++) {
					this.cards.Push(story_deck[i]);
				} 
			} 

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

			//create the rest of the adventure cards and add them to deck
			List<AdventureCard> adventure_list = new List<AdventureCard>{
				new Horse(this.match),                  //1
				new Sword(this.match),                  //2
				new Dagger(this.match),                 //3
				new Excalibur(this.match),              //4
				new Lance(this.match),                  //5
				new BattleAx(this.match),               //6
				new Dragon(this.match),                 //7
				new Giant(this.match),                  //8
				new Mordred(this.match),                //9
				new GreenKnight(this.match),            //10
				new BlackKnight(this.match),            //11
				new EvilKnight(this.match),             //12
				new SaxonKnight(this.match),            //13
				new RobberKnight(this.match),           //14
				new Saxons(this.match),                 //15
				new Boar(this.match),                   //16
				new Thieves(this.match),                //17
				new TestOfValor(this.match),            //18
				new TestOfTemptation(this.match),       //19
				new TestOfMorganLeFey(this.match),      //20
				new TestOfTheQuestingBeast(this.match)  //21 
			};
			

			List<int> deck_quantity_list = new List<int> {
				11, //1 
				16, //2
				6,  //3
				2,  //4
				6,  //5
				8,  //6
				1,  //7
				2,  //8
				4,  //9
				2,  //10
				3,  //11
				6,  //12
				8,  //13
				7,  //14
				5,  //15
				4,  //16
				8,  //17
				2,  //18
				2,  //19
				2,  //20
				2}; //21 

			for (int i = 0; i < adventure_list.Count; i++) {
				int deck_quantity = deck_quantity_list[i];
				for (int j = 0; j < deck_quantity; j++) {
					this.cards.Push(adventure_list[i]);
				} 
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

