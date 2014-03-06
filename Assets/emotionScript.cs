using UnityEngine;
using System.Collections;

public class emotionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float a = this.renderer.material.color.a;
		this.renderer.material.color = new Color (this.renderer.material.color.r,
		                                          this.renderer.material.color.g,
		                                          this.renderer.material.color.b,
		                                          Mathf.Max (0, a-=Time.deltaTime*2));
		if (this.renderer.material.color.a == 0) {
			Destroy(this);
		}
	}
}
