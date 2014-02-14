using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator buildingGenerator;
	public CityGenerator cityGenerator;
	public PathFinder pathFinder;


	//Uhm, I dont know what I'm doing anymore
	public FollowPath follower;

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


	//an instance of the follower
	private FollowPath f;

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



		//pathFinder.getPath (this.streets [0].Position, this.streets [this.streets.Count - 1].Position);
	
		this.f = this.generateFollower (this.streets [0].Position);
	}

	//I am teh unity n00b and I don't know if this is a good way to go about generating people
	private FollowPath generateFollower(Vector2 position){
		FollowPath f = (FollowPath) Instantiate (this.follower, new Vector3(position.x, 1, position.y), Quaternion.identity);
		f.pathFinder = this.pathFinder;
		return f;
	}

	// Update is called once per frame
	void Update () {
	
		//as it stands right now, this whole thing is just to test out A*. Right click to direct the simple capsuleman

		//simple mouse selection stuff

		if (Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown(0)) {
			Ray cameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		

			RaycastHit hit;//=new RaycastHit();
		
			if (Physics.Raycast(cameraRay, out hit, 1000)){

				Vector2 start = new Vector2(this.f.transform.position.x, this.f.transform.position.z);
				Vector2 end = new Vector2(hit.point.x, hit.point.z);

				this.f.setPath(this.pathFinder.getPath(start, end));
				
			}
		}

	}
}
