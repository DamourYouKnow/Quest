using System;
using Quest.Core.Cards;
using Quest.Core.Players;

namespace Quest.Core.Events {
    public abstract class EventCard : StoryCard {
        public EventCard(QuestMatch match) : base(match) {

        }

        abstract public void RunEvent();
    }

    public class ChivalrousDeedEvent : EventCard {
        public ChivalrousDeedEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class CourtCalledEvent : EventCard {
        public CourtCalledEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class CallToArmsEvent : EventCard {
        public CallToArmsEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class RecognitionEvent : EventCard {
        public RecognitionEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class PlagueEvent : EventCard {
        public PlagueEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class PoxEvent : EventCard {
        public PoxEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class ProsperityEvent : EventCard {
        public ProsperityEvent(QuestMatch match) : base(match) {

        }

        public override void RunEvent() {
            foreach (Player player in this.match.Players) {
                player.Draw(this.match.AdventureDeck, 2);
            }
        }
    }
}
