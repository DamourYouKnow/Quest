using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Core {
	public class MenuStart : MonoBehaviour {

		public void changeMenuScene(string sceneName){
			Application.LoadLevel(sceneName);
		}
		
		public void ExitGame() {
			Application.Quit();
		}
		
		public void StartMatch(){
			QuestMatch questMatch = new QuestMatch();
		}
	}
}