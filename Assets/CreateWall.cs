using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateWall : MonoBehaviour {

	bool creating;
	bool zSnap;
	bool xSnap;
	bool poleSnap = true;
	int deleting = 0;

	public GameObject start;
	public GameObject end;
	public GameObject wallPrefab;

	GameObject wall;
	GameObject startPole;
	GameObject endPole;

	List<GameObject> poles;
	List<GameObject> walls;

	// Use this for initialization
	void Start () {
		poles = new List<GameObject> ();
		walls = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		getInput ();
	}

	void getInput(){
		if (Input.GetMouseButtonDown (0)) {
			setStart ();
		} else if (Input.GetMouseButtonUp (0)) {
			setEnd ();
		} else {
			if (creating) {
				adjust ();
			}
		}

		if (Input.GetKey (KeyCode.X)) {
			xSnap = true;
		} else {
			xSnap = false;
		}

		if (Input.GetKey (KeyCode.Y)) {
			zSnap = true;
		} else {
			zSnap = false; 
		}

		if (Input.GetKey (KeyCode.P)) {
			poleSnap = false;
		} else {
			poleSnap = true;
		}

		if ((Input.GetKey (KeyCode.LeftShift) && Input.GetKey (KeyCode.Z)) || (Input.GetKey (KeyCode.RightShift) && Input.GetKey (KeyCode.Z))) {
			if (walls.Count > 0 && poles.Count > 0 && deleting !=1) {
				Destroy (walls [walls.Count - 1]);
				Destroy (poles [poles.Count - 1]);
				Destroy (poles [poles.Count - 2]);
				walls.RemoveAt (walls.Count - 1);
				poles.RemoveAt (poles.Count - 1);
				poles.RemoveAt (poles.Count - 1);
				deleting = 1;
			}
		} else {
			deleting = 0;
		}
	}

	void setStart(){
		creating = true;
		start.transform.position = gridSnap(getWorldPoint ());
		wall = Instantiate (wallPrefab, gridSnap(start.transform.position), Quaternion.identity) as GameObject;
		if (poleSnap && poles.Count>0) {
			start.transform.position = closestPoleTo (getWorldPoint ()).transform.position;
		}
	}

	GameObject closestPoleTo(Vector3 worldPoint){
		GameObject result = null;

		float distance = Mathf.Infinity;
		float currentDistance = Mathf.Infinity;

		foreach (GameObject p in poles) {
			currentDistance = Vector3.Distance (worldPoint, p.transform.position);
			if (currentDistance < distance) {
				distance = currentDistance;
				result = p;
			}
		}

		return result;
	}

	void setEnd(){
		creating = false;
		//end.transform.position = getWorldPoint ();
		setEndPoles ();
	}

	void setEndPoles(){
		GameObject p1 = Instantiate (wallPrefab, start.transform.position, start.transform.rotation) as GameObject;
		GameObject p2 = Instantiate (wallPrefab, end.transform.position, end.transform.rotation) as GameObject;
		p1.tag = "Pole";
		p2.tag = "Pole";
		poles.Add (p1);
		poles.Add (p2);
		walls.Add (wall);
	}

	void adjust(){
		end.transform.position = gridSnap(getWorldPoint ());
		if (xSnap) {
			end.transform.position = new Vector3 (start.transform.position.x, end.transform.position.y, end.transform.position.z);
		}
		if (zSnap) {
			end.transform.position = new Vector3 (end.transform.position.x, end.transform.position.y, start.transform.position.z);
		}
		adjustWall();
	}

	void adjustWall(){
		start.transform.LookAt(end.transform.position);
		end.transform.LookAt (start.transform.position);
		float distance = Vector3.Distance (start.transform.position, end.transform.position);
		wall.transform.position = 0.5f*(start.transform.position+end.transform.position);
		wall.transform.rotation = start.transform.rotation;
		wall.transform.localScale = new Vector3 (wall.transform.localScale.x, wall.transform.localScale.y, distance);

	}

	Vector3 getWorldPoint(){
		Ray ray = GetComponent<Camera>().ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			return hit.point;
		} else {
			return new Vector3(0,0);
		}
	}

	Vector3 gridSnap(Vector3 originalPos){
		int granularity = 1; 
		Vector3 snappedPosition = new Vector3 (Mathf.Floor (originalPos.x / granularity) * granularity, originalPos.y, Mathf.Floor (originalPos.z / granularity) * granularity);
		return snappedPosition;
	}
}
