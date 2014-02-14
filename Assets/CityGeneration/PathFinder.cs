using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {

	private const bool DEBUG = true;

	private class WayPoint {
		
		
		private Vector2 position;
		public Vector2 Position { get{ return this.position; } } 
		
		public WayPoint(Vector2 position){
			this.position = position;
		}
		
		public void addChild(WayPoint child){
		
		}

		public string ToString(){
			return "WAYPOINT{" + this.position.x + "," + this.position.y+"}";
		}
		
	}


	/// <summary>
	/// The way points.
	/// </summary>
	private List<WayPoint> wayPoints;

	public PathFinder(){

		this.wayPoints = new List<WayPoint> ();

		

	}

	/// <summary>
	/// Add a waypoint to the pathfinder. 
	/// each new waypoint will check what other waypoints it can see. It will 
	/// </summary>
	/// <param name="wayPoint">Way point.</param>
	public void addWaypoint(Vector2 waypointPosition){

		//create a new waypoint
		WayPoint wayPoint = new WayPoint (waypointPosition);


		Vector3 origin = new Vector3 (waypointPosition.x, 1, waypointPosition.y);
		if (DEBUG) {
			Debug.DrawLine (origin, origin +new Vector3(1,0,1), Color.blue, 10000f, false);
		}
	
		//determine what waypoints the new waypoint can see
		foreach (WayPoint otherWayPoint in this.wayPoints) {

			//math n stuff
			Vector3 direction = (new Vector3(otherWayPoint.Position.x - origin.x, 0, otherWayPoint.Position.y - origin.z));
			float distance = direction.magnitude;
			direction.Normalize();

			bool canSee = !Physics.Raycast(origin, direction, distance);

			if (canSee){

				//new waypoint can see another waypoint, which means the other waypoint can see the new waypoint. Reflect changes in both waypoints
				wayPoint.addChild(otherWayPoint);
				otherWayPoint.addChild (wayPoint);

				if (DEBUG){
					Debug.DrawLine(origin, origin + distance * direction, Color.red, 10000f, false);
				}
			}

		}


		this.wayPoints.Add (wayPoint);
	}




}
