﻿//"connected" to the GameController game object
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
		Logger logger;
		public Logger Logger {
			get { return this.logger; }
		}
		public QuestMatch GM {
			get { return this.gm; }
		}

		//Awake is called before Start function, guaranteeing we'll have it setup for other scripts
		void Awake(){
			gm = new QuestMatch();
			logger = new Logger ();
			Logger.Log ("GameController.Awake()");
		}

		// Use this for initialization
		void Start() {

		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
