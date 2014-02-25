using UnityEngine;
using System.Collections;

public class LevelTime : MonoBehaviour {

	float timeLeft = 1000;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Time Left: " + (int)timeLeft;
		timeLeft -= Time.deltaTime;
	}
}
