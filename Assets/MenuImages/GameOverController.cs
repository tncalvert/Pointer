using UnityEngine;
using System.Collections;

public class GameOverController : MonoBehaviour {
	
	public GameObject selectorBar;
	public GameObject playButton;
	public GameObject quitButton;

	private GameObject selectedButton;
	
	// Use this for initialization
	void Start () {
		this.selectedButton = this.playButton;
	}

	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown ("down") || Input.GetKeyDown ("up")) {
			if(this.selectedButton == this.playButton)
				this.selectedButton = this.quitButton;
			else
				this.selectedButton = this.playButton;

			this.setSelectionBar ();
		}
		
		if (Input.GetKeyDown ("return")) {
			
			if (this.selectedButton == this.quitButton)
				Application.Quit();
			else
			{
				Application.LoadLevel("Game");
				PlayerPrefs.SetInt("Score",0);
			}
		}
		
	}
	
	private void setSelectionBar(){
		//this.selectorBar.transform.position = new Vector3 (this.selectorBar.transform.position.x,
		    //                                               this.playButton.transform.position.y - 1.5f * (this.playButton.transform.position.y - this.selectedButton.transform.position.y),
		   //                                                this.selectorBar.transform.position.z);
		//
		this.selectorBar.transform.position = new Vector3 (this.selectorBar.transform.position.x,
		                                                  this.selectedButton.transform.position.y,
		                                                  this.selectorBar.transform.position.z);
			
	}
	
}
