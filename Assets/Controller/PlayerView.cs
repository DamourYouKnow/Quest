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
			private List<int> games;
			private Dictionary<string, GameObject> disabledObjects;
			private int scenario;

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
			SceneManager.activeSceneChanged += OnSceneChanged;
			DontDestroyOnLoad (this);
		}

    private void Start() {
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

		public void UpdateOnline(){
			switch(this.sceneName){
				case "MainMenu":
					UpdateOnlineMainMenu();
					break;
			}
		}

		public void UpdateOnlineMainMenu(){
			if(!this.disabledObjects.ContainsKey("Button_Connect")){
				EnableObject("Button_HostNetwork");
				EnableObject("Button_JoinNetwork");
				DisableObject("Canvas_Username");
				DisableObject("Canvas_Server");
				DisableObject("Button_Connect");
			}
		}

		public void UpdateOffline(){
			switch(this.sceneName){
				case "MainMenu":
					UpdateOfflineMainMenu();
					break;
			}
		}

		public void UpdateOfflineMainMenu(){
			if(this.disabledObjects.ContainsKey("Button_Connect")){
				DisableObject("Button_HostNetwork");
				DisableObject("Button_JoinNetwork");
				EnableObject("Canvas_Username");
				EnableObject("Canvas_Server");
				EnableObject("Button_Connect");
			}
		}

		public void OnSceneChanged(Scene lastScene, Scene nextScene){
				this.sceneName = nextScene.name;
				switch(this.sceneName){
					case "MainMenu":
						InitMainMenu();
						break;
				}
		}

		public void InitMainMenu(){
			DisableObject("Button_JoinNetwork");
			DisableObject("Button_HostNetwork");
		}

		public void InitLobby(){

		}

		public void DisableObject(string objectName){
			GameObject go = GameObject.Find(objectName);
			if(go != null){
				go.SetActive(false);
				disabledObjects.Add(objectName, go);
			}
		}

		public void EnableObject(string objectName){
			if(disabledObjects.ContainsKey(objectName)){
				GameObject go = disabledObjects[objectName];
				disabledObjects.Remove(objectName);
				go.SetActive(true);
			}
		}

		public void Connect(string uri=Constants.DEFAULT_SERVER_ADDRESS){
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
				try{
	        string message = Encoding.UTF8.GetString(data);
					JObject jqe = JObject.Parse(message);
	        Debug.Log("Message received: " + jqe);
					switch((string)jqe["event"]){
							//case "update_games":
							//		OnUpdateGames();
							default:
									break;

							//case RECEIVEGAMEID
					}
					/*
					switch(this.sceneName){
							case "Main Menu":
									break;
							case "Lobby":

									break;
							case "Match":
									break;
					}
					*/
				}
				catch(Exception e){
					Debug.Log("Exception: " + e);
				}
	    }

			//public void OnUpdateGames()
			public void OnInputUsernameValueChanged(string userName){
				if(userName == ""){
					this.userName = Constants.DEFAULT_USERNAME;
				}
				else{
					this.userName = userName;
				}
			}
			public void OnInputServerValueChanged(string serverAddress){
				if(serverAddress == ""){
					this.serverAddress = Constants.DEFAULT_SERVER_ADDRESS;
				}
				else{
					this.serverAddress = serverAddress;
				}
			}

			public void OnPlayerUpdate(){
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

	    public void SendMessage(string message) {
					if(this.socket==null){
							Debug.Log("ERR: No socket connection.");
							return;
					}
	        Debug.Log("Sending message: " + message);
	        byte[] data = Encoding.UTF8.GetBytes(message);
	        this.socket.SendAsync(data);
	    }

	    private void OnError(UnityWebSocket sender, string message) {
	        Debug.Log("Error: " + message);
	    }

		public void LoadScene(string scene_name){
			SceneManager.LoadScene (scene_name);
		}

		public void OnJoinNetwork(){
			this.isHost = false;
			JObject data = new JObject();
			EventData evn = new EventData("request_games", data);
			SendMessage(evn.ToString());
		}

		public void OnHostNetwork(){
			this.isHost = true;
			JObject data = new JObject();
			data["scenario"] = 0;
			EventData evn = new EventData("create_game", data);
			SendMessage(evn.ToString());
			LoadScene("Lobby");
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
