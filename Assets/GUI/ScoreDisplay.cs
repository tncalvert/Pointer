using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public int playerScore;

	// Use this for initialization
	void Start () {
		playerScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Score: " + playerScore;
	}

	//Increments the score
	public void incrementScore()
	{
		playerScore++;
	}
}
