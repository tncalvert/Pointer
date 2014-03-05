using UnityEngine;
using System.Collections;

public class SplatterScript : MonoBehaviour {

	public GameObject bloodSplat;
	public float minScale = .5f;
	public float maxScale = 4f;

	public void makeBigSplat(Vector3 position, Vector3 direction, float radius, int count, LayerMask layer){
	
		for (int i = 0; i < count; i ++) {
			makeSplat (position, direction + Random.onUnitSphere, radius, layer);
		}
	}

	public void makeSplat(Vector3 position, Vector3 direction, float distance, LayerMask layer){

		RaycastHit ray = new RaycastHit ();

		if (Physics.Raycast (position, direction, out ray, layer)) {
           
			GameObject splat = (GameObject)Instantiate(this.bloodSplat, ray.point+(.001f * ray.normal) , Quaternion.identity);
			Vector3 splatDirection = splat.transform.position - ray.normal;
			splat.transform.LookAt(splatDirection);
            splat.transform.localScale = new Vector3(1, 1, 1) * 3;
			splat.transform.Rotate(transform.forward, Random.value*300);
			//splat.transform.RotateAround(this.transform.position,this.transform.up,Random.value*300);
		}

	}

}
