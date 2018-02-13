using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public class Strategy2 : PlayerBehaviour {
        private const int tournamentTargetBattlePoints = 50;

        public override List<Card> DiscardAfterWinningTest() {
            throw new NotImplementedException();
        }

        public override List<AdventureCard> NextBid(TestCard testCard, Hand hand) {
            int targetBid = testCard.HigestBid;
            List<AdventureCard> bids = new List<AdventureCard>();
            int bidValue = 0;

            foreach (AdventureCard card in hand.AdventureCards) {
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
		
		public override bool ParticipateInQuest(QuestCard questCard, Hand hand) {
				
			List<AdventureCard> yourCards = hand.AdventureCards;
			int totalBattlePoints = 0;
			List<AdventureCard> discardableFoeCards = new List<AdventureCard>();
				
			//sorts your hand by battle points
			//yourCards.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
				
			//filter your hand for non-foes and discardable foes
			//non-discardable foes are ignored
			foreach (AdventureCard card in yourCards) {
				if ((card is FoeCard)
					&&(card.BattlePoints < 25)) {
					discardableFoeCards.Add(card);
				}
				else if (!(card is FoeCard)){
					totalBattlePoints += card.BattlePoints;
				}
			}
				
			//if you are able to increment by 10 per stage
			//and your list of discardable foes has at least 2 foe cards
			if (((totalBattlePoints / questCard.Stages.Count) > 10)
			&&(discardableFoeCards.Count > 1)){
				return true;
			}
			
			return false;
		}
		
		public override List<AdventureCard> PlayCardsInQuest(QuestCard questCard, Hand hand){
			//your current hand
			List<AdventureCard> yourCards = hand.AdventureCards;
			List<AdventureCard> toPlay = new List<AdventureCard>();
			//if current stage is quest: calls NextBid
			if(questCard.Stages[questCard.CurrentStage].MainCard is TestCard){
				TestCard currentTest = questCard.Stages[questCard.CurrentStage].MainCard as TestCard;
				toPlay = this.NextBid(currentTest, hand);
			}
			//if it's the last stage: sort hand and play all until current cards battle points = 0
			else if(questCard.CurrentStage == questCard.Stages.Count){
				//sort hand by battle points
				yourCards.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
				foreach(AdventureCard card in yourCards){
					if(!(card is FoeCard)){
						if(card.BattlePoints == 0){break;}
						else{
							//probably need to check for duplicate weapons
							toPlay.Add(card);
							}
					}
				}
				
			}
			//else, (if not last stage and not test)
			//...not sure how to check how many battle points were played last stage
			//i'll get back to this
			return toPlay;
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
