using System;
using System.Collections.Generic;
using Quest.Core.Cards;

namespace Quest.Core.Players {
    public abstract class PlayerBehaviour {
        public abstract bool ParticipateInTournament(TournamentCard tournamentCard);
        public abstract List<BattleCard> PlayCardsInTournament(TournamentCard TournamentCard, Player player);
        public abstract bool SponsorQuest(QuestCard questCard, Hand hand);
        public abstract List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand);
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

        public override List<AdventureCard>[] SetupQuest(QuestCard questCard, Hand hand) {
            throw new NotImplementedException();
        }
    }
}
