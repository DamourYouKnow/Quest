using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    class AIStrategies {
        private const int tournamentTargetBattlePoints = 50;

        internal class Strategy2 : PlayerBehaviour {
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
                int cardsVisited = 0;
                List<AdventureCard> playableCards = hand.AdventureCards;
                List<AdventureCard> cardsToPlay = new List<AdventureCard>();

                while (currentBattlePoints < tournamentTargetBattlePoints && cardsVisited < playableCards.Count) {
                    AdventureCard nextCard = hand.StrongestCard();
                    cardsToPlay.Add(nextCard);
                    currentBattlePoints += nextCard.BattlePoints;
                    cardsVisited++;
                }

                return cardsToPlay;
            }

            public override bool SponsorQuest(QuestCard questCard) {
                throw new NotImplementedException();
            }
        }
    }
}
