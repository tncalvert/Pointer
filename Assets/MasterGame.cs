using UnityEngine;
using System.Collections;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator buildingGenerator;
	public CityGenerator cityGenerator;

	public int width = 16;
	public int height = 16;

	// Use this for initialization
	void Start () {


		CityGenerator.FILLTYPE[,] gridCity = this.cityGenerator.buildCity (width, height);

		for (int x = 0; x < width; x ++) {

			for (int y = 0 ; y < height ; y ++){

				if (gridCity[x,y] == CityGenerator.FILLTYPE.BULIDING){
					buildingGenerator.generateBuilding(new Vector2(y * 8, x * 8));
				}

		//		buildingGenerator.generateBuilding(new Vector2(y * 16, x * 16));
			}
		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
