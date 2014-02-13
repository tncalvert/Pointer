using UnityEngine;
using System.Collections;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator generator;


	// Use this for initialization
	void Start () {

		for (int x = 0; x < 4; x ++) {

			for (int y = 0 ; y < 4 ; y ++){
				generator.generateBuilding(new Vector2(y * 16, x * 16));
			}
		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
