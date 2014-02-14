using UnityEngine;
using System.Collections;

public class WayPoint {


	private Vector2 position;
	public Vector2 Position { get{ return this.position; } } 

	public WayPoint(Vector2 position){
		this.position = position;
	}



}
