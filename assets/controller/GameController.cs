//"connected" to the GameController game object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Core {
	public class GameController : MonoBehaviour {

		GameManager gm;
		// Use this for initialization
		void Start () {
			gm = new GameManager();
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
