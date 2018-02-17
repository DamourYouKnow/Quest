﻿using System;
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

            List<BattleCard> yourCards = hand.BattleCards;
            int totalBattlePoints = 0;
            List<BattleCard> discardableFoeCards = new List<BattleCard>();

            //sorts your hand by battle points
            //yourCards.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));

            //filter your hand for non-foes and discardable foes
            //non-discardable foes are ignored
            foreach (BattleCard card in yourCards) {
                if ((card is FoeCard)
                    && (card.BattlePoints < 25)) {
                    discardableFoeCards.Add(card);
                }
                else if (!(card is FoeCard)) {
                    totalBattlePoints += card.BattlePoints;
                }
            }

            //if you are able to increment by 10 per stage
            //and your list of discardable foes has at least 2 foe cards
            if (((totalBattlePoints / questCard.Stages.Count) > 10)
            && (discardableFoeCards.Count > 1)) {
                return true;
            }

            return false;
        }

        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            //your current hand
            List<BattleCard> yourCards = hand.BattleCards;
            List<BattleCard> toPlay = new List<BattleCard>();

			//if it's the last stage: sort hand and play all until current cards battle points = 0
			if (questCard.CurrentStage == questCard.Stages.Count) {
                //sort hand by battle points
                yourCards.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
                foreach (BattleCard card in yourCards) {
                    if (!(card is FoeCard)) {
                        if (card.BattlePoints == 0) {
                            break;
                        } else {
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

        public override List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player) {
            int currentBattlePoints = player.BattlePointsInPlay();
            List<BattleCard> playableCards = player.Hand.BattleCards;
            playableCards.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
            List<BattleCard> cardsToPlay = new List<BattleCard>();

            foreach (BattleCard card in playableCards) {
                if (currentBattlePoints >= tournamentTargetBattlePoints) break;
                if (hasDuplicate(cardsToPlay, card)) continue;
                cardsToPlay.Add(card);
                currentBattlePoints += card.BattlePoints;
            }

            return cardsToPlay;
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {
			//if someone can be promoted by winning
            foreach(Player player in questCard.QuestingPlayers){
				if (promotableThroughQuest(player, questCard)){
					return false;
				}
			}
			
			List<AdventureCard> yourCards = hand.AdventureCards;
			List<FoeCard> yourFoes = new List<FoeCard>();
			
			foreach(AdventureCard card in yourCards){
					if(card is FoeCard){
						yourFoes.Add((FoeCard)card);
					}
			}
			//if you don't have enough foes
			if (yourFoes.Count < questCard.Stages.Count){
				return false;
			}
			yourFoes.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
			for(int i = 1; i < questCard.Stages.Count - 1; i++){
				//if there's not enough foes with increasing battle points
				if(yourFoes[i].BattlePoints <= yourFoes[i-1].BattlePoints){
					return false;
				}
			}
			return true;
        }

        public override bool SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
    }
}
