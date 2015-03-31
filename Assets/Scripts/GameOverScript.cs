﻿using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {
    void OnGUI() {
        const int buttonWidth = 120;
        const int buttonHeight = 60;

        // Center in X, 1/3 of the height in Y
        if (GUI.Button(new Rect(Screen.width / 2 - (buttonWidth / 2),
                                (1 * Screen.height / 3) - (buttonHeight / 2),
                                buttonWidth,
                                buttonHeight),
                                "Retry!")
            ) {
            // Reload the level
            Application.LoadLevel("test 2");
        }

        // Center in X, 2/3 of the height in Y
        if (GUI.Button(new Rect(Screen.width / 2 - (buttonWidth / 2), (2 * Screen.height / 3) - (buttonHeight / 2), buttonWidth, buttonHeight),"Back to menu")) {
                // Reload the level
                Application.LoadLevel("Title");
        }
    }
}