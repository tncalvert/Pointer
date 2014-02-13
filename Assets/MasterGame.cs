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
				Vector2 position = new Vector2(y * 8, x * 8);
				if (gridCity[x,y] == CityGenerator.FILLTYPE.BULIDING){
					buildingGenerator.generateBuilding(position);
				} else if (gridCity[x,y] == CityGenerator.FILLTYPE.ROAD){

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
				}
				else if (gridCity[x,y] == CityGenerator.FILLTYPE.PARK){
					buildingGenerator.generatePark(position);
				}

		//		buildingGenerator.generateBuilding(new Vector2(y * 16, x * 16));
			}
		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
