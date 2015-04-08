using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            Application.LoadLevel(Application.loadedLevel + 1);
            PlayerScript.playerDiedThisLevel = false;
        }
    }
}
