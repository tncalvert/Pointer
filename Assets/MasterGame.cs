using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator buildingGenerator;
	public CityGenerator cityGenerator;
	public PathFinder pathFinder;




	//Uhm, I dont know what I'm doing anymore
	public GameObject follower;

	public GameObject victim;

	public int width = 16;
	public int height = 16;

	/// <summary>
	/// All of the streets in the city
	/// </summary>
	private List<Street> streets;

	/// <summary>
	/// All of the buildings in the city
	/// </summary>
	private List<Building> buildings;




	// Use this for initialization
	void Start () {

		//init
		this.streets = new List<Street> ();
		this.buildings = new List<Building> ();

		//generate city data
		CityGenerator.FILLTYPE[,] gridCity = this.cityGenerator.buildCity (width, height);

		//generate visual city. buildings, streets, parks
		for (int x = 0; x < width; x ++) {
			for (int y = 0 ; y < height ; y ++){
				Vector2 position = new Vector2(y * 8, x * 8);

				//fill each cell with something 
				switch (gridCity[x,y]){
					case CityGenerator.FILLTYPE.BULIDING:
						this.buildings.Add (buildingGenerator.generateBuilding(position));
						break;
					case CityGenerator.FILLTYPE.ROAD:
						Street.ROADTYPE roadType = Street.ROADTYPE.NONE;
						
						
						if (y +1 < height && gridCity[x, y+1] != CityGenerator.FILLTYPE.ROAD){
							roadType |= Street.ROADTYPE.TOP;
						}
						
						if (x - 1 > -1 && gridCity[x-1, y] != CityGenerator.FILLTYPE.ROAD){
							roadType |= Street.ROADTYPE.LEFT;
						}
						if (x + 1 < width && gridCity[x+1, y] != CityGenerator.FILLTYPE.ROAD){
							roadType |= Street.ROADTYPE.RIGHT;
						}
						
						if ( y -1 > -1 && gridCity[x, y-1] != CityGenerator.FILLTYPE.ROAD){
							roadType |= Street.ROADTYPE.BOTTAM;
						}
						
						this.streets.Add (buildingGenerator.generateStreet(position, roadType ));
						break;
					case CityGenerator.FILLTYPE.PARK:

						// TODO make a park class and have generate park return it.
						buildingGenerator.generatePark(position);
						break;
				}
			
			}
		}


		//generate path graph
		this.pathFinder.buildPathGraph (this.buildings, this.streets);

		generateVictim (streets [0].Position);

		//pathFinder.getPath (this.streets [0].Position, this.streets [this.streets.Count - 1].Position);
		//for (int i = 0 ; i < 4 ; i ++)
		//	this.generateFollower (this.streets [0].Position);

	}

	private VictimSteering generateVictim(Vector2 position){
		VictimSteering v = ((GameObject)Instantiate(this.victim, new Vector3(position.x, 1, position.y), Quaternion.identity)).GetComponent<VictimSteering>();
		return v;
	}

	//I am teh unity n00b and I don't know if this is a good way to go about generating people
	private PlayerSteering generateFollower(Vector2 position){
		PlayerSteering f = ((GameObject)Instantiate (this.follower, new Vector3(position.x, 1, position.y), Quaternion.identity)).GetComponent<PlayerSteering>();
		return f;
	}

	// Update is called once per frame
	void Update () {
	

	}
}
