using System;
using System.Linq;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public class Strategy2 : PlayerBehaviour {
        private const int tournamentTargetBattlePoints = 50;

        public override List<Card> DiscardExcessCards(Hand hand) {
            List<Card> discards = new List<Card>();
            int discarded = 0;
            while (hand.Count - discarded >= 12) {
                discards.Add(hand.Cards[discarded]);
                discarded++;
            }
            return discards;
        }

        public override List<Card> DiscardAfterWinningTest(QuestCard questCard, Hand hand, int discardCount) {
            List<Card> discards = new List<Card>();

            if (questCard.CurrentStage == 1) {

            }
            if (questCard.CurrentStage == 2) {
                List<FoeCard> round1Foes = questCard.GetStage(1).GetCards<FoeCard>();
                foreach (FoeCard handFoe in hand.GetCards<FoeCard>()) {
                    if (round1Foes.FindAll(f => f.Name == handFoe.Name).Count > 0) {
                        discards.Add(handFoe);
                    }
                }
            }

            return discards;
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

            List<BattleCard>[] bestQuestParticipation = this.bestCardsToPlayInQuest(yourCards, questCard.StageCount);
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
			List<AdventureCard>[] stages = CardsToSponsorQuest(hand, questCard.StageCount);
			return validateCardsToSponsorQuest(stages);
        }

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {	
			return CardsToSponsorQuest(hand, questCard.StageCount);
        }
		
		private List<AdventureCard>[] CardsToSponsorQuest(Hand hand, int size) {
			List<AdventureCard>[] stages = new List<AdventureCard>[size];
            List<TestCard> tests = hand.GetCards<TestCard>();
            List<FoeCard> foes = new List<FoeCard>(hand.GetCards<FoeCard>());
            List<WeaponCard> weapons = new List<WeaponCard>(hand.GetCards<WeaponCard>());

            foes.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints)); // Ascending BP.
            weapons.Sort((x, y) => -x.BattlePoints.CompareTo(y.BattlePoints)); // Descending BP.

            int lastStageBP = 0;
            for (int s = 0; s < stages.Length; s++) {
                List<AdventureCard> nextStage = new List<AdventureCard>();
                stages[s] = nextStage;

                if (s + 1 == size - 1 && tests.Count > 0) {
                    nextStage.Add(tests[0]);
                }
				//if last stage
                else if (s + 1 == size) {
                    int currentStageBP = 0;
					if (foes.Count > 0) {
                            nextStage.Add(foes[0]);
                            currentStageBP += foes[0].BattlePoints;
                            foes.RemoveAt(0);
                    }
                    while ((currentStageBP < 40 || currentStageBP <= lastStageBP) && weapons.Count > 0) {
                        if (weapons.Count > 0) {
							int index = 0;
							foreach (AdventureCard card in stages[s]) {
								if (card.ToString() == weapons[index].ToString()) {
									index += 1;
								}
								
								if (index > weapons.Count - 1) {
										return stages;
								}
							}
                            nextStage.Add(weapons[index]);
                            currentStageBP += weapons[index].BattlePoints;
                            weapons.RemoveAt(index);
                        }
                    }
                    lastStageBP = currentStageBP;
                }
				//if not last stage
                else {
                    int currentStageBP = 0;
					if (foes.Count > 0) {
						nextStage.Add(foes[0]);
						currentStageBP += foes[0].BattlePoints;
						foes.RemoveAt(0);
					}
                    while (currentStageBP <= lastStageBP && foes.Count > 0) {
						int index = 0;
						foreach (AdventureCard card in stages[s]) {
                            if (card.ToString() == weapons[index].ToString()) {
                                index += 1;
							}
							
							if (index > weapons.Count - 1) {
                                    return stages;
                                }
						}
                        nextStage.Add(weapons[index]);
						currentStageBP += weapons[index].BattlePoints;
						weapons.RemoveAt(index);
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
		
		private bool validateCardsToSponsorQuest(List<AdventureCard>[] stages) {
            int lastBattlePoints = 0;

            foreach (List<AdventureCard> stage in stages) {
				PlayerArea compareArea = new PlayerArea();
                List<BattleCard> nonTests = new List<BattleCard>();
				bool StageIsTest = false;
				//filter test cards
				foreach(AdventureCard card in stage){
					if(!(card is TestCard)){
						nonTests.Add((BattleCard)card);
					}
					else{
						StageIsTest = true;
						break;
					}
				}
				if(!StageIsTest){
					foreach (BattleCard card in nonTests) {
						compareArea.Add(card);
					}

					if (compareArea.BattlePoints() <= lastBattlePoints) {
						return false;
					}
					lastBattlePoints = compareArea.BattlePoints();
				}
            }

            return true;
        }

        private bool validateCardsToPlayInQuest(List<BattleCard>[] questChunks) {
            int lastBattlePoints = 0;

            foreach (List<BattleCard> questChunk in questChunks) {
                PlayerArea compareArea = new PlayerArea();
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
