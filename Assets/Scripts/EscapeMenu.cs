using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the escape menu which can bring you back to the main menu while in game
/// </summary>
public class EscapeMenu : MonoBehaviour {

    /// <summary>
    /// Toggles showing of the menu
    /// </summary>
    private bool showMenu;

    /// <summary>
    /// The inspector to handle pausing
    /// </summary>
    private Inspector inspector;

    void Start() {
        showMenu = false;
        inspector = GetComponent<Inspector>();
    }

	void OnGUI () {
        if (showMenu) {
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 50, 150, 50), "Back to Main Menu")) {
                Application.LoadLevel("TopMenu");
            } else if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 + 25, 150, 50), "Resume")) {
                showMenu = false;
                inspector.setEscapePause(false);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            showMenu = true;
            inspector.setEscapePause(true);
        }

	}
}
