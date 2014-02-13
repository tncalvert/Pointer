using UnityEngine;
using System.Collections;

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
	
	/// <summary>
	/// The floor. This is the Y position that the bottom of the building will be on
	/// </summary>
	public float floor = 0;

	public float minHeight = 2;

	public float heightVariance = .5f;


	/// <summary>
	/// The master scale controls the over all size of the building. It scales xyz
	/// </summary>
	private float _masterScale = 1.0f;


	private float squareSize = 8f;

	// Use this for initialization

	//void Start () {
	//	this.generateBuilding ();
	//}

	public Building generateBuilding(){
		return this.generateBuilding (Vector2.zero);
	}
	/// <summary>
	/// Create a building that will fully occupy one game cell
	/// </summary>
	public Building generateBuilding(Vector2 position){
		Building building = new Building (position);

		GameObject squareBase = generateBlock (0, 0, 0, this.squareSize, 2, this.squareSize);
		this.putOnFloor (squareBase);


		GameObject core = generateBlock (0, 0, 0, this.squareSize * .92f, this.minHeight, this.squareSize * .92f);
		this.putOnBlock (core, squareBase);

		building.addBlock (squareBase);
		building.addBlock (core);

		float heightPlus = (Random.value * this.heightVariance);
		for (int i = 0; i < 6; i ++) {

			float y = this.minHeight + heightPlus + 6 + 4 *Random.value ;
			float x = 1 + 3 * Random.value;
			float z = 1 + 3 * Random.value;

			//x *= this._masterScale;
			//y *= this._masterScale;
			//z *= this._masterScale;

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

			GameObject feature = generateBlock(x/2 * xside, 0, z/2 * zside,
			                                   x,
			                                   y,
			                                   z);
			this.putOnBlock(feature, squareBase);

			building.addBlock(feature);

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
		block.transform.position = new Vector3 (block.transform.position.x, (block.transform.localScale.y / 2) + (baseBlock.transform.localScale.y/2), block.transform.position.z);
	}


	/// <summary>
	/// Generates a block with given position. The dimension will be the identity
	/// </summary>
	/// <returns>The block.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private GameObject generateBlock(float x, float y, float z){
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
	private GameObject generateBlock(float x, float y, float z, float sx, float sy, float sz){
		GameObject block = (GameObject) Instantiate (this.buildingBlock, new Vector3 (x, y, z), Quaternion.identity);
		block.transform.localScale = this._masterScale * new Vector3 (sx, sy, sz);
		return block;
	}



	// Not needed at the moment
	// Update is called once per frame
	//void Update () {
	
	//}
}
