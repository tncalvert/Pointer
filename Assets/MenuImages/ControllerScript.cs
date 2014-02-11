using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {

	public GameObject selectorBar;
	public GameObject playButton;
	public GameObject creditsButton;
	public GameObject quitButton;

	public Transform mainLookAt;
	public Camera camera;


	private GameObject selectedButton;

	private bool inTransitionToCredits = false;
	private float angleChangeToCredits = 0.0f;
	private float transSpeed = 10f;

	// Use this for initialization
	void Start () {
		this.camera.transform.LookAt (this.mainLookAt.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (this.inTransitionToCredits) {
			if (this.angleChangeToCredits <110f) {
				this.camera.transform.Rotate (Vector3.up * transSpeed);
				this.angleChangeToCredits += transSpeed;

			}

			if (Input.GetKeyDown ("return")) {
				this.inTransitionToCredits = false;
				//this.angleChangeToCredits = 110;
			}

			return;
		} else {
			if (this.angleChangeToCredits > 0f) {
				this.camera.transform.Rotate (Vector3.up * -transSpeed);
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
