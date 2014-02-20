using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Park {

	

	
	private List<GameObject> blocks;
	
	private Vector3 position;
	public Vector2 Position { get{ return new Vector2(position.x, position.z); } }
	
	

	
	public Park(Vector2 position){
		this.blocks = new List<GameObject>();
		this.position = new Vector3 (position.x, 0, position.y);

	}
	
	public void addBlock(GameObject block){
		this.blocks.Add (block);
		block.transform.position += this.position;
		
	}

	
	
}
