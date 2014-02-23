using UnityEngine;
using System.Collections;

public class SplatScript : MonoBehaviour {

	public Vector3 normal;

	public float sludgeAmount = .2f;
	public float sludgeRate = .01f;
	private float sludgeLeft;

	private float timeLeft  = 0;

	public SplatScript(){
		this.sludgeLeft = this.sludgeAmount;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		timeLeft -= Time.deltaTime;
		while (this.sludgeLeft > 0 && timeLeft<=0) {
			timeLeft = .1f;

			Vector3 d = Vector3.Cross (normal, Vector3.down);
			d = Quaternion.AngleAxis (-90, normal) * d;

			transform.position += d.normalized * this.sludgeRate;

			this.sludgeLeft -= this.sludgeRate;
		}
		//Debug.DrawLine (transform.position, transform.position + (normal * 10), Color.red);
	}
}
