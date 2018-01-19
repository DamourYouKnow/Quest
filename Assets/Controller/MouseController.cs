using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
	Vector3 prevPosition; //Position last frame
	Vector3 currPosition;
	GameObject selectedGameObject; //Game Object under mouse
	Vector3 selectedGameObjectOffset;
	bool isDragging;

	// Use this for initialization
	void Start () {
		isDragging = false;
	}
	
	// Update is called once per frame
	void Update () {
		currPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currPosition.z = 0;

		if (selectedGameObject != null) {
			//Debug.Log (selectedGameObject);
		}
		else {
			//Debug.Log (currPosition);
		}
		//if not dragging then update this.selectedGameObject.  This prevents the case where the mouse moves ahead of the game
		//object while dragging, causing it to lose focus on the one its dragging.
		//Also disallows other mouse actions during dragging
		if (!isDragging) {
			//Debug.Log ("Not dragging");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				selectedGameObject = hit.transform.gameObject;
				if (selectedGameObject != null) {
					selectedGameObjectOffset = selectedGameObject.transform.position-currPosition;
				}
			} 
			else {
				selectedGameObject = null;
			}
			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("MouseDown");
				if (selectedGameObject != null) {
					isDragging = true;
				}
			}
			if (Input.GetMouseButtonUp (0)) {

			}
		}
		else {
			if (Input.GetMouseButton (0)) {
				Vector3 newPosition = currPosition + selectedGameObjectOffset;
				selectedGameObject.transform.SetPositionAndRotation (newPosition, Quaternion.identity);
			}
			else {
				isDragging = false;
			}
		}

		prevPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currPosition.z = 0;
	}
}
