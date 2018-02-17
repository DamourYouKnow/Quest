using System;

namespace Quest.Core.Cards{

	public abstract class WeaponCard : BattleCard{

		public WeaponCard (QuestMatch match) : base(match){
		}
			
	}

	public class Horse : WeaponCard{
		public Horse(QuestMatch match) : base(match) {
			this.name = "Horse";
			this.imageFilename = "weapon_horse.png";
			this.battlePoints = 10;
		}
	}

	public class Sword : WeaponCard{
		public Sword(QuestMatch match) : base(match) {
			this.name = "Sword";
			this.imageFilename = "weapon_sword.png";
			this.battlePoints = 10;
		}
	}

	public class Dagger : WeaponCard{
		public Dagger(QuestMatch match) : base(match) {
			this.name = "Dagger";
			this.imageFilename = "weapon_dagger.png";
			this.battlePoints = 5;
		}
	}

	public class Excalibur : WeaponCard{
		public Excalibur(QuestMatch match) : base(match) {
			this.name = "Excalibur";
			this.imageFilename = "weapon_excalibur.png";
			this.battlePoints = 30;
		}
	}

	public class Lance : WeaponCard{
		public Lance(QuestMatch match) : base(match) {
			this.name = "Lance";
			this.imageFilename = "weapon_lance.png";
			this.battlePoints = 20;
		}
	}

	public class BattleAx : WeaponCard{
		public BattleAx(QuestMatch match) : base(match) {
			this.name = "Battle-Ax";
			this.imageFilename = "weapon_battleaxe.png";
			this.battlePoints = 15;
		}
	}


}