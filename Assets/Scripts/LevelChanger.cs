using UnityEngine;
using System.Collections;

/// <summary>
/// Class responsible for loading the next level, when the player beats one
/// </summary>
public class LevelChanger : MonoBehaviour {

    /// <summary>
    /// The number of victims currently in the level
    /// </summary>
    private int numberOfVictims;

    /// <summary>
    /// Flag indicating that all the victims are dead
    /// </summary>
    private bool allVictimsDead;

	// Use this for initialization
	void Start () {
        numberOfVictims = 0;
        allVictimsDead = false;
	}

    void OnGUI() {
        if (allVictimsDead) {
            if (GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height / 2) - 37, 100, 75), "Next Level")) {
                Application.LoadLevel("Game");
            }
        }
    }

    /// <summary>
    /// Called by a victim when it is created, increments the total count
    /// </summary>
    public void registerVictim() {
        ++numberOfVictims;
    }

    /// <summary>
    /// Called by a victim before it is destroyed, decrements the total count
    /// </summary>
    public void victimDied() {
        --numberOfVictims;
        if (numberOfVictims == 0) {
            allVictimsDead = true;
        }
    }
}
