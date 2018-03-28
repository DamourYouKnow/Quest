using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class TournamentCard : StoryCard {
		protected int bonusShields;
        protected int maxParticipants;
		protected List<Player> participants;

		public TournamentCard(QuestMatch match) : base(match) {
            this.participants = new List<Player>(match.Players);
            this.maxParticipants = this.participants.Count;
		}

        public List<Player> Participants {
            get { return this.participants; }
            set { this.participants = value; }
        }

        public int Shields {
			get { return this.participants.Count + this.bonusShields; }
        }

        public override void Run() {
			throw new NotImplementedException();
		}

        private List<Player> getWinners() {
            return Player.HighestBattlePoints(this.participants);
        }
	}

	public class TournamentAtCamelot : TournamentCard {
		public TournamentAtCamelot(QuestMatch match) : base(match) {
			this.name = "Tournament At Camelot";
			this.imageFilename = "tournament_at_camelot";
			this.bonusShields = 3;
		}
	}

	public class TournamentAtOrkney : TournamentCard {
		public TournamentAtOrkney(QuestMatch match) : base(match) {
			this.name = "Tournament At Orkney";
			this.imageFilename = "tournament_at_orkney";
			this.bonusShields = 2;
		}
	}

	public class TournamentAtTintagle : TournamentCard {
		public TournamentAtTintagle(QuestMatch match) : base(match) {
			this.name = "Tournament At Tintagel";
			this.imageFilename = "tournament_at_tintagel";
			this.bonusShields = 1;
		}
	}

	public class TournamentAtYork : TournamentCard {
		public TournamentAtYork(QuestMatch match) : base(match) {
			this.name = "Tournament At York";
			this.imageFilename = "tournament_at_York";
			this.bonusShields = 0;
		}
	}
}