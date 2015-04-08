using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		const int buttonWidth = 84;
		const int buttonHeight = 60;

		Rect buttonRect = new Rect (
			Screen.width / 2 - (buttonWidth / 20),
			(2 * Screen.height / 3) - (buttonHeight / 2),
			buttonWidth,
			buttonHeight
		);

		if (GUI.Button (buttonRect, "Start")) {
			Application.LoadLevel (1);
		}


	}
}
