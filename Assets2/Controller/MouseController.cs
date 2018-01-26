using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interacts with the mouse, it's whereabouts, and selected objects
/// </summary>
public class MouseController : MonoBehaviour {
	Vector3 currPosition;
	bool isDragging;

	//Game Object under mouse
	GameObject selectedGameObject;
	//Mouse location relative to selectedGameObject's origin
	Vector3 selectedGameObjectOffset;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
		isDragging = false;
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
		//Get position relative to world
		currPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		//Reset z to ensure z remains consistent in our 2d world
		currPosition.z = 0; 

		if (isDragging)
			drag ();
		else {
			raycastSelectedGameObject ();

			if (Input.GetMouseButtonDown (0)) {
				leftMouseButtonDown ();
			}
		}
	}

	/// <summary>
	/// Used to move selected object with mouse.
	/// TODO: Implement Draggable interface to distinguish objects that cannot be dragged, currently drags any Game Object.
	/// </summary>
	void drag(){
		if (Input.GetMouseButton (0)) {
			Vector3 newPosition = currPosition + selectedGameObjectOffset;
			selectedGameObject.transform.SetPositionAndRotation (newPosition, Quaternion.identity);
		}
		else {
			isDragging = false;
		}
	}

	/// <summary>
	/// Called if left mouse button was pressed down in last frame.
	/// </summary>
	void leftMouseButtonDown(){
		if (selectedGameObject != null) {
			isDragging = true;
		}
	}

	/// <summary>
	/// Finds object under mouse by means of raycast.
	/// Will only find Game Objects with Colliders.
	/// </summary>
	void raycastSelectedGameObject(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		//if hit found
		if (Physics.Raycast (ray, out hit)) {
			selectedGameObject = hit.transform.gameObject;
			if (selectedGameObject != null) {
				selectedGameObjectOffset = selectedGameObject.transform.position - currPosition;
			}
		}
		//if miss, clear previous hit
		else {
			selectedGameObject = null;
		}
	}
}
