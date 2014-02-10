using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {

	public GameObject selectorBar;
	public GameObject playButton;
	public GameObject creditsButton;
	public GameObject quitButton;

	private GameObject selectedButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown ("down")) {
			this.selectedButton = this.getNextButton ();
			this.setSelectionBar ();
		} else if (Input.GetKeyDown ("up")) {
			this.selectedButton = this.getPrevButton ();
			this.setSelectionBar();
		}

	}

	private void setSelectionBar(){
		this.selectorBar.transform.position = new Vector3 (this.selectorBar.transform.position.x, this.selectedButton.transform.position.y, this.selectorBar.transform.position.z);
	}

	private GameObject getPrevButton(){
		if (this.selectedButton == this.playButton) {
			return this.quitButton;
		} else if (this.selectedButton == this.creditsButton) {
			return this.playButton;
		} else {
			return this.quitButton;
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
