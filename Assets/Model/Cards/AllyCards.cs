using System;

namespace Quest.Core.Cards {
    public abstract class AllyCard : AdventureCard {
        public AllyCard(QuestMatch match) : base(match) {

        }
    }

    public class KingArthur : AllyCard {
        public KingArthur(QuestMatch match) : base(match) {
            this.name = "King Arthur";
            this.imageFilename = "ally_king_arthur.png";
            this.battlePoints = 10;
        }
    }

    public class KingPellinore : AllyCard {
        public KingPellinore(QuestMatch match) : base(match) {
            this.name = "King Pellinore";
            this.imageFilename = "ally_king_pellinore.png";
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
            this.imageFilename = "ally_sir_galahad.png";
            this.battlePoints = 15;
        }
    }

    public class SirGawain : AllyCard {
        public SirGawain(QuestMatch match) : base(match) {
            this.name = "Sir Gawain";
            this.imageFilename = "ally_sir_gawain.png";
            this.battlePoints = 10;
        }
    }

    public class SirLancelot : AllyCard {
        public SirLancelot(QuestMatch match) : base(match) {
            this.name = "Sir Lancelot";
            this.imageFilename = "ally_sir_lancelot.png";
            this.battlePoints = 15;
        }
    }

    public class SirPercival : AllyCard {
        public SirPercival(QuestMatch match) : base(match) {
            this.name = "Sir Percival";
            this.imageFilename = "ally_sir_percival.png";
            this.battlePoints = 5;
        }
    }

    public class SirTristan : AllyCard {
        public SirTristan(QuestMatch match) : base(match) {
            this.name = "Sir Tristan";
            this.imageFilename = "ally_sir_tristan.png";
            this.battlePoints = 10;
        }
    }
}