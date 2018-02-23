using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public abstract class PlayerBehaviour {
        public abstract bool ParticipateInTournament(TournamentCard tournamentCard);
        public abstract List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player);
        public abstract bool SponsorQuest(QuestCard questCard, Hand hand);
        public abstract void SetupQuest(QuestCard questCard, Hand hand);
        public abstract bool ParticipateInQuest(QuestCard questCard, Hand hand);
        public abstract List<AdventureCard> NextBid(TestCard testCard, Hand hand);
        public abstract List<Card> DiscardAfterWinningTest();
        public abstract List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand);

        protected static AdventureCard strongestCard(List<BattleCard> cards) {
            int maxBattlePoints = 0;
            BattleCard maxCard = null;
            foreach (BattleCard card in cards) {
                if (card.BattlePoints > maxBattlePoints) {
                    maxCard = card;
                    maxBattlePoints = card.BattlePoints;
                }
            }
            return maxCard;
        }

        protected bool hasDuplicate(List<BattleCard> cards, BattleCard card) {
            foreach (Card c in cards) {
                if (card.GetType() == c.GetType()) {
                    return true;
                }
            }
            return false;
        }

        // for AI deciding to sponsor quest
        public bool promotableThroughQuest(Player player, QuestCard questCard) {
            return (player.Rank.ShieldsToVictory() <= questCard.StageCount);
        }

        protected bool validateQuestStageCards(List<BattleCard>[] questChunks) {
            int lastBattlePoints = 0;

            foreach (List<BattleCard> questChunk in questChunks) {
                BattleArea compareArea = new BattleArea();
                foreach (BattleCard card in questChunk) {
                    compareArea.Add(card);
                }

                if (compareArea.BattlePoints() - 10 <= lastBattlePoints) {
                    return false;
                }
            }

            return true;
        }

        // assuming battleCards is sorted, starting from weakest
        protected List<BattleCard>[] questStageCards(List<BattleCard> battleCards, int size) {
            List<BattleCard>[] cardsToPlay = new List<BattleCard>[size];
            List<Amour> yourAmours = new List<Amour>();
            List<AllyCard> yourAllies = new List<AllyCard>();
            List<WeaponCard> yourWeapons = new List<WeaponCard>();
			
			for(int i = 0; i<size; i++){
				cardsToPlay[i] = new List<BattleCard>();
			}
			
            //split your cards up into amour, allies, and weapons
            foreach (BattleCard card in battleCards) {
                if (card is Amour) {
                    yourAmours.Add((Amour)card);
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
                while (currentBP - previousBP <= 10) {
                    if (yourAllies.Count >= 1) {
                        cardsToPlay[i].Add(yourAllies[0]);
                        yourAllies.Remove(yourAllies[0]);
                    }
                    else if (yourWeapons.Count >= 1) {
                        int index = 0;//the next index that contains a non duplicate weapon
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
    }

    internal class HumanPlayer : PlayerBehaviour {
        public override List<BattleCard> PlayCardsInQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override List<Card> DiscardAfterWinningTest() {
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

        public override List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player) {
            throw new NotImplementedException();
        }

        public override bool SponsorQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }

        public override void SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
    }
}
