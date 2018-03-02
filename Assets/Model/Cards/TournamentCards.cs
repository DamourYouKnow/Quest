using System;
using System.Collections.Generic;
using System.Linq;
using Quest.Core.Players;

namespace Quest.Core.Cards{
	public abstract class TournamentCard : StoryCard {
		protected int bonusSheilds;
		protected List<Player> participants;
		protected Player firstPlayer;
		protected int firstPlayerNum;
		protected int allAsked;

		public TournamentCard(QuestMatch match) : base(match) {
			this.participants = new List<Player>();
			this.allAsked = 0;
		}

		public Player FirstPlayer {
			get { return this.firstPlayer; }
			set { this.firstPlayer = value; }
		}

		public int FirstPlayerNum {
			get { return this.firstPlayerNum; }
			set { this.firstPlayerNum = value; }
		}

		public int AllAsked {
			get { return this.allAsked; }
			set { this.allAsked = value; }
		}
			

        public List<Player> Participants {
            get { return this.participants; }
            set { this.participants = value; }
        }

        public int Shields {
			get { return this.participants.Count + this.bonusSheilds; }
        }

        public override void Run() {
			this.firstPlayer = this.match.CurrentPlayer;
			for (int i = 0; i < this.match.Players.Count; i++) {
				if (this.match.Players [i] == this.firstPlayer) {
					this.firstPlayerNum = i;
					break;
				}
			}
			if (AllAsked == 0) {
				this.requestParticipation ();
			} else {
				playTournament ();
			}
		//	this.match.EndStory ();
		}
			

		public void playTournament (){
			this.match.Log ("playing tournament");
			this.match.State = MatchState.PLAY_TOURNAMENT;

			this.match.Wait ();
		}

		public void requestParticipation(){
			this.match.Log ("Requesting Participants");
			this.match.State = MatchState.START_TOURNAMENT;

			this.match.Wait ();
		}
	}

	public class TournamentAtCamelot : TournamentCard {
		public TournamentAtCamelot(QuestMatch match) : base(match) {
			this.name = "Tournament At Camelot";
			this.imageFilename = "tournament_at_camelot";
			this.bonusSheilds = 3;
		}

	}

	public class TournamentAtOrkney : TournamentCard {
		public TournamentAtOrkney(QuestMatch match) : base(match) {
			this.name = "Tournament At Orkney";
			this.imageFilename = "tournament_at_orkney";
			this.bonusSheilds = 2;
		}
	}

	public class TournamentAtTintagle : TournamentCard {
		public TournamentAtTintagle(QuestMatch match) : base(match) {
			this.name = "Tournament At Tintagel";
			this.imageFilename = "tournament_at_tintagel";
			this.bonusSheilds = 1;
		}
	}

	public class TournamentAtYork : TournamentCard {
		public TournamentAtYork(QuestMatch match) : base(match) {
			this.name = "Tournament At York";
			this.imageFilename = "tournament_at_York";
			this.bonusSheilds = 0;
		}
	}
}