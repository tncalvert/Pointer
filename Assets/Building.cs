using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building {

	private List<GameObject> blocks;

	private Vector3 position;

	public Building(Vector2 position){
		this.blocks = new List<GameObject>();
		this.position = new Vector3 (position.x, 0, position.y);
	}

	public void addBlock(GameObject block){
		this.blocks.Add (block);
		block.transform.position += this.position;

	}

}
