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

            public override List<Card> NextBid(TestCard testCard) {
                throw new NotImplementedException();
            }

            public override bool ParticipateInQuest(QuestCard questCard, Hand hand) {
				
				List<AdventureCard> playableCards = hand.AdventureCards;
                List<AdventureCard> cardsToPlay = new List<AdventureCard>();
				int currentBattlePoints = 0;
				/*
				List<AdventureCard> cardsToPlay = new List<AdventureCard>();
				while(currentBattlePoints < 50){
					int indexToPlay = 0;
					int currentIndex = 0;
					int maxBP = 0;
					foreach(AdventureCard card in hand.Cards){
						if(card.BattlePoints > maxBP){
							maxBP = card.BattlePoints;
							indexToPlay = currentIndex;
						}
						currentIndex += 1;
					}
					if (maxBP == 0){
						break;
					}
					currentBattlePoints += maxBP;
					//play/add the card
				}
				*/
				return true;
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
