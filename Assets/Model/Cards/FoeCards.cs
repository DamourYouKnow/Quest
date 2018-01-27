using System;

namespace Quest.Core.Cards {
	
	public abstract class FoeCard : AdventureCard {	
		protected int bonusBP;

        public FoeCard(QuestMatch match) : base(match) {

        }
    }
	
	public class BlackKnight : FoeCard{
        public BlackKnight(QuestMatch match) : base(match) {
            this.name = "Black Knight";
			this.imageFilename = "foe_black_knight.png";
			this.battlePoints = 25;
			this.bonusBP = 10;
		}
	}
	
	public class Boar : FoeCard{
        public Boar(QuestMatch match) : base(match) {
            this.name = "Boar";
			this.imageFilename = "foe_boar.png";
			this.battlePoints = 5;
			this.bonusBP = 10;
		}
	}
	
	public class Dragon : FoeCard{
        public Dragon(QuestMatch match) : base(match) {
            this.name = "Dragon";
			this.imageFilename = "foe_dragon.png";
			this.battlePoints = 50;
			this.bonusBP = 20;
		}
	}
	
	public class EvilKnight : FoeCard{
        public EvilKnight(QuestMatch match) : base(match) {
            this.name = "Evil Knight";
			this.imageFilename = "foe_evil_knight.png";
			this.battlePoints = 20;
			this.bonusBP = 10;
		}
	}
	
	public class Giant : FoeCard{
        public Giant(QuestMatch match) : base(match) {
            this.name = "Giant";
			this.imageFilename = "foe_giant.png";
			this.battlePoints = 40;
			this.bonusBP = 0;
		}
	}
	
	public class GreenKnight : FoeCard{
        public GreenKnight(QuestMatch match) : base(match) {
            this.name = "Green Knight";
			this.imageFilename = "foe_green_knight.png";
			this.battlePoints = 25;
			this.bonusBP = 15;
		}
	}
	
	public class Mordred : FoeCard{
        public Mordred(QuestMatch match) : base(match) {
            this.name = "Mordred";
			this.imageFilename = "foe_mordred.png";
			this.battlePoints = 30;
			this.bonusBP = 0;
		}
		//will need to do something for Mordred's ability later
	}
	
	public class RobberKnight : FoeCard{
        public RobberKnight(QuestMatch match) : base(match) {
            this.name = "Robber Knight";
			this.imageFilename = "foe_robber_knight.png"; 
			this.battlePoints = 15;
			this.bonusBP = 0;
		}
	}
	
	public class SaxonKnight : FoeCard{
        public SaxonKnight(QuestMatch match) : base(match) {
            this.name = "Saxon Knight";
			this.imageFilename = "foe_saxon_knight.png";
			this.battlePoints = 15;
			this.bonusBP = 10;
		}
	}
	
	public class Saxons : FoeCard{
        public Saxons(QuestMatch match) : base(match) {
            this.name = "Saxons";
			this.imageFilename = "foe_saxons.png";
			this.battlePoints = 10;
			this.bonusBP = 10;
		}
	}
	
	public class Thieves : FoeCard{
        public Thieves(QuestMatch match) : base(match) {
            this.name = "Thieves";
			this.imageFilename = "foe_thieves.png";
			this.battlePoints = 5;
			this.bonusBP = 0;
		}
	}
	
}