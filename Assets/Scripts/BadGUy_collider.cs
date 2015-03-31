using UnityEngine;
using System.Collections;

public class BadGUy_collider : MonoBehaviour {
	public Transform YouLose;


	// Use this for initialization
	void Start () {

		GUIText lose = YouLose.GetComponent<GUIText>();  
		lose.enabled = false;

	}
	
	// Update is called once per frame

	void OnTriggerEnter2D(Collider2D coll) {               //Calls for update when its run into



		if (coll.tag == "Player") {
			Debug.Log ("Here");
			GUIText lose = YouLose.GetComponent<GUIText>();  
			lose.enabled = true;
			Time.timeScale = 0;
		



			
		}
	}
}

				

