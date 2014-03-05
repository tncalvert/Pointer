using UnityEngine;
using System.Collections;

public class AttackTracker : MonoBehaviour {
	
	public string display;
	
	// Use this for initialization
	void Start () {
		display = "Attack Ready!";
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = display;
	}
	
	//Increments the score
	public void setText(string text)
	{
		display = text;
	}
}
