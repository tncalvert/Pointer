using UnityEngine;
using System.Collections;



//using System;
public class BuildingGenerator : MonoBehaviour {

	/*
	 * Each building must fully occupy a square.
	 * Every building will have a base object that fills the square.
	 * Every building will then have some additional number of features
	 * that help to individualize the building
	 *
	 *
	 * Possible features could include...
	 * 
	 * TOPS
	 * 1. A dome top
	 * 2. An extended flat top
	 * 3. A vent box on the top with radio attena
	 * 
	 * SIDES
	 * 1. 
	 */


	/// <summary>
	/// The building block. This is what the building generator will use to form buildings
	/// </summary>
	public GameObject buildingBlock;

	public GameObject domeBlock;

	public GameObject streetLight;

	/// <summary>
	/// The floor. This is the Y position that the bottom of the building will be on
	/// </summary>
	public float floor = 0;

	public float minHeight = 2;

	public float heightVariance = .5f;

	public float lightChance = .5f;

	/// <summary>
	/// The side walk width ratio.
	/// </summary>
	public float sideWalkWidthRatio = .8f;

	public Material buildingMaterial;
	public Material streetMaterial;
	public Material sidewalkMaterial;
	public Material parkMaterial;

	/// <summary>
	/// The master scale controls the over all size of the building. It scales xyz
	/// </summary>
	private float _masterScale = 1.0f;


	private float squareSize = 8f;

	/// <summary>
	/// scale ratio for the height of sidewalks and parks
	/// </summary>
	private float heightRatio = .1f;
	// Use this for initialization

	//void Start () {
	//	this.generateBuilding ();
	//}




	/// <summary>
	/// Generates the street.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="roadType">Road type.</param>
	public Street generateStreet(Vector2 position, Street.ROADTYPE roadType, bool nextToPark){

		Street street = new Street (position, roadType, nextToPark);

		float dist = this.squareSize / 2;
		dist -= sideWalkWidthRatio * this._masterScale * .5f;

		float length = this.squareSize - 2f*this._masterScale;
		//length *= this._masterScale/this.sideWalkWidthRatio;
		length += sideWalkWidthRatio * this._masterScale * .5f;


		GameObject sidewalk;

		//create road
		GameObject road = this.generateBlock(0, 0, 0, this.squareSize * this._masterScale, heightRatio*.5f,  this.squareSize * this._masterScale);
		road.renderer.material = this.streetMaterial;
        road.layer = LayerMask.NameToLayer("Ground");
		street.addBlock (road);


		//create top left
		sidewalk = this.generateBlock(dist, 0, -dist, sideWalkWidthRatio, heightRatio, sideWalkWidthRatio);
		sidewalk.renderer.material = this.sidewalkMaterial;
        sidewalk.layer = LayerMask.NameToLayer("City");
		this.putOnFloor (sidewalk);
		street.addBlock (sidewalk);

		//create top right
		sidewalk = this.generateBlock (dist, 0,dist, sideWalkWidthRatio, heightRatio, sideWalkWidthRatio);
		sidewalk.renderer.material = this.sidewalkMaterial;
        sidewalk.layer = LayerMask.NameToLayer("City");
		this.putOnFloor (sidewalk);
		street.addBlock (sidewalk);

		//create bottam left
		sidewalk = this.generateBlock (-dist, 0, -dist, sideWalkWidthRatio, heightRatio, sideWalkWidthRatio);
		sidewalk.renderer.material = this.sidewalkMaterial;
        sidewalk.layer = LayerMask.NameToLayer("City");
		this.putOnFloor (sidewalk);
		street.addBlock (sidewalk);

		//create bottam right
		sidewalk = this.generateBlock(-dist, 0, dist,sideWalkWidthRatio,heightRatio,sideWalkWidthRatio);
		sidewalk.renderer.material = this.sidewalkMaterial;
        sidewalk.layer = LayerMask.NameToLayer("City");
		this.putOnFloor (sidewalk);
		street.addBlock (sidewalk);



		float lightTest = Random.value;
		bool tryLight = Random.value <= this.lightChance;
		//create  left
		if ((roadType & Street.ROADTYPE.LEFT) == Street.ROADTYPE.LEFT) {
			sidewalk = this.generateBlock(0, 0, -dist, length, heightRatio, sideWalkWidthRatio);
			sidewalk.renderer.material = this.sidewalkMaterial;
            sidewalk.layer = LayerMask.NameToLayer("City");
			this.putOnFloor (sidewalk);
			street.addBlock (sidewalk);

			if (tryLight && lightTest < .25f){
				GameObject streetLight = (GameObject) Instantiate (this.streetLight, new Vector3 (position.x, 0, position.y-dist), Quaternion.identity);
				streetLight.transform.position += new Vector3(0, 0, streetLight.transform.localScale.z*1.1f);
                streetLight.layer = LayerMask.NameToLayer("City");
				this.putOnBlock (streetLight, road);
				streetLight.transform.position += new Vector3(0,2,0);
			}

		}//create  right
		if ((roadType & Street.ROADTYPE.RIGHT) == Street.ROADTYPE.RIGHT) {
			sidewalk = this.generateBlock(0, 0, +dist, length, heightRatio, sideWalkWidthRatio);
			sidewalk.renderer.material = this.sidewalkMaterial;
            sidewalk.layer = LayerMask.NameToLayer("City");
			this.putOnFloor (sidewalk);
			street.addBlock (sidewalk);

			if (tryLight && lightTest > .25f && lightTest < .5f){
				GameObject streetLight = (GameObject) Instantiate (this.streetLight, new Vector3 (position.x, 0, position.y+dist), Quaternion.identity);
				streetLight.transform.position += new Vector3(0, 0, -streetLight.transform.localScale.z*1.1f);
				streetLight.transform.Rotate(new Vector3(0, 180, 0));
                streetLight.layer = LayerMask.NameToLayer("City");
				this.putOnBlock (streetLight, road);
				streetLight.transform.position += new Vector3(0,2,0);
			}
		}


		//create top 
		if ((roadType & Street.ROADTYPE.TOP) == Street.ROADTYPE.TOP) {
			sidewalk = this.generateBlock(dist, 0, 0, sideWalkWidthRatio, heightRatio, length);
			sidewalk.renderer.material = this.sidewalkMaterial;
            sidewalk.layer = LayerMask.NameToLayer("City");
			this.putOnFloor (sidewalk);
			street.addBlock (sidewalk);

			if (tryLight && lightTest > .5f && lightTest < .75f){
				GameObject streetLight = (GameObject) Instantiate (this.streetLight, new Vector3 (position.x+dist, 0, position.y), Quaternion.identity);
				streetLight.transform.position += new Vector3(-streetLight.transform.localScale.z*1.1f, 0, 0);
				streetLight.transform.Rotate(new Vector3(0, 270, 0));
                streetLight.layer = LayerMask.NameToLayer("City");
				this.putOnBlock (streetLight, road);
				streetLight.transform.position += new Vector3(0,2,0);
			}
		}

		//create bottam
		if ((roadType & Street.ROADTYPE.BOTTAM) == Street.ROADTYPE.BOTTAM) {
			sidewalk = this.generateBlock(-dist, 0, 0, sideWalkWidthRatio, heightRatio, length);
			sidewalk.renderer.material = this.sidewalkMaterial;
            sidewalk.layer = LayerMask.NameToLayer("City");
			this.putOnFloor (sidewalk);
			street.addBlock (sidewalk);

			if (tryLight && lightTest > .75f){
				GameObject streetLight = (GameObject) Instantiate (this.streetLight, new Vector3 (position.x-dist, 0, position.y), Quaternion.identity);
				streetLight.transform.position += new Vector3(streetLight.transform.localScale.z*1.1f, 0, 0);
				streetLight.transform.Rotate(new Vector3(0, 90, 0));
                streetLight.layer = LayerMask.NameToLayer("City");
				this.putOnBlock (streetLight, road);
				streetLight.transform.position += new Vector3(0,2,0);
			}
		}



		return street;
	
	}

	public Park generatePark(Vector2 position){
		GameObject park = this.generateBlock(0, 0, 0, this.squareSize * this._masterScale, heightRatio*.5f,  this.squareSize * this._masterScale);
		park.renderer.material = this.parkMaterial;
        park.layer = LayerMask.NameToLayer("Ground");
		Park p = new Park (position);
		p.addBlock (park);
		return p;
	}



	/// <summary>
	/// Generates the building.
	/// </summary>
	/// <returns>The building.</returns>
	public Building generateBuilding(){
		return this.generateBuilding (Vector2.zero);
	}

	/// <summary>
	/// Generates the building at some spot x,z
	/// </summary>
	/// <returns>The building.</returns>
	/// <param name="position">Position.</param>
	public Building generateBuilding(Vector2 position){
		Building building = new Building (position);

		GameObject squareBase = generateBlock (0, 0, 0, this.squareSize, 2, this.squareSize);
        squareBase.layer = LayerMask.NameToLayer("City");
		this.putOnFloor (squareBase);


		GameObject core = generateBlock (0, 0, 0, this.squareSize * .92f, this.minHeight, this.squareSize * .92f);
        core.layer = LayerMask.NameToLayer("City");
		this.putOnBlock (core, squareBase);

		building.addBlock (squareBase);
		building.addBlock (core);

		float heightPlus = (Random.value * this.heightVariance);

		float highest = 0;
		GameObject highestBlock = null;

		for (int i = 0; i < 2; i ++) {

			float y = this.minHeight +heightPlus + 2 + 6 *Random.value ;
			float x = (squareSize/2) + squareSize/4 * Random.value;
			float z = (squareSize/2) + squareSize/4 * Random.value;


			float xside = 1;
			if (Random.value > .5f){
				xside = -1;
			}
			float zside = 1;
			if (Random.value > .5f){
				zside = -1;
			}
			xside *= this._masterScale;
			zside *= this._masterScale;

			GameObject feature = generateBlock(xside*(Random.value*squareSize/7), 0, zside*(Random.value*squareSize/6),
			                                   x,
			                                   y,
			                                   z);
            feature.layer = LayerMask.NameToLayer("City");
			this.putOnBlock(feature, core);

			building.addBlock(feature);


			if (y > highest){
				highest = y;
				highestBlock = feature;
			}

		}

		float randomTop = Random.value;

		if (randomTop > .9f) {
			float domeScale = Mathf.Min (highestBlock.transform.localScale.x, highestBlock.transform.localScale.z) * .95f;
			GameObject dome = generateDome (highestBlock.transform.position.x - position.x, 0, highestBlock.transform.position.z - position.y, domeScale, domeScale, domeScale);
            dome.layer = LayerMask.NameToLayer("City");
			this.putOnBlock (dome, highestBlock);
			dome.transform.position -= Vector3.up * dome.transform.localScale.y / 2;

			building.addBlock (dome);
		}
		return building;

	}

	/// <summary>
	/// Puts the block on the floor.
	/// </summary>
	/// <param name="block">Block.</param>
	private void putOnFloor(GameObject block){
		block.transform.position = new Vector3 (block.transform.position.x, this.floor + block.transform.localScale.y / 2, block.transform.position.z);
	}

	/// <summary>
	/// Puts the on block ontop of the other block
	/// </summary>
	/// <param name="block">Block.</param>
	/// <param name="baseBlock>The block whose ceiling will become the floor</para>"> 
	private void putOnBlock(GameObject block, GameObject baseBlock){
		block.transform.position = new Vector3 (block.transform.position.x,baseBlock.transform.position.y + (block.transform.localScale.y / 2) + (baseBlock.transform.localScale.y/2), block.transform.position.z);
	}


	/// <summary>
	/// Generates a block with given position. The dimension will be the identity
	/// </summary>
	/// <returns>The block.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public GameObject generateBlock(float x, float y, float z){
		return this.generateBlock (x, y, z, 1, 1, 1);
	}

	/// <summary>
	/// Generate a block with given position and dimension
	/// </summary>
	/// <returns>The block.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="sx">The x scale</param>
	/// <param name="sy">The y scale</param>
	/// <param name="sz">The z scale</param>
	public GameObject generateBlock(float x, float y, float z, float sx, float sy, float sz){
		GameObject block = (GameObject) Instantiate (this.buildingBlock, new Vector3 (x, y, z), Quaternion.identity);
		block.renderer.material = this.buildingMaterial;
		block.transform.localScale = this._masterScale * new Vector3 (sx, sy, sz);
		return block;
	}


	public GameObject generateDome(float x, float y, float z, float sx, float sy, float sz){
		GameObject block = (GameObject) Instantiate (this.domeBlock, new Vector3 (x, y, z), Quaternion.identity);
		//block.renderer.material = this.buildingMaterial;
		block.transform.localScale = this._masterScale * new Vector3 (sx, sy, sz);
		return block;
	}


	// Not needed at the moment
	// Update is called once per frame
	//void Update () {
	
	//}
}
