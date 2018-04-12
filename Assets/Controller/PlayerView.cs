using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quest.Core.View{
	static class Constants
	{
	    public const string DEFAULT_SERVER_ADDRESS = "ws://localhost:3004/quest";
			public const string DEFAULT_USERNAME = "Default";
			public const string RESOURCES_CARDS = "Cards/";
	}
	public class PlayerView : MonoBehaviour {
			private Dictionary<string, Action<JToken>> eventHandlers;
			private Dictionary<string, GameObject> disabledObjects;
			private Queue<Action> updateQueue;
			private List<string> games;
			private List<Player> players;
			private List<Card> otherAreaCards;
			private List<Card> handAreaCards;
			private List<Card> playerAreaCards;

	    private UnityWebSocket socket;
			private Card currentStory;
			private string sceneName;
			private string prepScene;
			private string userName;
			private string serverAddress;
			private string promptName;
			private string promptMessage;
			//TODO: receiving image is redundant for all current prompts.
			//private string promptImage;
			private string newHistory;
			private string otherAreaName;
			private string confirmationMessage;
			private bool isHost;
			private bool isConnected;
			private bool prompting;
			private int gameid;

			private GameObject gameCanvas;
			private GameObject battleArea;
			private GameObject otherArea;
			private GameObject handArea;
			private GameObject historyScroll;
			private Button historyButton;
			private Button confirmationButton;
			private Image currentStoryCard;
			private Text otherAreaText;
			private Text confirmationButtonText;
			private Text historyScrollText;
			private Text historyButtonText;

			public int Gameid{
				get {return this.gameid;}
				set {this.gameid = value;}
			}
			public GameObject GameCanvas {
				get {return this.gameCanvas;}
				set {this.gameCanvas = value;}
			}

		/*
			Initial PlayerView setup.
		*/
		private void Awake(){
			//Variable and collections
			this.disabledObjects = new Dictionary<string, GameObject>();
			this.serverAddress = Constants.DEFAULT_SERVER_ADDRESS;
			this.userName = Constants.DEFAULT_USERNAME;
			this.isHost = false;
			this.isConnected = false;
			this.gameid = -1;
			this.games = new List<string>();
			this.players = new List<Player>();
			this.promptName = "";
			this.prompting = false;
			this.updateQueue = new Queue<Action>();
			this.newHistory = "";
			this.confirmationMessage = "";
			this.eventHandlers = new Dictionary<string, Action<JToken>>();

			//Socket events
			On("update_games", OnRCVUpdateGames);
			On("update_players", OnRCVUpdatePlayers);
			On("update_story", OnRCVUpdateStory);
			On("update_player_area", OnRCVUpdatePlayerArea);
			On("update_other_area", OnRCVUpdateOtherArea);
			On("update_hand", OnRCVUpdateHand);
			On("request_quest_sponsor", OnRCVRequestQuestSponsor);
			On("request_quest_participation", OnRCVRequestQuestParticipation);
			On("message", OnRCVMessage);
			//On("request_discard", OnRCVRequestDiscard);
			On("request_stage", OnRCVRequestStage);
			On("request_play_cards", OnRCVRequestPlayCards);
			On("end_story", OnRCVEndStory);
			On("request_tournament_participation", OnRCVRequestTournamentParticipation);

			//Unity events
			SceneManager.activeSceneChanged += OnUISceneChanged;

			DontDestroyOnLoad (this);
		}
		/*
			More PlayerView setup if dependent on other objects.
		*/
    private void Start() {
    }

		/*
			Runs UpdateOnline function if connected to server,
			UpdateOffline otherwise.
		*/
		public void Update(){
				if(this.prepScene!=this.sceneName){
						LoadScene(this.prepScene);
				}
				else{
						if (this.gameCanvas == null){
								GameObject gc = GameObject.Find ("GameCanvas");
								if (gc != null) {
										this.gameCanvas = gc;
								}
						}
						if(!prompting && promptName!=""){
							UpdatePrompt();
						}
						if (this.isConnected){
								UpdateOnline();
						}
						else{
								UpdateOffline();
						}
				}
		}

		/*
			UpdateOnline calls specific scene UpdateOnline function
		*/
		private void UpdateOnline(){
			switch(this.sceneName){
				case "MainMenu":
					UpdateOnlineMainMenu();
					break;
				case "Lobby":
					UpdateOnlineLobby();
					break;
				case "Match":
					UpdateOnlineMatch();
					break;
			}
		}
		private void UpdateOnlineMainMenu(){
			if(!this.disabledObjects.ContainsKey("Button_Connect")){
				EnableObject("Canvas_NetworkGames");
				EnableObject("Button_HostGame");
				EnableObject("Button_JoinGame");
				EnableObject("button_Scenario1");
				EnableObject("button_Scenario2");
				EnableObject("button_RunSimulation");
				DisableObject("Canvas_Username");
				DisableObject("Canvas_Server");
				DisableObject("Button_Connect");
			}
			Dropdown dd = GameObject.Find("Dropdown_Games").GetComponent<Dropdown>();
			dd.ClearOptions();
			dd.AddOptions(this.games);
			if(dd.options.Count == 0){
				this.gameid = -1;
			}
			else{
				if (!int.TryParse(dd.options[0].text, out this.gameid)){
					this.gameid = -1;
				}
			}
		}
		private void UpdateOnlineLobby(){
			GameObject list = GameObject.Find ("List_of_players");
			Text t = list.GetComponent<Text> ();
			t.text = "";
			foreach(Player p in this.players){
				t.text = t.text + p.username + "\n";
			}
		}
		private void UpdateOnlineMatch(){
			while(this.updateQueue.Count>0){
				Action next = this.updateQueue.Dequeue();
				next();
			}
		}

		/*
			UpdateOffline calls specific scene UpdateOffline function.
			Is generally unused except to update main menu at start.
			TODO: Could run offline game here
		*/
		private void UpdateOffline(){
			switch(this.sceneName){
				case "MainMenu":
					UpdateOfflineMainMenu();
					break;
			}
		}
		private void UpdateOfflineMainMenu(){
			if(this.disabledObjects.ContainsKey("Button_Connect")){
				DisableObject("Canvas_NetworkGames");
				DisableObject("Button_HostNetwork");
				DisableObject("Button_JoinNetwork");
				DisableObject("button_Scenario1");
				DisableObject("button_Scenario2");
				DisableObject("button_RunSimulation");
				EnableObject("Canvas_Username");
				EnableObject("Canvas_Server");
				EnableObject("Button_Connect");
			}
		}

		/*
			Prompt specific updates.
		*/
		private void UpdatePrompt(){
			switch(promptName){
				case "SponsorQuestPrompt":
					UpdateSponsorQuestPrompt();
					break;
				case "RequestQuestParticipationPrompt":
					UpdateRequestQuestParticipationPrompt();
					break;
				case "RequestTournamentParticipationPrompt":
					UpdateRequestTournamentParticipationPrompt();
					break;
			}
			this.prompting = true;
		}
		private void UpdateSponsorQuestPrompt(){
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Message = this.promptMessage;
			prompt.Quest = this.currentStory;

			prompt.OnNoClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["sponsoring"] = false;
				EventData evn = new EventData("quest_sponsor_response", data);
				SendSocketMessage(evn.ToString());
			};
			prompt.OnYesClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["sponsoring"] = true;
				EventData evn = new EventData("quest_sponsor_response", data);
				SendSocketMessage(evn.ToString());
			};
		}
		private void UpdateRequestQuestParticipationPrompt(){
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Message = this.promptMessage;
			prompt.Quest = this.currentStory;

			prompt.OnNoClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["participating"] = false;
				EventData evn = new EventData("participation_response", data);
				SendSocketMessage(evn.ToString());
			};
			prompt.OnYesClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["participating"] = true;
				EventData evn = new EventData("participation_response", data);
				SendSocketMessage(evn.ToString());
			};
		}
		private void UpdateRequestTournamentParticipationPrompt(){
			GameObject promptObj = new GameObject("SponsorQuestPrompt");
			SponsorQuestPrompt prompt = promptObj.AddComponent<SponsorQuestPrompt>();
			prompt.Message = this.promptMessage;
			prompt.Quest = this.currentStory;

			prompt.OnNoClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["participating"] = false;
				EventData evn = new EventData("participation_response", data);
				SendSocketMessage(evn.ToString());
			};
			prompt.OnYesClick = () => {
				this.prompting = false;
				this.promptName = "";
				JObject data = new JObject();
				data["participating"] = true;
				EventData evn = new EventData("participation_response", data);
				SendSocketMessage(evn.ToString());
			};
		}

		/*
			Update functions added to updateQueue from other threads because they
			interact with UnityEngine and must be run from main thread.
		*/
		private void UpdateHand(){
			foreach (Transform child in this.handArea.transform) {
				GameObject.Destroy(child.gameObject);
			}
			foreach(Card c in this.handAreaCards){
				GameObject dCard = (GameObject)Instantiate(Resources.Load("DraggableCard"));
    		dCard.GetComponent<GameCard> ().Card = c;
				dCard.transform.SetParent(this.handArea.transform, false);
				Image im = dCard.GetComponent<Image>();
				im.sprite = (Sprite)Resources.Load<Sprite>(Constants.RESOURCES_CARDS + c.image);
			}
		}
		private void UpdatePlayerArea(){
			foreach (Transform child in this.battleArea.transform) {
				GameObject.Destroy(child.gameObject);
			}
			foreach(Card c in this.playerAreaCards){
				GameObject dCard = (GameObject)Instantiate(Resources.Load("DraggableCard"));
    		dCard.GetComponent<GameCard> ().Card = c;
				dCard.transform.SetParent(this.battleArea.transform, false);
				Image im = dCard.GetComponent<Image>();
				im.sprite = (Sprite)Resources.Load<Sprite>(Constants.RESOURCES_CARDS + c.image);
			}
		}
		private void UpdateOtherArea(){
			foreach (Transform child in this.otherArea.transform) {
				GameObject.Destroy(child.gameObject);
			}
			foreach(Card c in this.otherAreaCards){
				GameObject dCard = (GameObject)Instantiate(Resources.Load("DraggableCard"));
    		dCard.GetComponent<GameCard> ().Card = c;
				dCard.transform.SetParent(this.otherArea.transform, false);
				Image im = dCard.GetComponent<Image>();
				im.sprite = (Sprite)Resources.Load<Sprite>(Constants.RESOURCES_CARDS + c.image);
			}
		}
		private void UpdatePlayers(){
			foreach(Player p in this.players){
				p.UpdateGameObjects();
			}
		}
		private void UpdateStory(){
			currentStoryCard.sprite = (Sprite)Resources.Load<Sprite>(Constants.RESOURCES_CARDS + currentStory.image);
		}
		private void UpdateHistory(){
			if(this.newHistory != ""){
				this.historyButton.GetComponent<Image>().color = Color.yellow;
			}
			this.historyScrollText.text = this.newHistory + this.historyScrollText.text;
			this.newHistory = "";
		}
		private void UpdateOtherAreaNames(){
			this.otherAreaText.text = this.otherAreaName;
			this.otherArea.GetComponent<DropArea>().areaName = this.otherAreaName;
		}
		private void UpdateEndStory(){
			this.confirmationButton.gameObject.SetActive(true);
			this.confirmationButtonText.text = "End Turn";
			this.confirmationMessage = "round_end";
		}
		private void UpdateRequestPlayCards(){
			this.confirmationButton.gameObject.SetActive(true);
			this.confirmationButtonText.text = "Confirm Cards";
			this.confirmationMessage = "confirm_cards";
		}
		private void UpdateRequestStage(){
			this.confirmationButton.gameObject.SetActive(true);
			this.confirmationButtonText.text = "Confirm Cards";
			this.confirmationMessage = "confirm_stage";
		}
		/*
		private void UpdateRequestDiscard(){
			this.confirmationButton.gameObject.SetActive(false);
			this.confirmationButtonText.text = "";
			this.confirmationMessage = "";
		}
		*/

		/*
			Called on new scene.
			Calls Init for specific scene.
		*/
		public void OnUISceneChanged(Scene lastScene, Scene nextScene){
				this.sceneName = nextScene.name;
				this.prepScene = this.sceneName;
				switch(this.sceneName){
					case "MainMenu":
						InitMainMenu();
						break;
					case "Lobby":
						InitLobby();
						break;
					case "Match":
						InitMatch();
						break;
				}
		}
		public void InitMainMenu(){
			DisableObject("Canvas_NetworkGames");
			DisableObject("Button_JoinGame");
			DisableObject("Button_HostGame");
			DisableObject("button_HotSeatPlay");
			DisableObject("button_Scenario1");
			DisableObject("button_Scenario2");
			DisableObject("button_RunSimulation");
		}
		public void InitLobby(){
			Button btnAddAI = GameObject.Find("Button_addAI").GetComponent<Button>();
			Button btnStartGame = GameObject.Find("Button_startGame").GetComponent<Button>();

			btnStartGame.onClick.AddListener(OnUIStartGame);
			btnAddAI.onClick.AddListener(OnUIAddAI);

			DisableObject("Button_addPlayer");
			if(!this.isHost){
				DisableObject("Canvas_AI");
				DisableObject("Button_startGame");
			}
		}
		public void InitMatch(){
			handArea = GameObject.Find("HandPanel");
			battleArea = GameObject.Find("BattleArea");
			otherArea = GameObject.Find("OtherArea");
			otherAreaText = GameObject.Find("OtherAreaText").GetComponent<Text>();
			currentStoryCard = GameObject.Find("StoryCard").GetComponent<Image>();
			confirmationButton = GameObject.Find("ConfirmationButton").GetComponent<Button>();
			confirmationButtonText = GameObject.Find("ConfirmationText").GetComponent<Text>();
			historyScrollText = GameObject.Find("HistoryScrollText").GetComponent<Text>();
			historyScroll = GameObject.Find("HistoryScroll");
			historyButton = GameObject.Find("HistoryButton").GetComponent<Button>();
			historyButtonText = GameObject.Find("HistoryButtonText").GetComponent<Text>();
			historyScroll.SetActive(false);

			GameObject opponentsPanel = GameObject.Find("Opponents");
			foreach(Player p in this.players){
				GameObject opp = (GameObject)Instantiate(Resources.Load("Opponent"));
				opp.transform.SetParent(opponentsPanel.transform, false);
				p.SetGameObjects(opp);
			}

			confirmationButton.onClick.AddListener(OnUIConfirmation);
			historyButton.onClick.AddListener(OnUIHistoryButton);
		}

			/*
				UI triggered Event functions.
			*/
			public void OnUIAddAI(){
				JObject data = new JObject();
				data["strategy"] = GameObject.Find("Dropdown_AIStrategy").GetComponent<Dropdown>().value+1;
				EventData evn = new EventData("add_ai", data);
				SendSocketMessage(evn.ToString());
			}
			public void OnUIInputUsernameValueChanged(string userName){
				if(userName == ""){
					this.userName = Constants.DEFAULT_USERNAME;
				}
				else{
					this.userName = userName;
				}
			}
			public void OnUIInputServerValueChanged(string serverAddress){
				if(serverAddress == ""){
					this.serverAddress = Constants.DEFAULT_SERVER_ADDRESS;
				}
				else{
					this.serverAddress = serverAddress;
				}
			}
			public void OnUIJoinNetwork(){
				if(this.gameid>=0){
					this.isHost = false;
					JObject data = new JObject();
					data["game_id"] = this.gameid;
					EventData evn = new EventData("join_game", data);
					SendSocketMessage(evn.ToString());
					LoadScene("Lobby");
				}
			}
			public void OnUIHostNetwork(){
				this.isHost = true;
				JObject data = new JObject();
				data["scenario"] = 0;
				EventData evn = new EventData("create_game", data);
				SendSocketMessage(evn.ToString());
				LoadScene("Lobby");
			}
			public void OnUIRefreshGames(){
				JObject data = new JObject();
				EventData evn = new EventData("request_games", data);
				SendSocketMessage(evn.ToString());
			}
			public void OnUIStartGame(){
				JObject data = new JObject();
				EventData evn = new EventData("start_game", data);
				GameObject.Find("Button_startGame").SetActive(false);
				SendSocketMessage(evn.ToString());
			}
			public void OnUIConfirmation(){
				if(this.confirmationMessage != ""){
					JObject data = new JObject();
					EventData evn = new EventData(this.confirmationMessage, data);
					SendSocketMessage(evn.ToString());
				}
			}
			public void OnUIScenarioOne(){
				if(isConnected){
					this.isHost = true;
					JObject data = new JObject();
					data["scenario"] = 1;
					EventData evn = new EventData("create_game", data);
					SendSocketMessage(evn.ToString());
				}
				LoadScene("Lobby");
			}
			public void OnUIScenarioTwo(){
				if(isConnected){
					this.isHost = true;
					JObject data = new JObject();
					data["scenario"] = 2;
					EventData evn = new EventData("create_game", data);
					SendSocketMessage(evn.ToString());
				}
				LoadScene("Lobby");
			}
			public void OnUIRunSimulation(){
				if(isConnected){
					JObject data = new JObject();
					EventData evn = new EventData("simulate_game", data);
					SendSocketMessage(evn.ToString());
				}
			}
			public void OnUIDrop(string areaName, string cardName){
				Debug.Log(areaName);
				if(areaName == "Battle Area"){
					JObject data = new JObject();
					JArray cards = new JArray();
					cards.Add(cardName);
					data["cards"] = cards;
					EventData evn = new EventData("play_cards", data);
					SendSocketMessage(evn.ToString());
				}
				else if(areaName == "Quest Area"){
					JObject data = new JObject();
					JArray cards = new JArray();
					cards.Add(cardName);
					data["cards"] = cards;
					EventData evn = new EventData("play_cards_stage", data);
					SendSocketMessage(evn.ToString());
				}
				else if(areaName == "Discard Area"){
					JObject data = new JObject();
					JArray cards = new JArray();
					cards.Add(cardName);
					data["cards"] = cards;
					EventData evn = new EventData("discard", data);
					SendSocketMessage(evn.ToString());
				}
			}
			public void OnUIHistoryButton(){
				if(this.historyScroll.activeSelf){
					this.historyScroll.SetActive(false);
					this.historyButtonText.text = "<";
				}
				else{
					this.historyScroll.SetActive(true);
					this.historyButtonText.text = ">";
				}
				this.historyButton.GetComponent<Image>().color = Color.white;
			}

			/*
				Message received Event functions.
			*/
			public void OnRCVUpdateGames(JToken data){
				JArray arr = (JArray)data["game_ids"];
				this.games = arr.ToObject<List<string>>();
			}
			public void OnRCVUpdatePlayers(JToken data){
				JArray arr = (JArray)data["players"];
				List<Player> ps = arr.ToObject<List<Player>>();
				foreach(Player p in this.players){
					Player p2 = ps.Find(p3 => p3.username == p.username);
					p2.CopyGameObjects(p);
				}
				this.players = ps;
				this.updateQueue.Enqueue(UpdatePlayers);
			}
			public void OnRCVGameStart(JToken data){
				this.prepScene = "Match";
			}
			public void OnRCVUpdateStory(JToken data){
				if(sceneName!="Match"){
					this.prepScene = "Match";
				}
				this.currentStory = data["card"].ToObject<Card>();//new Card((string)data["name"], (string)data["image"]);
				this.updateQueue.Enqueue(UpdateStory);
			}
			public void OnRCVUpdatePlayerArea(JToken data){
				JArray arr = (JArray)data["cards"];
				this.playerAreaCards = arr.ToObject<List<Card>>();
				this.updateQueue.Enqueue(UpdatePlayerArea);
			}
			public void OnRCVUpdateOtherArea(JToken data){
				JArray arr = (JArray)data["cards"];
				this.otherAreaCards = arr.ToObject<List<Card>>();
				this.updateQueue.Enqueue(UpdateOtherArea);
			}
			public void OnRCVUpdateHand(JToken data){
				JArray arr = (JArray)data["cards"];
				this.handAreaCards = arr.ToObject<List<Card>>();
				this.updateQueue.Enqueue(UpdateHand);
			}
			public void OnRCVRequestQuestSponsor(JToken data){
				this.promptName = "SponsorQuestPrompt";
				this.promptMessage = (string)data["message"];
				//TODO: receiving image is redundant for all current prompts.
				//this.promptImage = (string)data["image"];
			}
			public void OnRCVRequestQuestParticipation(JToken data){
				this.promptName = "RequestQuestParticipationPrompt";
				this.promptMessage = (string)data["message"];
				//TODO: receiving image is redundant for all current prompts.
				//this.promptImage = (string)data["image"];
			}
			public void OnRCVMessage(JToken data){
				this.newHistory = data["message"] + "\n" + this.newHistory;
				this.updateQueue.Enqueue(UpdateHistory);
			}
			/*
			public void OnRCVRequestDiscard(JToken data){
				this.otherAreaName = "Discard Area";
				this.updateQueue.Enqueue(UpdateOtherAreaNames);
				this.updateQueue.Enqueue(UpdateRequestDiscard);
				//TODO:this.confirmationMessage = ".....";
			}
			*/
			public void OnRCVRequestStage(JToken data){
				this.otherAreaName = "Quest Area";
				this.updateQueue.Enqueue(UpdateOtherAreaNames);
				this.updateQueue.Enqueue(UpdateRequestStage);
			}
			public void OnRCVRequestPlayCards(JToken data){
				this.otherAreaName = "Stage";
				this.updateQueue.Enqueue(UpdateOtherAreaNames);
				this.updateQueue.Enqueue(UpdateRequestPlayCards);
			}
			public void OnRCVEndStory(JToken data){
				this.otherAreaName = "";
				this.updateQueue.Enqueue(UpdateOtherAreaNames);
				this.updateQueue.Enqueue(UpdateEndStory);
			}
			public void OnRCVRequestTournamentParticipation(JToken data){
				this.promptName = "RequestTournamentParticipationPrompt";
				this.promptMessage = (string)data["message"];
				//TODO: receiving image is redundant for all current prompts.
				//this.promptImage = (string)data["image"];
			}

			/*
				Socket functions.
			*/
			private void Connect(string uri=Constants.DEFAULT_SERVER_ADDRESS){
				if(uri == ""){
					uri = this.serverAddress;
				}
				if (this.socket != null) {
					this.socket.Close ();
					this.socket = null;
				}
				this.socket = new UnityWebSocket (uri);
				this.socket.OnClose += this.OnClose;
				this.socket.OnOpen += this.OnOpen;
				this.socket.OnMessage += this.OnMessage;
				this.socket.OnError += this.OnError;
			}
	    public void SendSocketMessage(string message) {
				if(this.socket==null){
					Debug.Log("ERR: No socket connection.");
					return;
				}
        Debug.Log("Sending message: " + message);
        byte[] data = Encoding.UTF8.GetBytes(message);
        this.socket.SendAsync(data);
	    }
			/*
				Socket events.
			*/
			public void OnClose(UnityWebSocket sender, int code, string reason) {
				Debug.Log("Connection closed: " + reason);
				this.isConnected = false;
			}
			public void OnOpen(UnityWebSocket accepted) {
				Debug.Log("Connection established");
				JObject data = new JObject();
				data["username"] = this.userName;
				EventData evn = new EventData("player_join", data);
				this.SendSocketMessage(evn.ToString());
				this.isConnected = true;
			}
			public void OnMessage(UnityWebSocket sender, byte[] data) {
				string message = Encoding.UTF8.GetString(data);
				JObject jqe = JObject.Parse(message);
				string eventName = (string)jqe["event"];
				Debug.Log("Message received: " + jqe);
				if (eventHandlers.ContainsKey(eventName)) {
					eventHandlers[eventName](jqe["data"]);
				}
			}
	    private void OnError(UnityWebSocket sender, string message) {
	        Debug.Log("Error: " + message);
	    }
			/*
				Add new action to messageHandler.
			*/
			public void On(string eventName, Action<JToken> handler) {
					eventHandlers.Add(eventName, handler);
			}

			/*
				Disable a game object.
				Must be called from main thread.
			*/
			private void DisableObject(string objectName){
				GameObject go = GameObject.Find(objectName);
				if(go != null){
					go.SetActive(false);
					disabledObjects.Add(objectName, go);
				}
			}
			/*
				Enable a game object.
				Must be called from main thread.
			*/
			private void EnableObject(string objectName){
				if(disabledObjects.ContainsKey(objectName)){
					GameObject go = disabledObjects[objectName];
					disabledObjects.Remove(objectName);
					go.SetActive(true);
				}
			}
			/*
				Load scene specified by scene_name.
				Must be called from main thread.
			*/
			public void LoadScene(string scene_name){
				SceneManager.LoadScene (scene_name);
			}
	}

	/*
		Class for handling incoming socket messages.
	*/
	public class EventData {
			private string eventName;
			private JObject data;

			public EventData(string message) {
					JObject eventJson = JObject.Parse(message);
					this.eventName = (string)eventJson["event"];
					this.data = (JObject)eventJson["data"];
			}
			public EventData(string eventName, JObject data) {
					this.eventName = eventName;
					this.data = data;
			}

			public override string ToString() {
					JObject eventJson = new JObject();
					eventJson["event"] = this.eventName;
					eventJson["data"] = data;
					return eventJson.ToString();
			}
	}
}
