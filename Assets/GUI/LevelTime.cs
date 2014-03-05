using UnityEngine;
using System.Collections;

public class LevelTime : MonoBehaviour {

	public float timeLeft;

	// Use this for initialization
	void Start () {
		timeLeft = 60;
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Time Left: " + (int)timeLeft;
		timeLeft -= Time.deltaTime;

		//If time runs out, game over!
		if(timeLeft <= 0)
			Application.LoadLevel("GameOver");
	}
}
