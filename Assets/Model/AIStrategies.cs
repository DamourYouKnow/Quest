using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    class AIStrategies {
        internal class Strategy2 : PlayerBehaviour {
            public override List<Card> DiscardAfterWinningTest() {
                throw new NotImplementedException();
            }

            public override List<Card> NextBid(TestCard testCard) {
                throw new NotImplementedException();
            }

            public override bool ParticipateInQuest(QuestCard questCard) {
                throw new NotImplementedException();
            }

            public override bool ParticipateInTournament(TournamentCard tournamentCard) {
                throw new NotImplementedException();
            }

            public override bool SponsorQuest(QuestCard questCard) {
                throw new NotImplementedException();
            }
        }
    }
}
