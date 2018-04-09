using System;
using Quest.Core.Players;

namespace Quest.Core.Cards {
    public abstract class AllyCard : BattleCard {
        public AllyCard(QuestMatch match) : base(match) {

        }
    }

    public class KingArthur : AllyCard {
        public KingArthur(QuestMatch match) : base(match) {
            this.name = "King Arthur";
            this.imageFilename = "ally_king_arthur";
            this.battlePoints = 10;
            this.freeBids = 2;
        }
    }

    public class KingPellinore : AllyCard {
        public KingPellinore(QuestMatch match) : base(match) {
            this.name = "King Pellinore";
            this.imageFilename = "ally_king_pellinore";
            this.battlePoints = 10;
        }
    }

    public class Merlin : AllyCard {
        public Merlin(QuestMatch match) : base(match) {
            throw new NotImplementedException();
        }
    }

    public class QueenGuinevere : AllyCard {
        public QueenGuinevere(QuestMatch match) : base(match) {
            throw new NotImplementedException();
        }
    }

    public class QueenIseult: AllyCard {
        public QueenIseult(QuestMatch match) : base(match) {
            throw new NotImplementedException();
        }
    }

    public class SirGalahad : AllyCard {
        public SirGalahad(QuestMatch match) : base(match) {
            this.name = "Sir Galahad";
            this.imageFilename = "ally_sir_galahad";
            this.battlePoints = 15;
        }
    }

    public class SirGawain : AllyCard {
        public SirGawain(QuestMatch match) : base(match) {
            this.name = "Sir Gawain";
            this.imageFilename = "ally_sir_gawain";
            this.battlePoints = 10;
        }

        public override int BattlePoints {
            get {
                if (this.match.CurrentStory is TestOfTheGreenKnight) {
                    return 20;
                }
                else {
                    return this.battlePoints;
                }
            }
        }
    }

    public class SirLancelot : AllyCard {
        public SirLancelot(QuestMatch match) : base(match) {
            this.name = "Sir Lancelot";
            this.imageFilename = "ally_sir_lancelot";
            this.battlePoints = 15;
        }

        public override int BattlePoints {
            get {
                if (this.match.CurrentStory is DefendTheQueensHonor) {
                    return 25;
                }
                else {
                    return this.battlePoints;
                }
            }
        }
    }

    public class SirPercival : AllyCard {
        public SirPercival(QuestMatch match) : base(match) {
            this.name = "Sir Percival";
            this.imageFilename = "ally_sir_percival";
            this.battlePoints = 5;
        }

        public override int BattlePoints {
            get {
                if (this.match.CurrentStory is SearchForTheHolyGrail) {
                    return 20;
                } else {
                    return this.battlePoints;
                }
            }
        }
    }

    public class SirTristan : AllyCard {
        public SirTristan(QuestMatch match) : base(match) {
            this.name = "Sir Tristan";
            this.imageFilename = "ally_sir_tristan";
            this.battlePoints = 10;
        }

        public override int BattlePoints {
            get {
                Player player = this.match.PlayerWithCardOnBoard(this);

                if (player != null) {
                    if (player.BattleArea.Cards.FindAll((x) => x is QueenIseult).Count > 0) {
                        return 20;
                    }
                }

                return this.battlePoints;
            }
        }
    }
}