//"connected" to the GameController game object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quest.Core;
using Quest.Core.Players;
using Quest.Core.Cards;
using Utils;
using System.Collections.Generic;

namespace Quest.Core {
	public class GameController : MonoBehaviour {
		QuestMatch gm;
		Logger logger;
		bool waiting;
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
			this.numPlayers = gc.numPlayers;
			this.Opponents = gc.Opponents;
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
				logger = new Logger ();
				gm = new QuestMatch(logger);
				sceneSet = false;
				waiting = false;
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
					if (this.gm.State == MatchState.START_GAME) {
						this.gm.RunGame();
					}
					if (this.gm.State == MatchState.START_TURN) {
						Debug.Log ("start turn");
						this.waiting = true;
						StartTurnPrompt ();
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
						this.ConfText.GetComponent<Text>().text = "End Turn";
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
						int i = 0;
						for (; i < this.gm.Players.Count; i++) {
							if (this.gm.Players [i] == qc.Sponsor) {
								break;
							}
						}
						this.gm.PromptingPlayer = i;
						GameObject.Find ("OtherAreaText").GetComponent<Text>().text = "Stage " + (qc.Stages.Count + 1) + " Area";
						this.ConfText.GetComponent<Text>().text = "Confirm Stage";
						QuestArea qa = new QuestArea ();
						qc.Stages.Add (qa);
						GameCardArea gca = this.GameOtherArea.GetComponent<GameCardArea> ();
						if (gca != null) {
							this.ClearGameArea (this.GameOtherArea.GetComponent<GameCardArea> ());
							GameObject.Destroy (this.GameOtherArea.GetComponent<GameCardArea> ());
							this.GameOtherArea.AddComponent<QuestGameCardArea> ().QuestCards = qa;
						}
						else {
							QuestGameCardArea qgca = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
							qgca.QuestCards = qa;
							this.ClearQuestGameArea (qgca);
						}
						ConfirmSponsorPrompt ();
					}
					if (this.gm.State == MatchState.RUN_STAGE) {
						this.waiting = true;
						QuestCard qc = this.gm.CurrentStory as QuestCard;
						QuestGameCardArea qgca = this.GameOtherArea.GetComponent<QuestGameCardArea> ();
						this.ClearQuestGameArea (qgca);
						qgca.QuestCards = qc.Stages [qc.CurrentStage-1];
						this.PopulateQuestGameArea (qgca);
						GameObject.Destroy(this.GameOtherArea.GetComponent<DropArea> ());
						this.PlayerQuestTurnPrompt ();
					}
				}
			}
			else{
				if(SceneManager.GetActiveScene().name == "Match"){
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
			if (this.gm.State == MatchState.START_TURN && this.waiting) {
				this.gm.Continue ();
				this.waiting = false;
				this.gm.NextStory();
			}
			if (this.gm.State == MatchState.END_STORY && this.waiting) {
				this.gm.CurrentPlayerNum = (this.gm.CurrentPlayerNum + 1) % this.gm.Players.Count;
				this.gm.PromptingPlayer = this.gm.CurrentPlayerNum;
				this.gm.Continue ();
				this.waiting = false;
				this.gm.NextTurn ();
			}
			if (this.gm.State == MatchState.REQUEST_STAGE && this.waiting) {
				Debug.Log ("requested stage");
				if (this.GameOtherArea.GetComponent<QuestGameCardArea> ().QuestCards.MainCard != null) {
					this.gm.Continue ();
					this.waiting = false;
					this.gm.CurrentStory.Run ();
				}
			}
			if (this.gm.State == MatchState.RUN_STAGE && this.waiting) {
				QuestCard qc = this.gm.CurrentStory as QuestCard;
				this.gm.Continue ();
				this.gm.PromptingPlayer = (this.gm.PromptingPlayer + 1) % this.gm.Players.Count;
				this.waiting = false;
				if (this.gm.Players [this.gm.PromptingPlayer] == qc.Sponsor) {
					qc.ResolveStage ();
				}
			}
		}

		public void RunStory(){

		}

		public void LoadScene(string sceneName){
			SceneManager.LoadScene(sceneName);
			sceneSet = false;

		}
		private void SetupMatchScene (){
			GameObject opponents = GameObject.Find ("Opponents");
			for (int i = 0; i < this.gm.Players.Count; i++) {
				GameObject opponent = Instantiate (Resources.Load("Opponent", typeof(GameObject))) as GameObject;
				this.Opponents.Add (new OpponentState (this, opponent, i));
				opponent.transform.SetParent (opponents.transform);
				opponent.transform.localScale = new Vector3 (1, 1, 1);
			}
			this.gm.Setup ();
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

		public void PopulateGameArea(GameCardArea gca){
			for (int i = 0; i < gca.Cards.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = gca.Cards.Cards[i];
				card.transform.SetParent (gca.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + gca.Cards.Cards[i].ImageFilename);
			}
		}
		public void PopulateQuestGameArea(QuestGameCardArea qgca){
			for (int i = 0; i < qgca.QuestCards.Cards.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.GetComponent<GameCard>().Card = qgca.QuestCards.Cards[i];
				card.transform.SetParent (qgca.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + qgca.QuestCards.Cards[i].ImageFilename);
			}
		}
		public void HideHand(){
			foreach (Transform child in this.GameHandArea.transform) {
				GameObject.Destroy (child.gameObject);
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

		public void StartGame(){
			gm.RunGame ();
		}

		public void StartTurnPrompt(){
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = this.gm.CurrentPlayer.Username +" ready?";
			prompt.OnYesClick = () => { Debug.Log("Player Ready clicked");
				this.ShowHand (this.gm.CurrentPlayer);
				this.ConfText.GetComponent<Text>().text = "Draw Story";};
			GameObject storyCardArea = GameObject.Find ("StoryCard");
			storyCardArea.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + "story_card_back");
		}
		public void ConfirmSponsorPrompt(){
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "Sponsor ready?";
			prompt.OnYesClick = () => { Debug.Log("Sponsor Ready clicked");
				this.ShowHand ((this.gm.CurrentStory as QuestCard).Sponsor);};
		}
		public void RequestSponsorPrompt(){
			Debug.Log ("requesting sponsor");
			bool canSponsor = false;
			int sponsori = 0;
			foreach(Card ccard in this.gm.Players[this.gm.PromptingPlayer].Hand.Cards){
				if (ccard.GetType ().IsSubclassOf (typeof(TestCard)) || ccard.GetType ().IsSubclassOf (typeof(FoeCard))) {
					sponsori += 1;
				}
			}
			canSponsor = (sponsori + 1) >= (this.gm.CurrentStory as QuestCard).StageCount;
			if (canSponsor) {
				this.HideHand ();
				GameObject promptObj = new GameObject ("SponsorQuestPrompt");
				SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt> ();
				prompt.Quest = this.gm.CurrentStory;
				prompt.Message = "Would " + this.gm.Players [this.gm.PromptingPlayer].Username + " like to sponsor " + this.gm.CurrentStory.Name + "?";
				prompt.OnYesClick = this.SponsorYes;
				prompt.OnNoClick = this.SponsorNo;
			}
			else {
				this.SponsorNo();
			}
		}

		public void SponsorYes(){
			Debug.Log ("sponser yes func");
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
			Debug.Log ("participate in touny");
			TournamentCard tc = this.gm.CurrentStory as TournamentCard;
			int i = 0;
			for (; i < this.gm.Players.Count; i++) {
				if (this.gm.Players [i] == tc.FirstPlayer) {
					break;
				}
			}
			this.gm.Log ("Participate Yes Clicked");
			Debug.Log (tc);
			tc.Participants.Add(this.gm.Players[this.gm.PromptingPlayer]);
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer + 1) % this.gm.Players.Count;
			if (this.gm.PromptingPlayer == i) {
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
			int i = 0;
			for (; i < this.gm.Players.Count; i++) {
				if (this.gm.Players [i] == tc.FirstPlayer) {
					break;
				}
			}
			this.gm.Log("Participate No Clicked");
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == i){
				this.gm.Continue();
				this.waiting = false;
				this.gm.CurrentStory.Run ();
			}
			else{
				this.waiting = false;
			}
		}

		public void RequestParticipantsPrompt(){
			Debug.Log ("wut");
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Quest = this.gm.CurrentStory;
			prompt.Message = "Would " + this.gm.Players[this.gm.PromptingPlayer].Username +" like to participate in "+ this.gm.CurrentStory.Name +"?";
			prompt.OnYesClick = this.ParticipateYes;
			prompt.OnNoClick = this.ParticipateNo;
		}
			

		public void ParticipateYes(){
			QuestCard qc = this.gm.CurrentStory as QuestCard;
			int i = 0;
			for (; i < this.gm.Players.Count; i++) {
				if (this.gm.Players [i] == qc.Sponsor) {
					break;
				}
			}
			this.gm.Log("Participate Yes Clicked");
			qc.Participants.Add (this.gm.Players [this.gm.PromptingPlayer]);
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == i) {
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
			int i = 0;
			for (; i < this.gm.Players.Count; i++) {
				if (this.gm.Players [i] == qc.Sponsor) {
					break;
				}
			}
			this.gm.Log("Participate No Clicked");
			this.gm.PromptingPlayer = (this.gm.PromptingPlayer+1)%this.gm.Players.Count;
			if (this.gm.PromptingPlayer == i){
				this.gm.Continue();
				this.waiting = false;
				this.gm.CurrentStory.Run ();
			}
			else{
				this.waiting = false;
			}
		}

		public void PlayerQuestTurnPrompt(){
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Quest = this.gm.CurrentStory;
			prompt.Message = this.gm.Players[this.gm.PromptingPlayer].Username +" ready?";
			prompt.OnYesClick = this.PlayerQuestTurnReady;
			prompt.OnNoClick = this.PlayerQuestTurnReady;
		}

		public void PlayerQuestTurnReady(){
			this.ShowHand (this.gm.Players[this.gm.PromptingPlayer]);
			this.ConfText.GetComponent<Text>().text = "Confirm Cards";
		}
		public void DiscardCardsPrompt(){
			/*
			GameObject promptObj = new GameObject("PlayerPrompt");
			Prompt prompt = promptObj.AddComponent<Prompt>();
			prompt.Message = "You have too many cards.";
			prompt.OnYesClick = () => { Debug.Log("Player Ready clicked");
				this.ShowHand ();};
			GameObject storyCardArea = GameObject.Find ("StoryCard");
			*/
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
				this.gm.AddPlayer (new Player (this.numPlayers.ToString() +"Human", this.gm));
				GameObject list = GameObject.Find ("List_of_players");
				if (list != null) {
					Text t = list.GetComponent<Text> ();
					t.text = t.text + "\n" + this.numPlayers.ToString() + "Human";
				}
				this.numPlayers += 1;
			}
		}

		public void AddAIPlayers(int num){
			for (int i = 0; i < num; i++) {
				this.gm.AddPlayer (new Player (this.numPlayers.ToString() +"AI", this.gm));
				GameObject list = GameObject.Find ("List_of_players");
				if (list != null) {
					Text t = list.GetComponent<Text> ();
					t.text = t.text + "\n" + this.numPlayers.ToString() + "AI";
				}
				this.numPlayers += 1;
			}
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
