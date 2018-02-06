using System;

namespace Quest.Core.Cards{
	public abstract class TournamentCard : StoryCard {
		protected int bonusSheilds;

		public TournamentCard(QuestMatch match) : base(match) {

		}

		public override void Run(){
			
		}
	}

	public class TournamentAtCamelot : TournamentCard {
		public TournamentAtCamelot(QuestMatch match) : base(match) {
			this.name = "Tournament At Camelot";
			this.imageFilename = "tournament_at_camelot.png";
			this.bonusSheilds = 3;
		}
	}

	public class TournamentAtOrkney : TournamentCard {
		public TournamentAtOrkney(QuestMatch match) : base(match) {
			this.name = "Tournament At Orkney";
			this.imageFilename = "tournament_at_orkney.png";
			this.bonusSheilds = 2;
		}
	}

	public class TournamentAtTintagle : TournamentCard {
		public TournamentAtTintagle(QuestMatch match) : base(match) {
			this.name = "Tournament At tintagle";
			this.imageFilename = "tournament_at_tintagle.png";
			this.bonusSheilds = 1;
		}
	}

	public class TournamentAtYork : TournamentCard {
		public TournamentAtYork(QuestMatch match) : base(match) {
			this.name = "Tournament At York";
			this.imageFilename = "tournament_at_York.png";
			this.bonusSheilds = 0;
		}
	}
}