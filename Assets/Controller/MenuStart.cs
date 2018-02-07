using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStart : MonoBehaviour {

	public void changeMenuScene(string sceneName){
		Application.LoadLevel(sceneName);
	}
	
	public void ExitGame() {
		Application.Quit();
	}
	
}
