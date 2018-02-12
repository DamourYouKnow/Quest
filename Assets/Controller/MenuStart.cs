using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quest.Core.Players;

namespace Quest.Core {
	public class MenuStart : MonoBehaviour {
		
		int numHumanPlayers = 0;
		int numAIPlayers = 0;
		
		public void changeMenuScene(string sceneName){
			SceneManager.LoadScene(sceneName);
		}
		
		public void ExitGame() {
			Application.Quit();
		}
		
		public void StartMatch(){
			QuestMatch questMatch = new QuestMatch();
			
			for(int i = 0; i < numHumanPlayers; i++){
				Player newPlayer = new Player("test human");
				questMatch.AddPlayer(newPlayer);
			}
			for(int i = 0; i < numAIPlayers; i++){
				Player newAI = new Player("test AI");
				questMatch.AddPlayer(newAI);
			}

			changeMenuScene ("Match");
		}
		
		public void AddHuman(){
			numHumanPlayers += 1;
		}
		
		public void AddAI(){
			numAIPlayers += 1;
		}
	}
}