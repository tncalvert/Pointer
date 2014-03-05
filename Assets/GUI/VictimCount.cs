using UnityEngine;
using System.Collections;

public class VictimCount : MonoBehaviour {
	
	public int victimsLeft;
	
	// Use this for initialization
	void Start () {
		victimsLeft = 0;
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Victims Left: " + victimsLeft;
	}
	
	//Increments the score
	public void setVictimsLeft(int victims)
	{
		victimsLeft = victims;
	}
}
