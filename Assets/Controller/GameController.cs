//"connected" to the GameController game object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest.Core;

//I call this a change
namespace Quest.Core {
	public class GameController : MonoBehaviour {
		GameManager gm;
		Logger Logger;
		public Logger log {
			get { return this.Logger; }
		}

		//Awake is called before Start function, guaranteeing we'll have it setup for other scripts
		void Awake(){
			gm = new GameManager();
			Logger = new Logger ();
			Logger.Log ("GameController.Awake()");
		}

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
