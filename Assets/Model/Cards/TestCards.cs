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
        }
    }

    public class TestOfTemptation : TestCard {
        public TestOfTemptation(QuestMatch match) : base(match) {
            
        }
    }

    public class TestOfTheQuestingBeast : TestCard {
        public TestOfTheQuestingBeast(QuestMatch match) : base(match) {
            // TODO: May be a better way to do this.
            this.minBidQuest = new SearchForTheQuestingBeast(match);
            this.minBid = 4;
        }
    }

    public class TestOfValor : TestCard {
        public TestOfValor(QuestMatch match) : base(match) {

        }
    }
}