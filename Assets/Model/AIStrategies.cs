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

            List<BattleCard> yourCards = hand.BattleCards;
            int totalBattlePoints = 0;
            List<BattleCard> discardableFoeCards = new List<BattleCard>();
			//sort hand by weakest
			yourCards.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
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
			if (discardableFoeCards.Count >= 2){
				if ((questCard.StageCount == 2)&&(totalBattlePoints >= 10)){
					return true;
				}
				else if ((questCard.StageCount == 3)&&(totalBattlePoints >= 30)){
					return true;
				}
				else if ((questCard.StageCount == 4)&&(totalBattlePoints >= 60)){
					return true;
				}
				else if ((questCard.StageCount == 5)&&(totalBattlePoints >= 100)){
					return true;
				}
			}
            return false;
        }

        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            //your current hand
            List<BattleCard> yourCards = hand.BattleCards;
            List<BattleCard> toPlay = new List<BattleCard>();

			//if it's the last stage: sort hand and play all until current cards battle points = 0
			if (questCard.CurrentStage == questCard.StageCount) {
                //sort hand by battle points
                yourCards.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
                foreach (BattleCard card in yourCards) {
                    if (!(card is FoeCard)) {
                        if (card.BattlePoints == 0) {
                            break;
                        } else {
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
			List<AdventureCard> yourCards = hand.AdventureCards;
			List<FoeCard> yourFoes = new List<FoeCard>();
			List<WeaponCard> yourWeapons = new List<WeaponCard>();
			int yourTestsCount = 0;
			int unplayableFoes = 0;
			int finalFoeBP = 0;
			
			//if someone can be promoted by winning
            foreach(Player player in questCard.Participants){
				if (promotableThroughQuest(player, questCard)){
					return false;
				}
			}
			
			foreach(AdventureCard card in yourCards){
					if(card is FoeCard){
						yourFoes.Add((FoeCard)card);
					}
					else if(card is TestCard){
						yourTestsCount += 1;
					}
					else if(card is WeaponCard){
						//Check for duplicate weapons (by card name)
						bool isDuplicate = false;
						foreach(WeaponCard weapon in yourWeapons){
							if (card.ToString() == weapon.ToString()){
								isDuplicate = true;
							}
						}
						if(!isDuplicate){
							yourWeapons.Add((WeaponCard)card);
						}
					}
			}
			
			if(yourFoes.Count > 0){
				//sort foes, weakest first
				yourFoes.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
				finalFoeBP = yourFoes[yourFoes.Count - 1].BattlePoints;
			}
			//if theres no foes in hand
			else{
				return false;
			}
			/*
			foreach(WeaponCard weapon in yourWeapons){
				if(finalFoeBP >= 40){
					break;
				}
				finalFoeBP += weapon.BattlePoints;
			}
			
			if (finalFoeBP < 40){
				return false;
			}
			*/
			
			//if you don't have a test card in hand
			if (yourTestsCount == 0){
				//if you don't have enough foes
				//if (yourFoes.Count < questCard.Stages.Count){
				if (yourFoes.Count < questCard.StageCount){
					return false;
				}
				
				for(int i = 1; i < yourFoes.Count; i++){
					//if there's not enough foes with increasing battle points
					if(yourFoes[i].BattlePoints <= yourFoes[i-1].BattlePoints){
						unplayableFoes += 1;
					}
				}
				if(yourFoes.Count - unplayableFoes + yourWeapons.Count < questCard.StageCount){
					return false;
				}
				
			}
			
			//if you have a test card in hand
			else if (yourTestsCount >= 1){
			//if you don't have enough foes
				//if (yourFoes.Count < questCard.Stages.Count - 1){
				if (yourFoes.Count < questCard.StageCount - 1){
					return false;
				}
				for(int i = 1; i < yourFoes.Count; i++){
					//if there's not enough foes with increasing battle points
					if(yourFoes[i].BattlePoints <= yourFoes[i-1].BattlePoints){
						unplayableFoes += 1;
					}
				}
				if(yourFoes.Count - unplayableFoes + yourWeapons.Count < questCard.StageCount - 1){
					return false;
				}
			}
			
			return true;
        }

        public override void SetupQuest(QuestCard questCard, Hand hand) {
			//i'm not actually sure if we're passing questCard by reference or value
			//but if it's by value i think something like 'stage.Add(...)' wouldn't work,
			//either way it's no big deal (i could just return a list of List<BattleCard>)
			
			//not sure if i need to make this 'if' statement here
			if(SponsorQuest(questCard, hand)){
				List<AdventureCard> yourCards = hand.AdventureCards;
				List<FoeCard> yourFoes = new List<FoeCard>();
				List<WeaponCard> yourWeapons = new List<WeaponCard>();
				List<TestCard> yourTests = new List<TestCard>();
				
				foreach(AdventureCard card in yourCards){
						if(card is FoeCard){
							yourFoes.Add((FoeCard)card);
						}
						else if(card is TestCard){
							yourTests.Add((TestCard)card);
						}
						else if(card is WeaponCard){
							//Check for duplicate weapons (by card name)
							bool isDuplicate = false;
							foreach(WeaponCard weapon in yourWeapons){
								if (card.ToString() == weapon.ToString()){
									isDuplicate = true;
								}
							}
							if(!isDuplicate){
								yourWeapons.Add((WeaponCard)card);
							}
						}
				}
				//sort foes, starting with the weakest
				yourFoes.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
				//sort weapons by strongest
				yourWeapons.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints));
				foreach(QuestArea stage in questCard.Stages){
					//if last stage
					if(stage == questCard.Stages[questCard.StageCount]){
						//add the strongest foe
						stage.Add(yourFoes[yourFoes.Count]);
						while(stage.BattlePoints() < 40){
							stage.Add(yourWeapons[0]);
							yourWeapons.Remove(yourWeapons[0]);
							//i believe yourWeapons[1] will be shifted over to 0
						}
					}
					else if(stage == questCard.Stages[questCard.StageCount - 1]){
						//if you have a test card
						if(yourTests.Count >= 1){
							stage.Add(yourTests[0]);
						}
						//if you don't
						else{
							stage.Add(yourFoes[0]);
							yourFoes.Remove(yourFoes[0]);
						}
					}
					else{
						stage.Add(yourFoes[0]);
						yourFoes.Remove(yourFoes[0]);
					}
				}
			}
        }
    }
}
