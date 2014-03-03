using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterGame : MonoBehaviour {

	public BuildingGenerator buildingGenerator;
	public CityGenerator cityGenerator;
	public PathFinder pathFinder;
    public GeneticMaster geneticMaster;

	//Uhm, I dont know what I'm doing anymore
	public GameObject follower;

	public int width = 16;
	public int height = 16;

	public int gunShopCount = 1;
	public int hotelCount = 2;

	//the list of all hotels in the map;
	private List<Building> hotels;

	/// <summary>
	/// All of the streets in the city
	/// </summary>
	private List<Street> streets;

	/// <summary>
	/// All of the buildings in the city
	/// </summary>
	private List<Building> buildings;

	/// <summary>
	/// all of the parks in the city
	/// </summary>
	private List<Park> parks;

	//an instance of the follower
	//private PlayerSteering f;

    public GameObject victim;



	private CityGenerator.FILLTYPE[,] gridCity;
	private float size;

	// Use this for initialization
	void Start () {

		//init
		this.streets = new List<Street> ();
		this.buildings = new List<Building> ();
		this.parks = new List<Park> ();
		this.hotels = new List<Building> ();

		//generate city data
		gridCity = this.cityGenerator.buildCity (width, height);
		size = 8;
		//generate visual city. buildings, streets, parks
		for (int x = 0; x < width; x ++) {
			for (int y = 0 ; y < height ; y ++){
				Vector2 position = new Vector2(y * size, x * size);

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
						
					bool nextToPark = ((y +1 < height && gridCity[x, y+1] == CityGenerator.FILLTYPE.PARK)
					    				||(x - 1 > -1 && gridCity[x-1, y] == CityGenerator.FILLTYPE.PARK)
					    				||(x + 1 < width && gridCity[x+1, y] == CityGenerator.FILLTYPE.PARK)
					                   	||( y -1 > -1 && gridCity[x, y-1] == CityGenerator.FILLTYPE.PARK));

					    this.streets.Add (buildingGenerator.generateStreet(position, roadType, nextToPark ));
						break;
					case CityGenerator.FILLTYPE.PARK:

						// TODO make a park class and have generate park return it.
						this.parks.Add (buildingGenerator.generatePark(position));
						break;
				}
			
			}
		}


		//place gunShops
		this.hotels = this.placeBuildingType (this.gunShopCount, Building.BUILDINGTYPE.GUNSHOP);

		//place hotels
		this.placeBuildingType (this.hotelCount, Building.BUILDINGTYPE.HOTEL);





		//generate path graph
		this.pathFinder.buildPathGraph (this.buildings, this.streets, this.parks);



		//pathFinder.getPath (this.streets [0].Position, this.streets [this.streets.Count - 1].Position);
	
		this.generateFollower (this.streets [0].Position);

        List<Vector2> victimPositions = new List<Vector2>();
        for (int i = 2; i < 3/*streets.Count/2*/; i++)
            victimPositions.Add(streets[i].Position);

        geneticMaster.initialPositions = victimPositions;
	}

	//I am teh unity n00b and I don't know if this is a good way to go about generating people
	private PlayerSteering generateFollower(Vector2 position){
		PlayerSteering f = ((GameObject)Instantiate (this.follower, new Vector3(position.x, 1, position.y), Quaternion.identity)).GetComponent<PlayerSteering>();
		return f;
	}


	private List<Building> placeBuildingType(int count, Building.BUILDINGTYPE kind){
		List<Building> selectedBuildings = new List<Building> ();
		for (int i = 0; i < count; i ++) {
			bool selected = false;
			while (!selected){
				Building b = this.buildings[Random.Range(0,this.buildings.Count)];
				int x = (int)(b.Position.x/size);
				int y = (int)(b.Position.y/size);
				if (this.isNextTo(y,x,CityGenerator.FILLTYPE.ROAD) && b.BuildingType == Building.BUILDINGTYPE.NONE){
					selected = true;
					b.BuildingType = kind;
					selectedBuildings.Add (b);
				}
			}
		}
		return selectedBuildings;
	}

	private bool isNextTo(int x, int y, CityGenerator.FILLTYPE type){
		bool result = false;
		if (x - 1 > -1) {
			result |= gridCity[x-1, y] == type;
		} 
		if (x + 1 < width) {
			result |= gridCity[x+1, y] == type;
		}
		if (y - 1 > -1) {
			result |= gridCity[x, y-1] == type;
		}
		if (y + 1 < height) {
			result |= gridCity[x, y+1] == type;
		}
		return result;
	}

	// Update is called once per frame
	void Update () {
	

	}

    public List<Street> getStreets() {
        return streets;
    }
}
