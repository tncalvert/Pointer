using UnityEngine;
using System.Collections;

public class CameraBirdsEye : MonoBehaviour {

	public float moveSpeed = 2;
	public float rotateSpeed = 2;


	void Start () {
		//this.transform.rotation.Set (90, 0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKey("w")) {
			transform.position += (transform.up * this.moveSpeed);
		}
		if (Input.GetKey("s")) {
			this.transform.position += this.transform.up * -this.moveSpeed;
		}
		if (Input.GetKey("d")) {
			this.transform.position += this.transform.right * this.moveSpeed;
		}
		if (Input.GetKey("a")) {
			this.transform.position += this.transform.right * -this.moveSpeed;
		}
		if (Input.GetKey("e")) {
			this.transform.position += this.transform.forward * this.moveSpeed;
		}
		if (Input.GetKey("q")) {
			this.transform.position += this.transform.forward * -this.moveSpeed;
		}

	}
}
