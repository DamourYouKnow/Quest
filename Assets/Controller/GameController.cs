//"connected" to the GameController game object
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Quest.Core;
using Quest.Core.Players;
using Quest.Core.Cards;
using Quest.Core.Scenarios;
using Utils;

namespace Quest.Core {
    public enum Scenario {
        LocalGame,
        Scenario1,
        Scenario2
    }

	public class GameController : MonoBehaviour {
		QuestMatch gm;
		Logger logger;
		bool waiting;
		Player discardingPlayer;
		GameCardArea discardArea;
		GameCardArea gameCardAreaSave;
		QuestGameCardArea questGameCardAreaSave;
		CardArea handAreaSave;
		CardArea battleAreaSave;
		int numPlayers;
		GameObject ConfButton;
		GameObject ConfText;
		GameObject GameOtherArea;
		GameObject GameBattleArea;
		GameObject GameHandArea;
		List<OpponentState> Opponents;

		//Specifies if a scene has been setup yet
		//Necessary because scenes are not loaded when Load is run, but rather at next update cycle
		bool sceneSet;
        private Scenario scenario;

		public Logger Logger {
			get { return this.logger; }
		}
		public QuestMatch GM {
			get { return this.gm; }
		}

		//Used to copy GameController between scenes
		private void init(GameController gc){
			this.gm = gc.gm;
			this.logger = gc.logger;
			this.sceneSet = gc.sceneSet;
			this.waiting = gc.waiting;
			this.discardingPlayer = gc.discardingPlayer;
			this.discardArea = gc.discardArea;
			this.gameCardAreaSave = gc.gameCardAreaSave;
			this.questGameCardAreaSave = gc.questGameCardAreaSave;
			this.handAreaSave = gc.handAreaSave;
			this.battleAreaSave = gc.battleAreaSave;
			this.numPlayers = gc.numPlayers;
			this.Opponents = gc.Opponents;
			this.scenario = gc.scenario;
		}

		//Awake is called before Start function, guaranteeing we'll have it setup for other scripts
		void Awake(){
			//This setup ensures only one GameController is running at a time.
			//If a new one comes along, copy all settings, destroy old one
			GameObject gc = GameObject.FindGameObjectWithTag ("GameController");
			if (gc != null) {
				this.init (gc.GetComponent<GameController>());
				Destroy (gc);
			}
			else {
                logger = new Logger();
                gm = new QuestMatch(this, logger);
     
  				sceneSet = false;
				waiting = false;
				this.discardingPlayer = null;
				this.discardArea = null;
				this.gameCardAreaSave = null;
				this.questGameCardAreaSave = null;
				this.handAreaSave = null;
				this.battleAreaSave = null;
				numPlayers = 0;
				this.Opponents = new List<OpponentState> ();
			}
			DontDestroyOnLoad (this);
			this.tag = "GameController";
			Logger.Log ("GameController.Awake()");
		}

		// Use this for initialization
		void Start() {
            // Sample prompt code.
            //GameObject promptObj = new GameObject("Prompt");
            //Prompt prompt = promptObj.AddComponent<Prompt>();
            //prompt.Message = "Hello this is a test prompt. Set my yes and no event handlers and click my buttons!";
            //prompt.OnNoClick = () => { Debug.Log("No clicked"); };
            //prompt.OnYesClick = () => { Debug.Log("Yes clicked"); };
        }
		
		// Update is called once per frame
		void Update () {
			if (sceneSet) {
				foreach (OpponentState opp in Opponents) {
					opp.update ();
				}
				if (this.gm.Waiting && !this.waiting) {
					foreach (Player p in this.gm.Players) {
						if (p.Hand.Count > Constants.MaxHandSize) {
							this.waiting = true;
							this.DiscardCards (p);
							return;
						}
					}
					if (this.gm.State == MatchState.START_GAME) {
						this.gm.RunGame();
					}
					if (this.gm.State == MatchState.START_TURN) {
						Debug.Log ("start turn");
						this.waiting = true;
						if (this.gm.CurrentPlayer.Behaviour is HumanPlayer) {
							StartTurnPrompt ();
						}
						else {
							this.gm.Continue ();
							this.waiting = false;
							this.gm.NextStory ();
						}
					}
					if (this.gm.State == MatchState.REQUEST_SPONSOR) {
						Debug.Log ("requesting sponsor");
						this.waiting = true;
						RequestSponsorPrompt();
					}
					if (this.gm.State == MatchState.RUN_STORY) {
						GameObject.Find("StoryCard").GetComponent<Image>().sprite = Resources.Load<Sprite> ("Cards/" + this.gm.CurrentStory.ImageFilename);
						this.gm.RunStory ();
					}
					if (this.gm.State == MatchState.END_STORY) {
						this.waiting = true;
						QuestGameCardArea qgca = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
						if (qgca != null) {
							this.ClearQuestGameArea (qgca);
							GameObject.Destroy (qgca);
						}
						if (this.GameOtherArea.GetComponent<GameCardArea> () == null) {
							this.GameOtherArea.AddComponent<GameCardArea> ();
						}
						if (this.GameOtherArea.GetComponent<DropArea> () == null) {
							this.GameOtherArea.AddComponent<DropArea> ();
						}
						if (this.GameBattleArea.GetComponent<GameCardArea> () == null) {
							this.GameBattleArea.AddComponent<GameCardArea> ();
						}
						if (this.GameBattleArea.GetComponent<DropArea> () == null) {
							this.GameBattleArea.AddComponent<DropArea> ();
						}
						this.ClearGameArea (this.GameOtherArea.GetComponent<GameCardArea>());
						this.ClearGameArea (this.GameBattleArea.GetComponent<GameCardArea> ());
						if (this.gm.CurrentPlayer.Behaviour is HumanPlayer) {
							this.ConfText.GetComponent<Text> ().text = "End Turn";
						}
						else {
							this.gm.CurrentPlayerNum = (this.gm.CurrentPlayerNum + 1) % this.gm.Players.Count;
							this.gm.PromptingPlayer = this.gm.CurrentPlayerNum;
							this.gm.Continue ();
							this.waiting = false;
							this.gm.NextTurn ();
						}
					}
					if (this.gm.State == MatchState.REQUEST_PARTICIPANTS) {
						this.waiting = true;
						RequestParticipantsPrompt ();
					}
					if (this.gm.State == MatchState.START_TOURNAMENT) {
						Debug.Log ("went into tournament");
						this.waiting = true;
						RequestParticipationTournament ();
					}

					if (this.gm.State == MatchState.REQUEST_STAGE) {
						QuestCard qc = this.gm.CurrentStory as QuestCard;
						this.waiting = true;
						this.gm.PromptingPlayer = qc.SponsorNum;
						GameObject.Find ("OtherAreaText").GetComponent<Text>().text = "Stage " + (qc.Stages.Count + 1) + " Area";
						this.ConfText.GetComponent<Text>().text = "Confirm Stage";
						QuestArea qa = new QuestArea ();
						qc.Stages.Add (qa);
						GameCardArea gca = this.GameOtherArea.GetComponent<GameCardArea> ();
						if (gca != null) {
							this.ClearGameArea (this.GameOtherArea.GetComponent<GameCardArea> ());
							GameObject.Destroy (this.GameOtherArea.GetComponent<GameCardArea> ());
							this.GameOtherArea.AddComponent<QuestGameCardArea> ();
							this.GameOtherArea.GetComponent<QuestGameCardArea> ().QuestCards = qa;
						}
						else {
							QuestGameCardArea qgca = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
							qgca.QuestCards = qa;
							this.ClearQuestGameArea (qgca);
						}
						if (qc.Sponsor.Behaviour is HumanPlayer) {
							ConfirmSponsorPrompt ();
						}
						else {
							List<AdventureCard>[] stages = qc.Sponsor.Behaviour.SetupQuest (qc, qc.Sponsor.Hand);
							for(int i=0; i<qc.StageCount; i++){
								qc.Stages.Add (new QuestArea ());
								qc.Stages [i].Cards = stages [i].ConvertAll(c => (Card)c);
								foreach (Card c in stages[i]) {
									if (c.GetType ().IsSubclassOf (typeof(TestCard)) || c.GetType ().IsSubclassOf (typeof(FoeCard))) {
										qc.Stages [i].MainCard = c;
										break;
									}
								}
							}
							this.gm.Continue ();
							this.waiting = false;
							AIActingPrompt (qc.Sponsor.Username + " has setup " + qc.Name + ".", this.gm.CurrentStory.Run);
						}
					}
					if (this.gm.State == MatchState.RUN_STAGE) {
						this.waiting = true;
						QuestCard qc = this.gm.CurrentStory as QuestCard;
						Player p = qc.Participants [this.gm.PromptingPlayer];
						QuestGameCardArea qgca = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
						GameCardArea gba = this.GameBattleArea.GetComponent<GameCardArea> ();
						this.ClearQuestGameArea (qgca);
						this.ClearGameArea (gba);
						qgca.QuestCards = qc.Stages [qc.CurrentStage-1];
						this.PopulateQuestGameArea (qgca);
						GameObject.Destroy(this.GameOtherArea.GetComponent<DropArea> ());
						GameObject.Find ("OtherAreaText").GetComponent<Text>().text = "Stage " + qc.CurrentStage + " Area";
						gba.Cards = p.BattleArea;

						if (p.Behaviour is HumanPlayer) {
							this.PlayerQuestTurnPrompt ();
						}
						else {
							List<BattleCard> cards = p.Behaviour.PlayCardsInQuest (qc, p.Hand);
							p.BattleArea.Cards = cards.ConvertAll(c => (Card)c);
							this.gm.Continue ();
							this.waiting = false;
							AIActingPrompt (p.Username + " has played its cards.", qc.ResolveStage);
						}
					}

					if (this.gm.State == MatchState.PLAY_TOURNAMENT) {
						TournamentCard tc = this.gm.CurrentStory as TournamentCard;
						this.GameOtherArea.AddComponent<TournamentCardArea> ();
						TournamentCardArea tca = this.GameOtherArea.GetComponent<TournamentCardArea>();
						this.GameOtherArea.GetComponent<TournamentCardArea> ().Cards = tca.Cards;
						this.waiting = true;
						//TournamentCard tc = this.gm.CurrentStory as TournamentCard;
						Player p = tc.Participants [this.gm.PromptingPlayer];
						//TournamentCardArea tca = this.GameOtherArea.GetComponent<TournamentCardArea>();
						GameCardArea gba = this.GameBattleArea.GetComponent<GameCardArea> ();
						this.ClearTournamentGameArea (tca);
						this.ClearGameArea (gba);
						this.PopulateTournamentGameArea (tca);
						GameObject.Destroy (this.GameOtherArea.GetComponent<DropArea> ());
						GameObject.Find ("OtherAreaText").GetComponent<Text>().text = "Tournament Aria";
						gba.Cards = p.BattleArea;

						if (p.Behaviour is HumanPlayer) {
							this.PlayerTournamentTurnPrompt ();
						}
						else {
							List<BattleCard> cards = p.Behaviour.PlayCardsInTournament (tc, p);
							p.BattleArea.Cards = cards.ConvertAll (c => (Card)c);
							this.gm.Continue ();
							this.waiting = false;
							//AIActingPrompt (p.Username + " has played its cards.", );
						}
						//ConfirmSponsorPrompt ();
					}
				}
			}
			else{
				if(SceneManager.GetActiveScene().name == "Match"){
					this.gm.GC = this;
					SetupMatchScene ();
					sceneSet = true;
					this.ConfButton = GameObject.Find ("ConfirmationButton");
					this.ConfText = GameObject.Find ("ConfirmationText");
					this.GameOtherArea = GameObject.Find("OtherArea");
					this.GameBattleArea = GameObject.Find("BattleArea");
					this.GameHandArea = GameObject.Find("HandPanel");
				}
			}
		}

		public void ConfirmationButton(){
			if (this.discardingPlayer != null && this.waiting) {
				if (discardingPlayer.Hand.Count == Constants.MaxHandSize) {
					//Currently Discard setup is transfering to a new card area in UI.
					//Have to transfer back to player then get player to discard
					//to ensure discard gets logged.
					List<Card> discards = new List<Card> ();
					foreach (Card c in this.discardArea.Cards.Cards) {
						discards.Add (c);
					}
					foreach (Card c in discards) {
						this.discardArea.Cards.Transfer (discardingPlayer.Hand, c);
						discardingPlayer.Discard (c);
					}
					this.ClearGameArea (this.discardArea);
					this.discardArea.enabled = false;
					GameObject.Destroy (this.discardArea);
					GameObject.Destroy(this.GameOtherArea.GetComponent<DropArea> ());
					this.EndDiscardPrompt ();
				}
				else{
					this.ConfText.GetComponent<Text> ().text = "Need 12 cards";
				}
			}
			else if (this.gm.State == MatchState.START_TURN && this.waiting) {
				this.gm.Continue ();
				this.waiting = false;
				this.gm.NextStory();
			}
			else if (this.gm.State == MatchState.END_STORY && this.waiting) {
				this.gm.CurrentPlayerNum = (this.gm.CurrentPlayerNum + 1) % this.gm.Players.Count;
				this.gm.PromptingPlayer = this.gm.CurrentPlayerNum;
				this.gm.Continue ();
				this.waiting = false;
				this.gm.NextTurn ();
			}
			else if (this.gm.State == MatchState.REQUEST_STAGE && this.waiting) {
				Debug.Log ("requested stage");
				if (this.GameOtherArea.GetComponent<QuestGameCardArea> ().QuestCards.MainCard != null) {
					this.gm.Continue ();
					this.waiting = false;
					this.gm.CurrentStory.Run ();
				}
			}
			else if (this.gm.State == MatchState.RUN_STAGE && this.waiting) {
				QuestCard qc = this.gm.CurrentStory as QuestCard;
				this.gm.PromptingPlayer = (this.gm.PromptingPlayer + 1) % qc.Participants.Count;
				this.waiting = false;
				if (this.gm.PromptingPlayer == 0) {
					this.gm.Continue ();
					try{
						qc.ResolveStage ();
					}
					catch(NotImplementedException){
						this.logger.Log("Feature not implemented");
					}
					catch (Exception e) {
						this.logger.Log(e.Message);
						this.logger.Log(e.StackTrace);
					}
				}
			}
		}

		public void RunStory(){

		}

		public void LoadScene(string sceneName){
			SceneManager.LoadScene(sceneName);
			sceneSet = false;
		}
		
        public void LoadLocalGameScene(string sceneName) {
			this.scenario = Scenario.LocalGame;
            this.LoadScene(sceneName);
        }

        public void LoadScenario1GameScene(string sceneName) {
            this.scenario = Scenario.Scenario1;
			gm = ScenarioCreator.Scenario1();
			gm.GC = this;
            this.LoadScene(sceneName);
        }

        public void LoadScenario2GameScene(string sceneName) {
            this.scenario = Scenario.Scenario2;
			gm = ScenarioCreator.Scenario2();
			gm.GC = this;
            this.LoadScene(sceneName);
        }

        private void SetupMatchScene (){
			GameObject opponents = GameObject.Find ("Opponents");
			for (int i = 0; i < this.gm.Players.Count; i++) {
				GameObject opponent = Instantiate (Resources.Load("Opponent", typeof(GameObject))) as GameObject;
				this.Opponents.Add (new OpponentState (this, opponent, i));
				opponent.transform.SetParent (opponents.transform);
				opponent.transform.localScale = new Vector3 (1, 1, 1);
			}
			this.gm.Setup (this.scenario == Scenario.LocalGame);
		}

		public void ShowHand(Player p){
			HideHand ();
			this.GameHandArea.GetComponent<GameCardArea> ().Cards = p.Hand;
			for (int i = 0; i < p.Hand.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = p.Hand.Cards[i];
				card.transform.SetParent (this.GameHandArea.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + p.Hand.Cards [i].ImageFilename);
			}
		}
		public void ShowHand(CardArea hand){
			HideHand ();
			this.GameHandArea.GetComponent<GameCardArea> ().Cards = hand;
			for (int i = 0; i < hand.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = hand.Cards[i];
				card.transform.SetParent (this.GameHandArea.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + hand.Cards [i].ImageFilename);
			}
		}
		public void HideHand(){
			foreach (Transform child in this.GameHandArea.transform) {
				GameObject.Destroy (child.gameObject);
			}
		}
		public void ShowBattleArea(Player p){
			HideBattleArea ();
			if (p.BattleArea == null) {
				return;
			}
			this.GameBattleArea.GetComponent<GameCardArea> ().Cards = p.BattleArea;
			for (int i = 0; i < p.BattleArea.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = p.BattleArea.Cards[i];
				card.transform.SetParent (this.GameBattleArea.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + p.BattleArea.Cards [i].ImageFilename);
			}
		}
		public void ShowBattleArea(CardArea area){
			HideBattleArea ();
			if (area == null) {
				return;
			}
			this.GameBattleArea.GetComponent<GameCardArea> ().Cards = area;
			for (int i = 0; i < area.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = area.Cards[i];
				card.transform.SetParent (this.GameBattleArea.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + area.Cards [i].ImageFilename);
			}
		}
		public void HideBattleArea(){
			foreach (Transform child in this.GameBattleArea.transform) {
				GameObject.Destroy (child.gameObject);
			}
		}

		public void PopulateGameArea(GameCardArea gca){
			if (gca.Cards == null) {
				return;
			}
			for (int i = 0; i < gca.Cards.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = gca.Cards.Cards[i];
				card.transform.SetParent (gca.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + gca.Cards.Cards[i].ImageFilename);
			}
		}
		public void PopulateQuestGameArea(QuestGameCardArea qgca){
			if (qgca.QuestCards == null) {
				return;
			}
			for (int i = 0; i < qgca.QuestCards.Cards.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = qgca.QuestCards.Cards[i];
				card.transform.SetParent (qgca.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + qgca.QuestCards.Cards[i].ImageFilename);
			}
		}

		public void PopulateTournamentGameArea(TournamentCardArea tca){
			if (tca.Cards == null) {
				return;
			}
			for (int i = 0; i < tca.Cards.Cards.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = tca.Cards.Cards[i];
				card.transform.SetParent (tca.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + tca.Cards.Cards[i].ImageFilename);
			}
		}

		public void ClearGameArea(GameCardArea gca){
			foreach (Transform child in gca.transform) {
				GameObject.Destroy (child.gameObject);
			}
		}

		public void ClearQuestGameArea(QuestGameCardArea qgca){
			foreach (Transform child in qgca.transform) {
				GameObject.Destroy (child.gameObject);
			}
		}

		public void ClearTournamentGameArea(TournamentCardArea tca){
			Debug.Log (tca);
			foreach (Transform child in tca.transform){
				GameObject.Destroy (child.gameObject);
			}
		}

		public void StartGame(){
			gm.RunGame ();
		}
		public void AIActingPrompt(string action, UnityAction onclick){
			this.HideHand ();
			this.HideBattleArea ();
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = action;
			prompt.OnYesClick = onclick;
			prompt.OnNoClick = onclick;
		}
		public void StartTurnPrompt(){
			this.HideHand ();
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = this.gm.CurrentPlayer.Username +" ready?";
			prompt.OnYesClick = this.StartTurnYes;
			prompt.OnNoClick = this.StartTurnYes;
			GameObject storyCardArea = GameObject.Find ("StoryCard");
			storyCardArea.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + "story_card_back");
		}
		public void StartTurnYes(){
			Debug.Log("Player Ready clicked");
			this.ShowHand (this.gm.CurrentPlayer);
			this.ConfText.GetComponent<Text>().text = "Draw Story";
		}
		public void EndDiscardPrompt(){
			this.HideHand();
			this.HideBattleArea ();
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Return to current player?";
			prompt.OnYesClick = this.EndDiscardYes;
			prompt.OnNoClick = this.EndDiscardYes;
		}
		public void EndDiscardYes(){
			Debug.Log("Player Discard Ready clicked");
			if (gameCardAreaSave != null) {
				gameCardAreaSave.enabled = true;
				this.PopulateGameArea (gameCardAreaSave);
			}
			if (questGameCardAreaSave != null) {
				questGameCardAreaSave.enabled = true;
				this.PopulateQuestGameArea (questGameCardAreaSave);
			}
			this.discardingPlayer = null;
			this.gameCardAreaSave = null;
			this.questGameCardAreaSave = null;
			this.waiting = false;
			this.ShowHand (this.handAreaSave);
			this.ShowBattleArea (this.battleAreaSave);
			this.handAreaSave = null;
			this.battleAreaSave = null;
		}
		public void ConfirmSponsorPrompt(){
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Sponsor ready?";
			prompt.OnYesClick = this.ConfirmSponsorYes;
			prompt.OnNoClick = this.ConfirmSponsorYes;
		}
		public void ConfirmSponsorYes(){
			Debug.Log("Sponsor Ready clicked");
			this.ShowHand ((this.gm.CurrentStory as QuestCard).Sponsor);
		}
		public void RequestSponsorPrompt(){
			Debug.Log ("requesting sponsor");
			bool canSponsor = false;
			Player p = this.gm.Players [this.gm.PromptingPlayer];
			int sponsori = 0;
			foreach(Card ccard in p.Hand.Cards){
				if (ccard.GetType ().IsSubclassOf (typeof(TestCard)) || ccard.GetType ().IsSubclassOf (typeof(FoeCard))) {
					sponsori += 1;
				}
			}
			canSponsor = sponsori >= (this.gm.CurrentStory as QuestCard).StageCount;
			if (canSponsor) {
				if (p.Behaviour is HumanPlayer) {
					this.HideHand ();
					GameObject promptObj = new GameObject ("SponsorQuestPrompt");
					SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt> ();
					prompt.Quest = this.gm.CurrentStory;
					prompt.Message = "Would " + p.Username + " like to sponsor " + this.gm.CurrentStory.Name + "?";
					prompt.OnYesClick = this.SponsorYes;
					prompt.OnNoClick = this.SponsorNo;
				}
				else {
					bool willSponsor = p.Behaviour.SponsorQuest (this.gm.CurrentStory as QuestCard, p.Hand);
					if (willSponsor) {
						this.AIActingPrompt (p.Username + " will sponsor " + this.gm.CurrentStory.Name + ".", this.SponsorYes);
					}
					else {
						this.AIActingPrompt (p.Username + " will not sponsor " + this.gm.CurrentStory.Name + ".", this.SponsorNo);
					}
				}
			}
			else {
				this.SponsorNo();
			}
		}

		public void SponsorYes(){
			this.gm.Log("Sponsor Yes Clicked");
			QuestCard qc = this.gm.CurrentStory as QuestCard;
			qc.Sponsor = this.gm.Players[this.gm.PromptingPlayer];
			this.gm.Continue ();
			this.waiting = false;
			qc.requestParticipation ();
		}

		public void SponsorNo(){
			this.gm.Log("Sponsor No Clicked");
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == this.gm.CurrentPlayerNum){
				this.gm.Continue();
				this.waiting = false;
				this.gm.EndStory();
			}
			else{
				this.waiting = false;
			}
		}
			
		public void RequestParticipationTournament(){
			GameObject promptObj = new GameObject ("ParticipateInTournament");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Would " + this.gm.Players[this.gm.PromptingPlayer].Username +" like to participate in "+ this.gm.CurrentStory.Name +"?";
			prompt.OnYesClick = this.ParticipateTournament;
			prompt.OnNoClick = this.NoParticipateTournament;
		}

		public void ParticipateTournament(){
			TournamentCard tc = this.gm.CurrentStory as TournamentCard;
			this.gm.Log ("Participate Yes Clicked");
			tc.Participants.Add(this.gm.Players[this.gm.PromptingPlayer]);
			Debug.Log (tc.Participants.Count);
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer + 1) % this.gm.Players.Count;
			if (this.gm.PromptingPlayer == tc.FirstPlayerNum) {
				tc.AllAsked = 1;
				this.gm.Continue ();
				this.waiting = false;
				this.gm.CurrentStory.Run ();
			}
			else {
				this.waiting = false;
			}
		}
		public void NoParticipateTournament(){
			TournamentCard tc = this.gm.CurrentStory as TournamentCard;
			this.gm.Log("Participate No Clicked");
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == tc.FirstPlayerNum){
				this.gm.Continue();
				this.waiting = false;
				this.gm.EndStory ();
			}
			else{
				this.waiting = false;
			}
		}

		public void RequestParticipantsPrompt(){
			Player p = this.gm.Players [this.gm.PromptingPlayer];
			if (p.Behaviour is HumanPlayer) {
				GameObject promptObj = new GameObject ("SponsorQuestPrompt");
				SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt> ();
				prompt.Quest = this.gm.CurrentStory;
				prompt.Message = "Would " + p.Username + " like to participate in " + this.gm.CurrentStory.Name + "?";
				prompt.OnYesClick = this.ParticipateYes;
				prompt.OnNoClick = this.ParticipateNo;
			}
			else {
				bool willParticipate = p.Behaviour.ParticipateInQuest (this.gm.CurrentStory as QuestCard, p.Hand);
				if (willParticipate) {
					this.AIActingPrompt (p.Username + " will participate.", this.ParticipateYes);
				}
				else {
					this.AIActingPrompt (p.Username + " will not participate.", this.ParticipateNo);
				}
			}
		}
			

		public void ParticipateYes(){
			QuestCard qc = this.gm.CurrentStory as QuestCard;
			this.gm.Log("Participate Yes Clicked");
			qc.AddParticipant (this.gm.Players [this.gm.PromptingPlayer]);
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == qc.SponsorNum) {
				this.gm.Continue ();
				this.waiting = false;
				this.gm.CurrentStory.Run ();
			}
			else {
				this.waiting = false;
			}
		}

		public void ParticipateNo(){
			QuestCard qc = this.gm.CurrentStory as QuestCard;
			this.gm.Log("Participate No Clicked");
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == qc.SponsorNum){
				this.gm.Continue();
				this.waiting = false;
				this.gm.CurrentStory.Run ();
			}
			else{
				this.waiting = false;
			}
		}

		public void PlayerQuestTurnPrompt(){
			this.HideHand ();
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Quest = this.gm.CurrentStory;
			prompt.Message = (this.gm.CurrentStory as QuestCard).Participants[this.gm.PromptingPlayer].Username +" ready?";
			prompt.OnYesClick = this.PlayerQuestTurnReady;
			prompt.OnNoClick = this.PlayerQuestTurnReady;
		}

		public void PlayerTournamentTurnPrompt(){
			this.HideHand ();
			GameObject promptObj = new GameObject("TournamentPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = (this.gm.CurrentStory as TournamentCard).Participants[this.gm.PromptingPlayer].Username +" ready?";
			prompt.OnYesClick = this.PlayerTournamentTurnReady;
			prompt.OnNoClick = this.PlayerTournamentTurnReady;
		}

		public void PlayerTournamentTurnReady(){
			this.ShowHand ((this.gm.CurrentStory as TournamentCard).Participants[this.gm.PromptingPlayer]);
			this.PopulateGameArea (this.GameBattleArea.GetComponent<GameCardArea>());
			this.ConfText.GetComponent<Text>().text = "Confirm Cards";
		}

		public void PlayerQuestTurnReady(){
			this.ShowHand ((this.gm.CurrentStory as QuestCard).Participants[this.gm.PromptingPlayer]);
			this.PopulateGameArea (this.GameBattleArea.GetComponent<GameCardArea>());
			this.ConfText.GetComponent<Text>().text = "Confirm Cards";
		}

		public void DiscardCards(Player p){
			if (p.Behaviour is HumanPlayer) {
				this.discardingPlayer = p;
				this.gameCardAreaSave = this.GameOtherArea.GetComponent<GameCardArea> ();
				this.questGameCardAreaSave = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
				this.handAreaSave = this.GameHandArea.GetComponent<GameCardArea> ().Cards;
				this.battleAreaSave = this.GameBattleArea.GetComponent<GameCardArea> ().Cards;
				if (gameCardAreaSave != null) {
					this.ClearGameArea (gameCardAreaSave);
					gameCardAreaSave.enabled = false;
				}
				if (questGameCardAreaSave != null) {
					this.ClearQuestGameArea (questGameCardAreaSave);
					questGameCardAreaSave.enabled = false;
				}
				this.GameOtherArea.AddComponent<DropArea> ();
				this.GameOtherArea.AddComponent<GameCardArea> ();
				this.discardArea = this.GameOtherArea.GetComponent<GameCardArea> ();
				this.discardArea.Cards = new BattleArea ();
				DiscardCardsPrompt (p);
			}
			else {
				List<Card> discards = p.Behaviour.DiscardExcessCards (p.Hand);
				p.Discard (discards);
				this.AIActingPrompt (p.Username + " discarded excess cards.", () => {
					this.waiting = false;
				});
			}
		}
		public void DiscardCardsPrompt(Player p){
			this.HideHand ();
			this.HideBattleArea ();
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Too many cards: " + p.Username +"\nPlay or Discard excess";
			prompt.OnYesClick = () => { Debug.Log("Player Discard Ready clicked");
				this.ShowHand (p);
				this.ShowBattleArea(p);
				GameObject.Find("OtherAreaText").GetComponent<Text>().text = "Discard Area";
				this.ConfText.GetComponent<Text>().text = "Confirm";};
			prompt.OnNoClick = () => { Debug.Log("Player Discard Ready clicked");
				this.ShowHand (p);
				this.ShowBattleArea(p);
				GameObject.Find("OtherAreaText").GetComponent<Text>().text = "Discard Area";
				this.ConfText.GetComponent<Text>().text = "Confirm";};
		}

		public void QuitGame(){
			//When game is run in editor, the application cannot be quit as this would close editor
			//Therefore, have to specifically stop it through editor
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		void SetQuestSponsor(Player player){
			QuestCard quest = this.gm.CurrentStory as QuestCard;
			quest.Sponsor = player;
		}

		void SetQuestPlayers(List<Player> qPlayers){
			QuestCard quest = this.gm.CurrentStory as QuestCard;
			quest.Participants = qPlayers;
		}

		public void AddHumanPlayers(int num){
			for (int i = 0; i < num; i++) {
				Player p = new Player (this.numPlayers.ToString () + "Human", this.gm);
				p.Behaviour = new HumanPlayer ();
				this.gm.AddPlayer (p);
				GameObject list = GameObject.Find ("List_of_players");
				if (list != null) {
					Text t = list.GetComponent<Text> ();
					t.text = t.text + "\n" + this.numPlayers.ToString() + "Human";
				}
				this.numPlayers += 1;
			}
		}

		public void AddAIPlayers(int num){
			bool canAdd = false;
			foreach (Player p in this.gm.Players) {
				if (p.Behaviour is HumanPlayer) {
					canAdd = true;
				}
			}
			if (canAdd) {
				for (int i = 0; i < num; i++) {
					Player p = new Player (this.numPlayers.ToString () + "AI", this.gm);
					this.gm.AddPlayer (p);
					GameObject list = GameObject.Find ("List_of_players");
					if (list != null) {
						Text t = list.GetComponent<Text> ();
						t.text = t.text + "\n" + this.numPlayers.ToString () + "AI";
					}
					this.numPlayers += 1;
					AIStrategyPrompt (p);
				}
			}
			else {
				GameObject promptObj = new GameObject("PlayerPrompt");
				Prompt prompt = promptObj.AddComponent<Prompt>();
				prompt.Message = "Must have a human player first";
				prompt.YesButton.GetComponentInChildren<Text> ().text = "OK";
				prompt.NoButton.GetComponentInChildren<Text> ().text = "OK";
			}
		}

		public void AIStrategyPrompt(Player p){
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Which strategy?";
			prompt.YesButton.GetComponentInChildren<Text> ().text = "Strategy 1";
			prompt.NoButton.GetComponentInChildren<Text> ().text = "Strategy 2";
			prompt.OnYesClick = () => {p.Behaviour = new Strategy1();};
			prompt.OnNoClick = () => {p.Behaviour = new Strategy2();};
		}
			
	}
	public class OpponentState{
		GameController gc;
		GameObject opponent;
		int playerNum;
		Image rankImage;
		Text playerName;
		Text shieldText;
		Text shieldValue;
		Text cardsText;
		Text cardsValue;
		Text inPlayText;
		Text inPlayValue;
		public OpponentState(GameController gc, GameObject opponent, int playerNum){
			this.gc = gc;
			this.opponent = opponent;
			this.playerNum = playerNum;
			List<GameObject> opponentGOs = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Opponent"));
			foreach (GameObject go in opponentGOs) {
				if (go.transform.IsChildOf (opponent.transform)) {
					if (go.name == "OpponentRankImage") {
						this.rankImage = go.GetComponent<Image>();
					}
					if (go.name == "OpponentPlayerName") {
						this.playerName = go.GetComponent<Text>();
					}
					if (go.name == "OpponentShieldText") {
						this.shieldText = go.GetComponent<Text>();
					}
					if (go.name == "OpponentShieldValue") {
						this.shieldValue = go.GetComponent<Text>();
					}
					if (go.name == "OpponentCardsText") {
						this.cardsText = go.GetComponent<Text>();
					}
					if (go.name == "OpponentCardsValue") {
						this.cardsValue = go.GetComponent<Text>();
					}
					if (go.name == "OpponentInPlayText") {
						this.inPlayText = go.GetComponent<Text>();
					}
					if (go.name == "OpponentInPlayValue") {
						this.inPlayValue = go.GetComponent<Text>();
					}
				}
			}
		}

		public void update(){
			this.rankImage.sprite = Resources.Load<Sprite> ("Cards/" + this.gc.GM.Players [this.playerNum].RankCard.ImageFilename);
			this.playerName.text = this.gc.GM.Players [this.playerNum].Username;
			this.shieldValue.text = this.gc.GM.Players [this.playerNum].Rank.Shields.ToString();
			this.cardsValue.text = this.gc.GM.Players [this.playerNum].Hand.Cards.Count.ToString();
			this.inPlayValue.text = this.gc.GM.Players [this.playerNum].BattleArea.Cards.Count.ToString();
		}
	}
}
