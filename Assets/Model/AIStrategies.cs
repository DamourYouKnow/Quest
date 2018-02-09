using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public class Strategy2 : PlayerBehaviour {
        private const int tournamentTargetBattlePoints = 50;

        public override List<Card> DiscardAfterWinningTest() {
            throw new NotImplementedException();
        }

        public override List<Card> NextBid(TestCard testCard, Hand hand) {
            int targetBid = testCard.HigestBid;
            List<Card> bids = new List<Card>();
            int bidValue = 0;

            foreach (Card card in hand.Cards) {
                if (card is FoeCard) {
                    bids.Add(card);
                    bidValue += card.BidValue;
                }
            }

            if (bidValue <= targetBid) {
                bids.Clear();
            }
            return bids;
        }

        public override bool ParticipateInQuest(QuestCard questCard) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            return true;
        }

        public override List<AdventureCard> PlayCardsInTournament(TournamentCard TournamentCard, Hand hand) {
            int currentBattlePoints = 0;
            List<AdventureCard> playableCards = hand.AdventureCards;
            playableCards.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
            List<AdventureCard> cardsToPlay = new List<AdventureCard>();

            foreach (AdventureCard card in playableCards) {
                if (currentBattlePoints >= tournamentTargetBattlePoints) continue;
                cardsToPlay.Add(card);
                currentBattlePoints += card.BattlePoints;
            }

            return cardsToPlay;
        }

        public override bool SponsorQuest(QuestCard questCard) {
            throw new NotImplementedException();
        }
    }
}
