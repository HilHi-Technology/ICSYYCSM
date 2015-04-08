using UnityEngine;
using System.Collections;

public class TextAppearAfterDeath : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        GetComponent<GUIText>().enabled = false;
        if (PlayerScript.playerDiedThisLevel) {
            GetComponent<GUIText>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
