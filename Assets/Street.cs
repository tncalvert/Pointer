using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Street {


	/*
	 * An enum to keep track of what kind of road a road piece is
	 * X | X X X | X
	 * _   _____   _
	 * X  |     |  X
	 * X  |     |  X
	 * X  |_____|  X
	 * _           _
	 * X | X X X | X
	 * 
	 */
	[System.Flags]
	public enum ROADTYPE{
		NONE = 0x00, 
		TOP = 0x01, 
		
		LEFT = 0x02, 
		RIGHT = 0x04,
		
		BOTTAM = 0x08
		
		
	}

	/* TODO
	 * instead of just adding blocks, have unique methods to add sidewalk, street, and street light objects. 
	 */

	private List<GameObject> blocks;
	
	private Vector3 position;
	public Vector2 Position { get{ return new Vector2(position.x, position.z); } }

	private bool nextToPark;
	public bool NextToPark{ get { return this.nextToPark; } }

	private ROADTYPE roadType;
	
	public Street(Vector2 position, ROADTYPE roadType, bool nextToPark){
		this.blocks = new List<GameObject>();
		this.position = new Vector3 (position.x, 0, position.y);
		this.roadType = roadType;
		this.nextToPark = nextToPark;
	}
	
	public void addBlock(GameObject block){
		this.blocks.Add (block);
		block.transform.position += this.position;
		
	}

	/// <summary>
	/// determines if the street segment is just going vertically
	/// </summary>
	/// <returns><c>true</c>, if street is up and down, <c>false</c> otherwise.</returns>
	public bool isUpDown(){
		return ((ROADTYPE.TOP & this.roadType) == ROADTYPE.TOP)
				&& ((ROADTYPE.BOTTAM & this.roadType) == ROADTYPE.BOTTAM)
				&& !((ROADTYPE.RIGHT & this.roadType) == ROADTYPE.RIGHT)
				&& !((ROADTYPE.LEFT & this.roadType) == ROADTYPE.LEFT);
	}

	/// <summary>
	/// determines if the street segment is just going horizontally
	/// </summary>
	/// <returns><c>true</c>, if street is right and left, <c>false</c> otherwise.</returns>
	public bool isRightLeft(){
		return !((ROADTYPE.TOP & this.roadType) == ROADTYPE.TOP)
			&& !((ROADTYPE.BOTTAM & this.roadType) == ROADTYPE.BOTTAM)
				&& ((ROADTYPE.RIGHT & this.roadType) == ROADTYPE.RIGHT)
				&& ((ROADTYPE.LEFT & this.roadType) == ROADTYPE.LEFT);
	}

	
}
