using UnityEngine;
using System.Collections;

public class HealthDisplay : MonoBehaviour {
	
	public float health;
	
	// Use this for initialization
	void Start () {
		health = 9;
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Health: " + health;
	}
	
	//Increments the score
	public void doDamage(float damage)
	{
		health -= damage;
	}
}