using System;
using System.Linq;
using System.Collections.Generic;
using Quest.Core.Cards;
using Utils;

namespace Quest.Core.Players {
    public class Strategy1 : PlayerBehaviour {
        public override List<Card> DiscardAfterWinningTest(QuestCard questCard, Hand hand, int discardCount) {
            throw new NotImplementedException();
        }

        public override List<Card> DiscardExcessCards(Hand hand) {
			List<Card> discards = new List<Card> ();
			for (int i = 0; i < hand.Count; i++) {
				if (i >= Constants.MaxHandSize) {
					discards.Add (hand.Cards [i]);
				}
			}
			return discards;
        }

        public override List<AdventureCard> NextBid(TestCard testCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInQuest(QuestCard questCard, Hand hand) {
			List<BattleCard> yourCards = hand.BattleCards;
            List<BattleCard> discardableFoeCards = new List<BattleCard>();
			List<BattleCard> WepsAndAllies = new List<BattleCard>();
			
			foreach (BattleCard card in yourCards){
				if(card is FoeCard){
					if(card.BattlePoints < 20){
						discardableFoeCards.Add(card);
					}
				}
				else if((card is WeaponCard)||(card is AllyCard)){
					WepsAndAllies.Add(card);
				}
				
				if((WepsAndAllies.Count/questCard.StageCount >= 2)
					&&(discardableFoeCards.Count >= 2)){
						return true;
					}
			}
			
            return false;
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            foreach (Player player in tournamentCard.Match.Players) {
                if (promotableThroughTournament(player, tournamentCard)) return true;
            }
            return false;
        }

        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
			List<BattleCard> yourCards = hand.BattleCards;
			List<BattleCard> toPlay = new List<BattleCard>();
			List<BattleCard> amours = new List<BattleCard>();
			List<BattleCard> allies = new List<BattleCard>();
			List<BattleCard> weapons = new List<BattleCard>();
			
			//sort in descending order (highest bp first)
			yourCards.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
			
			//filter your cards into amour/ally/weapon
			foreach(BattleCard card in yourCards){
				if(card is Amour){
					amours.Add(card);
				}
				else if(card is AllyCard){
					allies.Add(card);
				}
				//add only unique weapons
				else if(card is WeaponCard){
					bool isDuplicate = false;
					//check for duplicate weapons
					if(weapons.Count > 0){
						foreach(BattleCard wep in weapons){
							if (card.ToString() == wep.ToString()){
								isDuplicate = true;
							}
						}
					}
					if(!isDuplicate){
						weapons.Add(card);
					}
				}
			}
			//if last stage: play all allies and weapons
			if (questCard.CurrentStage == questCard.StageCount) {
                if(allies.Count >= 1){
					foreach(BattleCard ally in allies){
						toPlay.Add(ally);
					}
				}
				if(weapons.Count >= 1){
					foreach(BattleCard wep in weapons){
						toPlay.Add(wep);
					}
				}
            }
			//if not last stage:
			else{
				//just play 1 amour
				if(amours.Count >= 1){
					toPlay.Add(amours[0]);
				}
				//or just 1 ally if you don't have amour
				else if(allies.Count >= 1){
					toPlay.Add(allies[0]);
				}
				//or 2 weapons if you don't have either of the above
				else if(weapons.Count >= 2){
					toPlay.Add(weapons[0]);
					toPlay.Add(weapons[1]);
				}
			}
            return toPlay;
        }

        public override List<BattleCard> PlayCardsInTournament(TournamentCard tournamentCard, Player player) {
            bool strongestHand = false;
            foreach (Player participant in tournamentCard.Participants) {
                if (promotableThroughTournament(participant, tournamentCard)) strongestHand = true;
            }

            Hand hand = player.Hand;
            List<BattleCard> playing = new List<BattleCard>();
            if (strongestHand) {
                playing.AddRange(hand.GetDistinctCards<WeaponCard>().Cast<BattleCard>().ToList());
                playing.AddRange(hand.GetDistinctCards<Amour>().Cast<BattleCard>().ToList());
                playing.AddRange(hand.GetCards<AllyCard>().Cast<BattleCard>().ToList());
            } else {
                HashSet<Type> foundWeaponTypes = new HashSet<Type>();
                foreach (WeaponCard weapon in hand.GetCards<WeaponCard>()) {
                    Type t = weapon.GetType();
                    if (foundWeaponTypes.Contains(t)) {
                        playing.Add(weapon);
                    } else {
                        foundWeaponTypes.Add(t);
                    }
                }
            }

            return playing;
        }

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {
            foreach (Player player in questCard.Match.Players) {
                if (player != hand.Player && promotableThroughQuest(player, questCard)) return false;
            }

            // TODO: Change these functions.
            List<AdventureCard>[] stages = cardsToSponsorQuest(hand, questCard.StageCount);
            return validateCardsToSponsorQuest(stages);
        }

        private List<AdventureCard>[] cardsToSponsorQuest(Hand hand, int stageCount) {
			List<AdventureCard> yourCards = hand.AdventureCards;
			List<AdventureCard>[] cardsToSponsor = new List<AdventureCard>[stageCount];
			List<AdventureCard> foes = new List<AdventureCard>();
			List<AdventureCard> weapons = new List<AdventureCard>();
			List<AdventureCard> tests = new List<AdventureCard>();
			
			//filter cards into foes/weapons/tests
			foreach(AdventureCard card in yourCards){
				
			}
			//foes.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
			
            throw new NotImplementedException();
        }

        private bool validateCardsToSponsorQuest(List<AdventureCard>[] stages) {
            throw new NotImplementedException();
        }
    }

    public class Strategy2 : PlayerBehaviour {
        private const int tournamentTargetBattlePoints = 50;

        public override List<Card> DiscardExcessCards(Hand hand) {
            List<Card> discards = new List<Card>();
            int discarded = 0;
            while (hand.Count - discarded >= 12) {
                discards.Add(hand.Cards[discarded]);
                discarded++;
            }
			
			hand.Player.Match.Log(hand.Player.Username+" has discarded "+Utils.Stringify.CommaList<Card>(discards));
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
			hand.Player.Match.Log(hand.Player.Username+" has discarded "+Utils.Stringify.CommaList<Card>(discards));
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
			
			hand.Player.Match.Log(hand.Player.Username+" bids "+Utils.Stringify.CommaList<AdventureCard>(bids));
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

            List<BattleCard>[] bestQuestParticipation = this.bestCardsToPlayInQuest(questCard, hand);
			bool participate = validateCardsToPlayInQuest(bestQuestParticipation);
			if(participate){
				hand.Player.Match.Log(hand.Player.Username + " will participate in the quest");
			}
			else{
				hand.Player.Match.Log(hand.Player.Username + " will not participate in the quest");
			}
			return participate;
            //return validateCardsToPlayInQuest(bestQuestParticipation);
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
			
			hand.Player.Match.Log(hand.Player.Username+" plays "+Utils.Stringify.CommaList<BattleCard>(playing));
            return playing;
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            tournamentCard.Match.Log(tournamentCard.Match.CurrentPlayer.Username+"joins the tournament");
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
			player.Match.Log(player.Username+" plays "+Utils.Stringify.CommaList<BattleCard>(cardsToPlay));
            return cardsToPlay;
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {	
            foreach (Player player in questCard.Match.Players) {
                if (player != hand.Player && promotableThroughQuest(player, questCard)) {
					hand.Player.Match.Log(hand.Player.Username+" does not sponsor the quest");
					return false;
				}
            }

			List<AdventureCard>[] stages = cardsToSponsorQuest(hand, questCard.StageCount);
			bool sponsor = validateCardsToSponsorQuest(stages);
			if(sponsor){
				hand.Player.Match.Log(hand.Player.Username+" sponsors the quest");
			}
			else{
				hand.Player.Match.Log(hand.Player.Username+" does not sponsor the quest");
			}
			//return validateCardsToSponsorQuest(stages);
			return sponsor;
        }

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {	
			List<AdventureCard>[] toSponsor = cardsToSponsorQuest(hand, questCard.StageCount);
			
			int i = 1;
			foreach(List<AdventureCard> stage in toSponsor){
				hand.Player.Match.Log(hand.Player.Username
				+" sets up stage " + i + " with cards " + Utils.Stringify.CommaList<AdventureCard>(stage));
				i += 1;
			}
			//return cardsToSponsorQuest(hand, questCard.StageCount);
			return toSponsor;
        }
		
        // assuming battleCards is sorted, starting from weakest
        private List<BattleCard>[] bestCardsToPlayInQuest(QuestCard questCard, Hand hand) {
            List<BattleCard>[] stages = new List<BattleCard>[questCard.StageCount];

            List<Amour> amours = new List<Amour>(hand.GetDistinctCards<Amour>());
            List<AllyCard> allies = new List<AllyCard>(hand.GetCards<AllyCard>());
            List<WeaponCard> weapons = new List<WeaponCard>(hand.GetCards<WeaponCard>());

            allies.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));
            weapons.Sort((x, y) => x.BattlePoints.CompareTo(y.BattlePoints));

            for (int i = 0; i < questCard.StageCount; i++) {
                stages[i] = new List<BattleCard>();
            }

            BattleArea lastCards = new BattleArea();
            BattleArea currCards;
            for (int i = 0; i < questCard.StageCount; i++) {
                currCards = new BattleArea();

                BattleArea weaponArea = new BattleArea();
                weapons.ForEach(x => weaponArea.Add(x));
                List<WeaponCard> playableWeapons = weaponArea.GetDistinctCards<WeaponCard>();

                while (currCards.BattlePoints() < lastCards.BattlePoints() + 10 && amours.Count + allies.Count + playableWeapons.Count > 0) {
                    if (i == 0 && amours.Count > 0) {
                        Amour nextAmour = amours[0];
                        stages[i].Add(nextAmour);
                        currCards.Add(nextAmour);
                        amours.Remove(nextAmour);
                        continue;
                    } 

                    // Add ally.
                    else if (allies.Count > 0) {
                        AllyCard nextAlly = allies[0];
                        stages[i].Add(nextAlly);
                        currCards.Add(nextAlly);
                        allies.Remove(nextAlly);
                        continue;
                    }

                    // Add wepon
                    else if (playableWeapons.Count > 0) {
                        WeaponCard nextWeapon = playableWeapons[0];
                        stages[i].Add(nextWeapon);
                        currCards.Add(nextWeapon);
                        playableWeapons.Remove(nextWeapon);
                        weapons.Remove(nextWeapon);
                        continue;
                    }
                }

                lastCards = currCards;
            }
    
            return stages;
        }

        protected List<AdventureCard>[] cardsToSponsorQuest(Hand hand, int size) {
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

        private bool validateCardsToSponsorQuest(List<AdventureCard>[] stages) {
            int lastBattlePoints = 0;

            foreach (List<AdventureCard> stage in stages) {
				BattleArea compareArea = new BattleArea();
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


    public class Strategy3 : PlayerBehaviour {
        public override List<Card> DiscardAfterWinningTest(QuestCard questCard, Hand hand, int discardCount) {
            throw new NotImplementedException();
        }

        public override List<Card> DiscardExcessCards(Hand hand) {
            throw new NotImplementedException();
        }

        public override List<AdventureCard> NextBid(TestCard testCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool ParticipateInTournament(TournamentCard tournamentCard) {
            throw new NotImplementedException();
        }

        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player) {
            throw new NotImplementedException();
        }

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
    }
}
