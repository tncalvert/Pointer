using UnityEngine;
using System.Collections;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator buildingGenerator;
	public CityGenerator cityGenerator;

	public int width = 16;
	public int height = 16;

	// Use this for initialization
	void Start () {

		//generate city data
		CityGenerator.FILLTYPE[,] gridCity = this.cityGenerator.buildCity (width, height);

		//generate visual city. buildings, streets, parks
		for (int x = 0; x < width; x ++) {
			for (int y = 0 ; y < height ; y ++){
				Vector2 position = new Vector2(y * 8, x * 8);

				//fill each cell with something 
				switch (gridCity[x,y]){
					case CityGenerator.FILLTYPE.BULIDING:
						buildingGenerator.generateBuilding(position);
						break;
					case CityGenerator.FILLTYPE.ROAD:
						BuildingGenerator.ROADTYPE roadType = BuildingGenerator.ROADTYPE.NONE;
						
						
						if (y +1 < height && gridCity[x, y+1] != CityGenerator.FILLTYPE.ROAD){
							roadType |= BuildingGenerator.ROADTYPE.TOP;
						}
						
						if (x - 1 > -1 && gridCity[x-1, y] != CityGenerator.FILLTYPE.ROAD){
							roadType |= BuildingGenerator.ROADTYPE.LEFT;
						}
						if (x + 1 < width && gridCity[x+1, y] != CityGenerator.FILLTYPE.ROAD){
							roadType |= BuildingGenerator.ROADTYPE.RIGHT;
						}
						
						if ( y -1 > -1 && gridCity[x, y-1] != CityGenerator.FILLTYPE.ROAD){
							roadType |= BuildingGenerator.ROADTYPE.BOTTAM;
						}
						
						buildingGenerator.generateStreet(position, roadType );
						break;
					case CityGenerator.FILLTYPE.PARK:
						buildingGenerator.generatePark(position);
						break;
				}


		
			}
		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
