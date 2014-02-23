using UnityEngine;
using System.Collections;

public class SplatterScript : MonoBehaviour {

	public GameObject bloodSplat;
	public float minScale = .5f;
	public float maxScale = 4f;
	// Use this for initialization
	void Start () {
		if (bloodSplat == null) {
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (bloodSplat == null) {
			return;
		}

		if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) {
			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(cameraRay, out hit)) {



				GameObject splat = (GameObject)Instantiate(this.bloodSplat, hit.point+(.001f * hit.normal) , Quaternion.identity);
				Vector3 direction = splat.transform.position - hit.normal;
				splat.transform.LookAt(direction);
				splat.transform.localScale*= Mathf.Clamp(Random.value, .5f, 1f);
				//splat.transform.Rotate(splat.transform.forward, Random.value*300);


			}
		}

	}



	public void makeSplat(Vector3 position, Vector3 direction, float distance, LayerMask layer){


		RaycastHit ray = new RaycastHit ();

		if (Physics.Raycast (position, direction, out ray, distance, layer)) {
			GameObject splat = (GameObject)Instantiate(this.bloodSplat, ray.point+(.001f * ray.normal) , Quaternion.identity);
			Vector3 splatDirection = splat.transform.position- ray.normal;
			splat.transform.LookAt(splatDirection);

			splat.transform.localScale*=Mathf.Clamp(Random.value*this.maxScale, this.minScale, this.maxScale);
			//splat.transform.Rotate(splat.transform.forward, Random.value*300);
		}

	}

}
