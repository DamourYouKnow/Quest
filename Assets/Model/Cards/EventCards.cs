using System;
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
			this.name = "Chivalrous Deed";
			this.imageFilename = "event_chivalrous_deed.png";
        }

        public override void Run() {
            List<Player> lowestRanked = Player.LowestShields(this.match.Players);
            foreach (Player player in lowestRanked) {
                player.Rank.AddShields(3);
            }
        }
    }

    public class CourtCalledEvent : EventCard {
        public CourtCalledEvent(QuestMatch match) : base(match) {
			this.name = "Court Called To Camelot";
			this.imageFilename = "event_court_called_to_camelot.png";
        }

        public override void Run() {
            foreach (Player player in this.match.Players) {
                List<Card> removeCards = new List<Card>();
                foreach (Card card in player.BattleArea.Cards) {
                    if (card is AllyCard) {
                        removeCards.Add(card);
                    }
                }
                player.BattleArea.Transfer(player.Hand, removeCards);
                player.Discard(removeCards);
            }
        }
    }

    public class CallToArmsEvent : EventCard {
        public CallToArmsEvent(QuestMatch match) : base(match) {
			this.name = "King's Call To Arms";
			this.imageFilename = "event_kings_call_to_arms.png";
        }

        public override void Run() {
            throw new NotImplementedException();
        }
    }

    public class RecognitionEvent : EventCard {
        public RecognitionEvent(QuestMatch match) : base(match) {
			this.name = "King's Recognition";
			this.imageFilename = "event_kings_recognition.png";
        }

        public override void Run() {
            throw new NotImplementedException();
        }
    }

    public class PlagueEvent : EventCard {
        public PlagueEvent(QuestMatch match) : base(match) {
			this.name = "Plauge";
			this.imageFilename = "event_plauge.png";
        }

        public override void Run() {
            Player drawingPlayer = this.match.PlayerWithCard(this);
            if (drawingPlayer != null) {
                drawingPlayer.Rank.RemoveShields(2);
            }
        }
    }

    public class PoxEvent : EventCard {
        public PoxEvent(QuestMatch match) : base(match) {
			this.name = "Pox";
			this.imageFilename = "event_pox.png";
        }

        public override void Run() {
            Player drawingPlayer = this.match.PlayerWithCard(this);
            foreach (Player player in this.match.Players) {
                if (player != drawingPlayer) {
                    player.Rank.RemoveShields(1);
                }
            }
        }
    }

    public class ProsperityEvent : EventCard {
        public ProsperityEvent(QuestMatch match) : base(match) {
			this.name = "Prosperity Throughout The Relm";
			this.imageFilename = "event_prosperity_throughout_the_relm.png";
        }

        public override void Run() {
            foreach (Player player in this.match.Players) {
                player.Draw(this.match.AdventureDeck, 2);
            }
        }
    }

    public class QueensFavourEvent : EventCard {
        public QueensFavourEvent(QuestMatch match) : base(match) {
			this.name = "Queen's Favour";
			this.imageFilename = "event_queens_favor.png";
        }

        public override void Run() {
            List<Player> lowestRanked = Player.LowestRanked(this.match.Players);
            foreach (Player player in lowestRanked) {
                player.Draw(this.match.AdventureDeck, 2);
            }
        }
    }
}
