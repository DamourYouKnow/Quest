﻿using System;
using System.Collections.Generic;
using Quest.Core.Cards;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class EventCard : StoryCard {
        public EventCard(QuestMatch match) : base(match) {

        }
    }

    public class ChivalrousDeedEvent : EventCard {
        public ChivalrousDeedEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            List<Player> lowestRanked = Player.LowestShields(this.match.Players);
            foreach (Player player in lowestRanked) {
                player.Rank.AddShields(3);
            }
        }
    }

    public class CourtCalledEvent : EventCard {
        public CourtCalledEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            throw new NotImplementedException();
        }
    }

    public class CallToArmsEvent : EventCard {
        public CallToArmsEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            throw new NotImplementedException();
        }
    }

    public class RecognitionEvent : EventCard {
        public RecognitionEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            throw new NotImplementedException();
        }
    }

    public class PlagueEvent : EventCard {
        public PlagueEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            throw new NotImplementedException();
        }
    }

    public class PoxEvent : EventCard {
        public PoxEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            throw new NotImplementedException();
        }
    }

    public class ProsperityEvent : EventCard {
        public ProsperityEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            foreach (Player player in this.match.Players) {
                player.Draw(this.match.AdventureDeck, 2);
            }
        }
    }

    public class QueensFavourEvent : EventCard {
        public QueensFavourEvent(QuestMatch match) : base(match) {

        }

        public override void run() {
            List<Player> lowestRanked = Player.LowestRanked(this.match.Players);
            foreach (Player player in lowestRanked) {
                player.Draw(this.match.AdventureDeck, 2);
            }
        }
    }
}
