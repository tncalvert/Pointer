using UnityEngine;
using System.Collections;

public class CloudScript : MonoBehaviour {

	public float speed;
	public float radius;
	public float moveSpeed;

	private float angle;
	private Vector2 start;
	// Use this for initialization
	void Start () {
		angle = 0;
		start = new Vector2 (this.transform.position.x, this.transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {

		angle += moveSpeed;

		this.transform.position = new Vector3 (this.start.x + this.radius * Mathf.Cos (this.angle), this.start.y + this.radius * Mathf.Sin (this.angle), this.transform.position.z);

		this.transform.Rotate(new Vector3(0, 0, speed));
	}
}
