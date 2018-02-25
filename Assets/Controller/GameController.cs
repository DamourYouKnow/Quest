//"connected" to the GameController game object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quest.Core;
using Quest.Core.Players;
using Quest.Core.Cards;

namespace Quest.Core {
	public class GameController : MonoBehaviour {
		QuestMatch gm;
		Logger logger;

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
				
			}
			else{
				if(SceneManager.GetActiveScene().name == "Match"){
					SetupMatchScene ();
					sceneSet = true;
				}
			}
		}


		public void LoadScene(string sceneName){
			SceneManager.LoadScene(sceneName);
			sceneSet = false;

		}
		private void SetupMatchScene (){
			GameObject opponents = GameObject.Find ("Opponents");
			for (int i = 0; i < this.gm.Players.Count; i++) {
				GameObject opponent = Instantiate (Resources.Load("Opponent", typeof(GameObject))) as GameObject;
				opponent.transform.SetParent (opponents.transform);
				opponent.transform.localScale = new Vector3 (1, 1, 1);
			}
			this.gm.Setup ();
			this.ShowHand ();
		}

		public void ShowHand(){
			GameObject hand = GameObject.Find ("HandPanel");
			for (int i = 0; i < this.gm.CurrentPlayer.Hand.Count; i++) {
				GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
				card.transform.SetParent (hand.transform);
				card.transform.localScale = new Vector3 (1, 1, 1);
				card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + this.gm.CurrentPlayer.Hand.Cards [i].ImageFilename);


				/*Ideas to change the card image that dident work but might with some tweeking
				 
				card.GetComponent<Image> ().overrideSprite = Resources.Load<Sprite> ();
				Sprite cardSprite = Resources.Load (this.gm.CurrentPlayer.Hand.Cards [i].getFilename) as Sprite;
				card.GetComponent<SpriteRenderer> ().sprite = cardSprite;
				Renderer rend = card.GetComponent<Renderer> ();
				rend.material.mainTexture = Resources.Load (this.gm.CurrentPlayer.Hand.Cards [i].getFilename) as Texture;
				*/
			}
		}

		public void StartGame(){
			gm.RunGame ();
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
				this.gm.AddPlayer (new Player ("Human", this.gm));
				GameObject list = GameObject.Find ("List_of_players");
				if (list != null) {
					Text t = list.GetComponent<Text> ();
					t.text = t.text + "\nHuman";
				}
			}
		}

		public void AddAIPlayers(int num){
			for (int i = 0; i < num; i++) {
				this.gm.AddPlayer (new Player ("AI", this.gm));
				GameObject list = GameObject.Find ("List_of_players");
				if (list != null) {
					Text t = list.GetComponent<Text> ();
					t.text = t.text + "\nAI";
				}
			}
		}
	}
}
