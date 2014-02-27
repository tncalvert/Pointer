using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building {

	public enum BUILDINGTYPE
	{
		NONE,
		GUNSHOP,
		HOTEL
	}


	private List<GameObject> blocks;

	private Vector2 position;

	public Vector2 Position{ get {return this.position;}}

	private BUILDINGTYPE buildingType;
	public BUILDINGTYPE BuildingType { get{ return this.buildingType; } set{this.buildingType = value;this.adjustColor();} }

	public Building(Vector2 position){
		this.blocks = new List<GameObject>();
		this.position = position;
		BuildingType = BUILDINGTYPE.NONE;
	}

	public void addBlock(GameObject block){
		this.blocks.Add (block);
		block.transform.position += new Vector3(this.position.x, 0, this.position.y);

	}

	private void adjustColor(){
		if (BuildingType == BUILDINGTYPE.GUNSHOP) {
			foreach (GameObject block in this.blocks) {
				block.renderer.material.color = Color.red;

				Inspectible insp = (Inspectible)block.AddComponent("Inspectible");
				//insp.text = "GUNSHOP";

				insp.getTextFunc = () => {
					List<string> lines = new List<string>();
					lines.Add ("GUN SHOP");
					lines.Add ("INFO");
					return lines;
				};

			}
		} else if (BuildingType == BUILDINGTYPE.HOTEL) {
			foreach (GameObject block in this.blocks) {
				block.renderer.material.color = Color.blue;
				Inspectible insp = (Inspectible)block.AddComponent("Inspectible");
				insp.getTextFunc = () => {
					List<string> lines = new List<string>();
					lines.Add ("HOTEL");
					lines.Add ("INFO");
					return lines;
				};
			}

		}

	}

}
