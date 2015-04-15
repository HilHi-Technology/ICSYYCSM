using UnityEngine;
using System.Collections;

public class BadGuyCollider : MonoBehaviour {
	public Transform YouLose;
	public AudioSource playerSource;
	public AudioClip deathSound;
    
	// Use this for initialization
	void Start () {
		GUIText lose = YouLose.GetComponent<GUIText>();  
		lose.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll) { 
        //If player runs into the bad guy, pause the game and display retry buttons
		if (coll.gameObject.tag == "Player") {
			//Debug.Log ("Here");
			playerSource.PlayOneShot(deathSound);
			GUIText lose = YouLose.GetComponent<GUIText>();  
			lose.enabled = true;
			transform.gameObject.AddComponent<GameOverScript>();
            Cursor.visible = true;
			Time.timeScale = 0;
            PlayerScript.playerDiedThisLevel = true;
		}
	}
}
