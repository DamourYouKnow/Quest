//"connected" to the GameController game object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest.Core;
using Quest.UI;

//I call this a change
namespace Quest.Core {
	public class GameController : MonoBehaviour {
        GameObject testPromptObj;
		QuestMatch gm;
		Logger Logger;
		public Logger log {
			get { return this.Logger; }
		}

		//Awake is called before Start function, guaranteeing we'll have it setup for other scripts
		void Awake(){
			gm = new QuestMatch();
			Logger = new Logger ();
			Logger.Log ("GameController.Awake()");
		}

		// Use this for initialization
		void Start () {
            Canvas canvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
            testPromptObj = new GameObject("TestPrompt");
            YesNoPrompt prompt = testPromptObj.AddComponent<YesNoPrompt>();
            prompt.Canvas = canvas;
            prompt.Message = "Hello, world!";
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
