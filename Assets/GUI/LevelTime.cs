using UnityEngine;
using System.Collections;

public class LevelTime : MonoBehaviour {

	public float timeLeft;
	public int keepCounting;

	// Use this for initialization
	void Start () {
		timeLeft = 120;
		keepCounting = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(keepCounting == 1)
		{
			guiText.text = "Time Left: " + (int)timeLeft;
			timeLeft -= Time.deltaTime;

			//If time runs out, game over!
			if(timeLeft <= 0)
				Application.LoadLevel("GameOver");
		}
	}

	//Stops counting time
	public void stopCounting()
	{
		keepCounting = 0;
	}


	//Starts counting time again
	public void startCounting()
	{
		keepCounting = 1;
	}
}
