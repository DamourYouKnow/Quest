using System;
using System.Linq;
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
            List<BattleCard> discardableFoeCards = new List<BattleCard>();
			List<BattleCard> yourNonFoes = new List<BattleCard>();
			int totalBattlePoints = 0;
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
					yourNonFoes.Add(card);//should be sorted by default(weakest first)
                }
            }

            // Do not participate if there is less than 2 discardable foes.
            if (discardableFoeCards.Count < 2) return false;

            List<BattleCard>[] bestQuestParticipation = this.bestCardsToPlayInQuest(hand.BattleCards, questCard.StageCount);
            return validateCardsToPlayInQuest(bestQuestParticipation);
        }

        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            List<BattleCard> playing = new List<BattleCard>();
            List<BattleCard> lastPlay = questCard.GetLastHistory(hand.Player);
            List<BattleCard> playable = playableInQuest(questCard, hand);

            if (questCard.CurrentStage == questCard.StageCount) {
                // Play best valid combination.
                playing = playable;
            } else {
                // Increment by 10.
                int lastBattlePoints = 0;
                lastPlay.ForEach(c => lastBattlePoints += c.BattlePoints);

                int currentBattlePoints = 0;
                foreach (BattleCard card in playable) {
                    currentBattlePoints += card.BattlePoints;
                    playing.Add(card);
                    if (currentBattlePoints >= lastBattlePoints + 10) break;
                }
            }

            return playing;
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

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {
            List<AdventureCard>[] stages = new List<AdventureCard>[questCard.StageCount];

            List<TestCard> tests = hand.GetCards<TestCard>();
            List<FoeCard> foes = new List<FoeCard>(hand.GetCards<FoeCard>());
            List<WeaponCard> weapons = new List<WeaponCard>(hand.GetCards<WeaponCard>());

            foes.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints)); // Ascending BP.
            weapons.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints)); // Descending BP.

            int lastStageBP = 0;
            for (int s = 0; s < stages.Length; s++) {
                List<AdventureCard> nextStage = new List<AdventureCard>();
                stages[s] = nextStage;

                if (s + 1 == questCard.StageCount - 1 && tests.Count > 0) {
                    nextStage.Add(tests[0]);
                }
                else if (s + 1 == questCard.StageCount) {
                    int currentStageBP = 0;
                    while ((currentStageBP < 40 || currentStageBP <= lastStageBP) && weapons.Count + foes.Count > 0) {
                        if (foes.Count > 0) {
                            nextStage.Add(foes[0]);
                            currentStageBP += foes[0].BattlePoints;
                            foes.RemoveAt(0);
                        }
                        else if (weapons.Count > 0) {
                            nextStage.Add(weapons[0]);
                            currentStageBP += weapons[0].BattlePoints;
                            weapons.RemoveAt(0);
                        }
                    }
                    lastStageBP = currentStageBP;
                }
                else {
                    int currentStageBP = 0;
                    while (currentStageBP <= lastStageBP && foes.Count > 0) {
                        nextStage.Add(foes[0]);
                        currentStageBP += foes[0].BattlePoints;
                        foes.RemoveAt(0);
                    }
                    lastStageBP = currentStageBP;
                }
            }

            return stages;
        }

        // assuming battleCards is sorted, starting from weakest
        private List<BattleCard>[] bestCardsToPlayInQuest(List<BattleCard> battleCards, int size) {
            List<BattleCard>[] cardsToPlay = new List<BattleCard>[size];
            List<Amour> yourAmours = new List<Amour>();
            List<AllyCard> yourAllies = new List<AllyCard>();
            List<WeaponCard> yourWeapons = new List<WeaponCard>();
			
            for (int i = 0; i < size; i++) {
                cardsToPlay[i] = new List<BattleCard>();
            }

            //split your cards up into amour, allies, and weapons
            foreach (BattleCard card in battleCards) {
                if (card is Amour) {
                    yourAmours.Add((Amour)card);
					//battleCards.Remove(card);
                }
                else if (card is AllyCard) {
                    yourAllies.Add((AllyCard)card);
                }
                else if (card is WeaponCard) {
                    yourWeapons.Add((WeaponCard)card);
                }
            }
            //cards to use for the first stage
            if (yourAmours.Count >= 1) {
                cardsToPlay[0].Add(yourAmours[0]);
                yourAmours.Remove(yourAmours[0]);
            }
            else if (yourAllies.Count >= 1) {
                cardsToPlay[0].Add(yourAllies[0]);
                yourAllies.Remove(yourAllies[0]);
            }
            else if (yourWeapons.Count >= 1) {
                cardsToPlay[0].Add(yourWeapons[0]);
                yourWeapons.Remove(yourWeapons[0]);
            }
            //start at stage 2
            for (int i = 1; i < size; i++) {
                int previousBP = 0;
                int currentBP = 0;
                foreach (BattleCard card in cardsToPlay[i - 1]) {
                    previousBP += card.BattlePoints;
                }
                //while current stage's BP is still 10 less than the previous stage's BP
                while (currentBP - previousBP < 10) {
					
                    if (yourAllies.Count >= 1) {
                        cardsToPlay[i].Add(yourAllies[0]);
                        currentBP += yourAllies[0].BattlePoints;
                        yourAllies.Remove(yourAllies[0]);
                    }
                    if ((yourWeapons.Count >= 1) && (currentBP - previousBP < 10)) {
                        int index = 0;//the next index that contains a non duplicate weapon
                                      
                        //check for duplicate weapons
                        foreach (BattleCard card in cardsToPlay[i]) {
                            if (card.ToString() == yourWeapons[index].ToString()) {
                                index += 1;
                                //if every single weapon is a duplicate
                                if (index > yourWeapons.Count - 1) {
                                    return cardsToPlay;
                                }
                            }
                                else {
                                    break;
                                }
                        }
                                     
                        cardsToPlay[i].Add(yourWeapons[index]);
                        currentBP += yourWeapons[index].BattlePoints;
                        yourWeapons.Remove(yourWeapons[index]);
                    }
                    else if ((yourAllies.Count == 0)
                        && (yourWeapons.Count == 0)) {
                        return cardsToPlay;
                    }
					
                }
            }
            return cardsToPlay;
        }

        private bool validateCardsToPlayInQuest(List<BattleCard>[] questChunks) {
            int lastBattlePoints = 0;

            foreach (List<BattleCard> questChunk in questChunks) {
                BattleArea compareArea = new BattleArea();
                foreach (BattleCard card in questChunk) {
                    compareArea.Add(card);
                }

                if (compareArea.BattlePoints() - 10 < lastBattlePoints) {
                    return false;
                }
				lastBattlePoints = compareArea.BattlePoints();
            }

            return true;
        }
    }
}
