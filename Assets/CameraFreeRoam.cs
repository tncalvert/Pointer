using UnityEngine;
using System.Collections;

public class CameraFreeRoam : MonoBehaviour {

	public float moveSpeed = 1f;
	public float rotateSpeed = 2f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {



		transform.Rotate(this.rotateSpeed *new Vector3 (0, Input.GetAxis ("Mouse X"), 0));

		if (Input.GetKey("w")) {
			transform.position += (transform.forward * this.moveSpeed);
		}
		if (Input.GetKey("s")) {
			this.transform.position += this.transform.forward * -this.moveSpeed;
		}
		if (Input.GetKey("d")) {
			this.transform.position += this.transform.right * this.moveSpeed;
		}
		if (Input.GetKey("a")) {
			this.transform.position += this.transform.right * -this.moveSpeed;
		}
		if (Input.GetKey("e")) {
			this.transform.position += this.transform.up * this.moveSpeed;
		}
		if (Input.GetKey("q")) {
			this.transform.position += this.transform.up * -this.moveSpeed;
		}

	}
}
