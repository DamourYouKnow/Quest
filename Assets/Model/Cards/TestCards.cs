using System;
using System.Collections.Generic;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class TestCard : StoryCard {
        private int minBid;
        private QuestCard minBidQuest;
        private Dictionary<Player, List<Card>> bids;

        public TestCard(QuestMatch match) : base(match) {
            foreach (Player player in this.match.Players) {
                this.bids.Add(player, new List<Card>());
            }
        }

        public void AddBid(Player player, Card card) {
            this.bids[player].Add(card);
        }

        public void GetBids(Player player) {
            int bids = this.bids[player].Count;
            // TODO: Add bids from cards in play.
        }
    }
}