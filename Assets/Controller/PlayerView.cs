using System;
using System.Text;
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
	}
	public class PlayerView : MonoBehaviour {
	    private UnityWebSocket socket;
			private GameObject gameCanvas;
			private string sceneName;
			private string userName;
			private string serverAddress;
			private bool isHost;
			private bool isConnected;
			private List<string> games;
			private Dictionary<string, GameObject> disabledObjects;
			private int scenario;
			private Dictionary<string, Action<JToken>> eventHandlers;
			private int gameid;

			public int Gameid{
				get {return this.gameid;}
				set {this.gameid = value;}
			}
			public GameObject GameCanvas {
				get {return this.gameCanvas;}
				set {this.gameCanvas = value;}
			}

		private void Awake(){
			this.disabledObjects = new Dictionary<string, GameObject>();
			this.serverAddress = Constants.DEFAULT_SERVER_ADDRESS;
			this.userName = Constants.DEFAULT_USERNAME;
			this.isHost = false;
			this.isConnected = false;
			this.scenario = 0;
			this.gameid = -1;
			this.games = new List<string>();
			this.eventHandlers = new Dictionary<string, Action<JToken>>();
			On("update_games", OnRCVUpdateGames);
			//messageHandler.On("player_join", OnPlayerJoined);
			SceneManager.activeSceneChanged += OnUISceneChanged;
			//messageHandler.On("player_join", OnPlayerJoined);
			DontDestroyOnLoad (this);
		}

    private void Start() {
    }

		public void InitMainMenu(){
			DisableObject("Canvas_NetworkGames");
			DisableObject("Button_JoinGame");
			DisableObject("Button_HostGame");
		}

		public void InitLobby(){

		}

		public void Update(){
				if (this.gameCanvas == null){
						GameObject gc = GameObject.Find ("GameCanvas");
						if (gc != null) {
								this.gameCanvas = gc;
						}
				}
				if (this.isConnected){
					UpdateOnline();
				}
				else{
					UpdateOffline();
				}
		}

		private void UpdateOnline(){
			switch(this.sceneName){
				case "MainMenu":
					UpdateOnlineMainMenu();
					break;
			}
		}

		private void UpdateOnlineMainMenu(){
			if(!this.disabledObjects.ContainsKey("Button_Connect")){
				EnableObject("Canvas_NetworkGames");
				EnableObject("Button_HostGame");
				EnableObject("Button_JoinGame");
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

		}

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
				EnableObject("Canvas_Username");
				EnableObject("Canvas_Server");
				EnableObject("Button_Connect");
			}
		}

		public void OnUISceneChanged(Scene lastScene, Scene nextScene){
				this.sceneName = nextScene.name;
				switch(this.sceneName){
					case "MainMenu":
						InitMainMenu();
						break;
				}
		}

			//public void OnUpdateGames()
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
					SendMessage(evn.ToString());
					LoadScene("Lobby");
				}
			}
			public void OnUIHostNetwork(){
				this.isHost = true;
				JObject data = new JObject();
				data["scenario"] = 0;
				EventData evn = new EventData("create_game", data);
				SendMessage(evn.ToString());
				LoadScene("Lobby");
			}
			public void OnUIRefreshGames(){
				JObject data = new JObject();
				EventData evn = new EventData("request_games", data);
				SendMessage(evn.ToString());
			}

			public void On(string eventName, Action<JToken> handler) {
					eventHandlers.Add(eventName, handler);
			}
			public void OnRCVUpdateGames(JToken data){
				JArray arr = (JArray)data["game_ids"];
				this.games = arr.ToObject<List<string>>();
			}
			public void OnRCVPlayerUpdate(){
					switch(this.sceneName){
							case "Main Menu":
									break;
							case "Lobby":
							/*
									GameObject list = GameObject.Find ("List_of_players");
									if (list != null) {
										Text t = list.GetComponent<Text> ();
										t.text = t.text + "\n" + this.numPlayers.ToString() + "Human";
									}
									*/
									break;
							case "Match":
									break;
					}
			}

			private void DisableObject(string objectName){
				GameObject go = GameObject.Find(objectName);
				if(go != null){
					go.SetActive(false);
					disabledObjects.Add(objectName, go);
				}
			}
			private void EnableObject(string objectName){
				if(disabledObjects.ContainsKey(objectName)){
					GameObject go = disabledObjects[objectName];
					disabledObjects.Remove(objectName);
					go.SetActive(true);
				}
			}
			public void LoadScene(string scene_name){
				SceneManager.LoadScene (scene_name);
			}

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
	    public void SendMessage(string message) {
				if(this.socket==null){
					Debug.Log("ERR: No socket connection.");
					return;
				}
        Debug.Log("Sending message: " + message);
        byte[] data = Encoding.UTF8.GetBytes(message);
        this.socket.SendAsync(data);
	    }

			public void OnClose(UnityWebSocket sender, int code, string reason) {
				Debug.Log("Connection closed: " + reason);
				this.isConnected = false;
			}
			public void OnOpen(UnityWebSocket accepted) {
				Debug.Log("Connection established");
				JObject data = new JObject();
				data["username"] = this.userName;
				EventData evn = new EventData("player_join", data);
				this.SendMessage(evn.ToString());
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
	}

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
