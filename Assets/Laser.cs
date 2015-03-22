using UnityEngine;
using System.Collections;


public class Laser : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	void onCollisionEnter2D(Collider2D other)//Should call for update on collision with the collision box
	{
		if(
		G
	}*/
	void OnTriggerEnter2D(Collider2D collider) {
		Debug.Log ("Collision: " + collider.name);

	}
}
