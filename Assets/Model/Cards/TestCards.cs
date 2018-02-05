using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class TestCard : AdventureCard {
        protected int minBid = 0;
        protected QuestCard minBidQuest = null;
        protected Dictionary<Player, List<Card>> bids;

        public TestCard(QuestMatch match) : base(match) {
            this.bids = new Dictionary<Player, List<Card>>();
            foreach (Player player in this.match.Players) {
                this.bids.Add(player, new List<Card>());
            }
        }

        public void AddBid(Player player, Card card) {
            this.bids[player].Add(card);
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