using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class TestCard : AdventureCard {
        protected int minBid = 0;
        protected int currentBid = 0;
        protected Player currentBidPlayer;
        protected QuestCard quest;
        protected QuestCard minBidQuest = null;
        protected Dictionary<Player, List<Card>> bids;

        public TestCard(QuestMatch match) : base(match) {
            this.bids = new Dictionary<Player, List<Card>>();
            foreach (Player player in this.match.Players) {
                this.bids.Add(player, new List<Card>());
            }
        }

        public int HigestBid {
            get { return this.currentBid; }
        }

        public Player HighestBidPlayer {
            get { return this.currentBidPlayer; }
        }

        public void AddBid(Player player, Card card) {
            if (card.BidValue <= currentBid) {
                throw new Exception("Value of bid must be higher than current bid");
            }
            // TODO: Add bids from cards in play.
            this.bids[player].Add(card);
            this.currentBid = card.BidValue;
            this.currentBidPlayer = player;
        }

        public void AddBid(Player player, List<Card> cards) {
            int bids = 0;
            foreach (Card card in cards) {
                bids += card.BidValue;
            }

            if (bids <= currentBid) {
                throw new Exception("Value of bid must be higher than current bid");
            }

            this.bids[player].AddRange(cards);
            this.currentBid = bids;
            this.currentBidPlayer = player;
        }

        public int GetBids(Player player) {
            int bids = this.bids[player].Count;
            // TODO: Add bids from cards in play.
            return bids;
        }
    }

    public class TestOfMorganLeFey : TestCard {
        public TestOfMorganLeFey(QuestMatch match) : base(match) {
            this.minBid = 3;
			this.name = "Test Of Morgan Le Fey";
			this.imageFilename = "test_of_morgan_le_fey.png";
        }
    }

    public class TestOfTemptation : TestCard {
        public TestOfTemptation(QuestMatch match) : base(match) {
			this.name = "Test Of Temptation";
			this.imageFilename = "test_of_temptation.png";
        }
    }

    public class TestOfTheQuestingBeast : TestCard {
        public TestOfTheQuestingBeast(QuestMatch match) : base(match) {
            // TODO: May be a better way to do this.
            this.minBidQuest = new SearchForTheQuestingBeast(match);
            this.minBid = 4;
			this.name = "Test Of The Questing Beast";
			this.imageFilename = "test_of_the_questing_beast.png";
        }
    }

    public class TestOfValor : TestCard {
        public TestOfValor(QuestMatch match) : base(match) {
			this.name = "Test Of Valor";
			this.imageFilename = "test_of_valor.png";
        }
    }
}