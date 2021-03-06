using System;
using Quest.Core.Players;

namespace Quest.Core.Cards {
	
	public abstract class FoeCard : BattleCard {	
		protected int bonusBP;
		
        public FoeCard(QuestMatch match) : base(match) {

        }

		public override int BattlePoints {
			get {
				int total = base.BattlePoints;
				QuestCard story;
				if (this.match.CurrentStory is QuestCard) {
					story = this.match.CurrentStory as QuestCard;
					if(story.QuestFoes.Contains(this.GetType())){
						total += this.bonusBP;
					}
				}
				return total;
			}
		}
    }
	
	public class BlackKnight : FoeCard{
        public BlackKnight(QuestMatch match) : base(match) {
            this.name = "Black Knight";
			this.imageFilename = "foe_black_knight";
			this.battlePoints = 25;
			this.bonusBP = 10;
			
		}
	}
	
	public class Boar : FoeCard{
        public Boar(QuestMatch match) : base(match) {
            this.name = "Boar";
			this.imageFilename = "foe_boar";
			this.battlePoints = 5;
			this.bonusBP = 10;
		}
	}
	
	public class Dragon : FoeCard{
        public Dragon(QuestMatch match) : base(match) {
            this.name = "Dragon";
			this.imageFilename = "foe_dragon";
			this.battlePoints = 50;
			this.bonusBP = 20;
		}
	}
	
	public class EvilKnight : FoeCard{
        public EvilKnight(QuestMatch match) : base(match) {
            this.name = "Evil Knight";
			this.imageFilename = "foe_evil_knight";
			this.battlePoints = 20;
			this.bonusBP = 10;
		}
	}
	
	public class Giant : FoeCard{
        public Giant(QuestMatch match) : base(match) {
            this.name = "Giant";
			this.imageFilename = "foe_giant";
			this.battlePoints = 40;
			this.bonusBP = 0;
		}
	}
	
	public class GreenKnight : FoeCard{
        public GreenKnight(QuestMatch match) : base(match) {
            this.name = "Green Knight";
			this.imageFilename = "foe_green_knight";
			this.battlePoints = 25;
			this.bonusBP = 15;
		}
	}
	
	public class Mordred : FoeCard{
        public Mordred(QuestMatch match) : base(match) {
            this.name = "Mordred";
			this.imageFilename = "foe_mordred";
			this.battlePoints = 30;
			this.bonusBP = 0;
		}
		
		//SpecialAbility discards a target ally and Mordred
		//Should work since there's only 1 of each ally card 
		public void SpecialAbility(AllyCard card){
			//problem: this might discard another player's Mordred
			//but at least for now let's just have it discard any Mordred
			//just to confirm that this function "works"
			Player targetPlayer = this.match.PlayerWithCardOnBoard(card);
			Player mordredPlayer = this.match.PlayerWithCard(this);
			targetPlayer.Discard(card);
			mordredPlayer.Discard(this);
		}
	}
	
	public class RobberKnight : FoeCard{
        public RobberKnight(QuestMatch match) : base(match) {
            this.name = "Robber Knight";
			this.imageFilename = "foe_robber_knight"; 
			this.battlePoints = 15;
			this.bonusBP = 0;
		}
	}
	
	public class SaxonKnight : FoeCard{
        public SaxonKnight(QuestMatch match) : base(match) {
            this.name = "Saxon Knight";
			this.imageFilename = "foe_saxon_knight";
			this.battlePoints = 15;
			this.bonusBP = 10;
		}
	}
	
	public class Saxons : FoeCard{
        public Saxons(QuestMatch match) : base(match) {
            this.name = "Saxons";
			this.imageFilename = "foe_saxons";
			this.battlePoints = 10;
			this.bonusBP = 10;
		}
	}
	
	public class Thieves : FoeCard{
        public Thieves(QuestMatch match) : base(match) {
            this.name = "Thieves";
			this.imageFilename = "foe_thieves";
			this.battlePoints = 5;
			this.bonusBP = 0;
		}
	}
	
}