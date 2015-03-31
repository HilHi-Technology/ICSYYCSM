﻿using UnityEngine;
using System.Collections;

public class BadGuyCollider : MonoBehaviour {
	public Transform YouLose;
	// Use this for initialization
	void Start () {
		GUIText lose = YouLose.GetComponent<GUIText>();  
		lose.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll) { 
        //If player runs into the bad guy, pause the game and display retry buttons
		if (coll.tag == "Player") {
			Debug.Log ("Here");
			GUIText lose = YouLose.GetComponent<GUIText>();  
			lose.enabled = true;
			transform.gameObject.AddComponent<GameOverScript>();
			Time.timeScale = 0;
		}
	}
}