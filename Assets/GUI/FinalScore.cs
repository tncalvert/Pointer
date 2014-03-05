using UnityEngine;
using System.Collections;

public class FinalScore : MonoBehaviour {
	
	public string display;
	
	// Use this for initialization
	void Start () {
		display = "Final Score: " + PlayerPrefs.GetInt("Score");
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = display;
	}
}
