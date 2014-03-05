using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {

	public GameObject selectorBar;
	public GameObject playButton;
	public GameObject creditsButton;
	public GameObject quitButton;

	public Transform mainLookAt;
	public Camera ourCamera;


	private GameObject selectedButton;

	private bool inTransitionToCredits = false;
	private float angleChangeToCredits = 0.0f;
	private float transSpeed = 10f;


	private bool inTransitionToHelp = false;
	private float distToHelp = 0.0f;

	// Use this for initialization
	void Start () {
		this.ourCamera.transform.LookAt (this.mainLookAt.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	

		if (this.inTransitionToHelp) {
			if (this.distToHelp < 8){
				this.ourCamera.transform.position += Vector3.up * this.transSpeed*.2f;
				this.distToHelp += this.transSpeed*.2f;
			}
		}

		if (this.inTransitionToCredits) {
			if (this.angleChangeToCredits <110f) {
				this.ourCamera.transform.Rotate (Vector3.up * transSpeed);
				this.angleChangeToCredits += transSpeed;

			}

			if (Input.GetKeyDown ("return")) {
				this.inTransitionToCredits = false;
				//this.angleChangeToCredits = 110;
			}

			return;
		} else {
			if (this.angleChangeToCredits > 0f) {
				this.ourCamera.transform.Rotate (Vector3.up * -transSpeed);
				this.angleChangeToCredits -= transSpeed;
				
			}
		}

		if (Input.GetKeyDown ("down")) {
			this.selectedButton = this.getNextButton ();
			this.setSelectionBar ();
		} else if (Input.GetKeyDown ("up")) {
			this.selectedButton = this.getPrevButton ();
			this.setSelectionBar();
		}

		if (Input.GetKeyDown ("return")) {

			if (this.selectedButton == this.quitButton){
				Application.Quit();
			} else if (this.selectedButton == this.creditsButton){
				this.inTransitionToCredits = true;
				//this.angleChangeToCredits = 0;
			} else {
				if (this.inTransitionToHelp){
					Application.LoadLevel("Game");
					PlayerPrefs.SetInt("Score",0);
				}
				this.inTransitionToHelp = true;

			}

		}

	}

	private void setSelectionBar(){
		this.selectorBar.transform.position = new Vector3 (this.selectorBar.transform.position.x,
		                                                   this.playButton.transform.position.y - 1.5f * (this.playButton.transform.position.y - this.selectedButton.transform.position.y),
		                                                   this.selectorBar.transform.position.z);
	}

	private GameObject getPrevButton(){
		if (this.selectedButton == this.playButton) {
			return this.quitButton;
		} else if (this.selectedButton == this.creditsButton) {
			return this.playButton;
		} else {
			return this.creditsButton;
		}
	}

	private GameObject getNextButton(){
		if (this.selectedButton == this.playButton) {
			return this.creditsButton;
		} else if (this.selectedButton == this.creditsButton) {
			return this.quitButton;
		} else {
			return this.playButton;
		}
	}

}
