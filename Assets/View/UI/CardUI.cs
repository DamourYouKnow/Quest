using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Quest.Core{
	public class CardUI : ScriptableObject {
		public CardUI(){
		}

		public void ShowOpponents(int numOfPlayers){
			GameObject opponents = GameObject.Find ("Opponents");
			for (int i = 0; i < numOfPlayers; i++) {
				GameObject opponent = Instantiate (Resources.Load("Opponent", typeof(GameObject))) as GameObject;
				opponent.transform.SetParent (opponents.transform);
				opponent.transform.localScale = new Vector3 (1, 1, 1);
			}
		}

		//public void ShowHand(int numOfCards, QuestMatch gm){
		//	GameObject hand = GameObject.Find ("HandPanel");
		//	for (int i = 0; i < numOfCards; i++) {
		//		GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
		//		card.transform.SetParent (hand.transform);
		//		card.transform.localScale = new Vector3 (1, 1, 1);
		//		card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + gm.CurrentPlayer.Hand.Cards [i].ImageFilename);
		//	}
		//}
    }
}
