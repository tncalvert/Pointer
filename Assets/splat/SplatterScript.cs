using UnityEngine;
using System.Collections;

public class SplatterScript : MonoBehaviour {

	public SplatScript bloodSplat;
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


				SplatScript splat = (SplatScript)Instantiate(this.bloodSplat, hit.point+(.001f * hit.normal) , Quaternion.identity);
				splat.normal = hit.normal;
				Vector3 direction = splat.transform.position - hit.normal;
				splat.transform.LookAt(direction);
				splat.transform.localScale*= Mathf.Clamp(minScale + Random.value*(maxScale-minScale), minScale, maxScale);
				//splat.transform.Rotate(splat.transform.forward, Random.value*300);

				splat.transform.RotateAround(splat.transform.position, splat.transform.forward, Random.value*360);

			}
		}

	}



	public void makeSplat(Vector3 position, Vector3 direction, float distance, LayerMask layer){


		RaycastHit ray = new RaycastHit ();

		if (Physics.Raycast (position, direction, out ray, distance, layer)) {
			SplatScript splat = (SplatScript)Instantiate(this.bloodSplat, ray.point+(.001f * ray.normal) , Quaternion.identity);
			Vector3 splatDirection = splat.transform.position- ray.normal;
			splat.transform.LookAt(splatDirection);
			splat.transform.localScale = new Vector3(1,1,1)*100;
			//splat.transform.localScale*=Mathf.Clamp(Random.value*this.maxScale, this.minScale, this.maxScale);
			//splat.transform.Rotate(splat.transform.forward, Random.value*300);
		}

	}

}
