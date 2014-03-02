using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {

	private const bool DEBUG = true;

// Disable warning for not implementing GetHashCode()
#pragma warning disable 0659

    private class WayPoint {
			
		private Vector2 position;
		public Vector2 Position { get{ return this.position; } } 

		private Dictionary<WayPoint, float> childrenCostTable;
		private List<WayPoint> children;

		public WayPoint(Vector2 position){
			this.position = position;

			this.children = new List<WayPoint>();
			this.childrenCostTable = new Dictionary<WayPoint, float>();
		}
		
		public void addChild(WayPoint child, float cost){
			this.children.Add (child);
			this.childrenCostTable [child] = cost;
		}

		/// <summary>
		/// Get the manhatten distance between two waypoints
		/// </summary>
		/// <param name="other">Other.</param>
		public float heuristicTo(WayPoint other){

			return this.position.x - other.position.x + this.position.y - other.position.y;
		}

		/// <summary>
		/// Get the estimated cost to the other waypoint. If the other waypoint is a child of this waypoint, then some cost will be returned. Otherwise, +Infinity will be returned
		/// </summary>
		/// <param name="other">Other.</param>
		public float costTo(WayPoint other){
			if (this.children.Contains(other)){
				return this.childrenCostTable[other];
			} else {
				return float.PositiveInfinity;
			}
		}

		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <returns>The children.</returns>
		public List<WayPoint> getChildren(){
			return this.children;
		}


        public override bool Equals(object obj) {
            try {
                WayPoint w = (WayPoint)obj;
                return this.Position.Equals(w.Position);
            } catch /*InvalidCastException most likely */ {
                // Either way they probably aren't equal
                return false;
            }
        }

		public override string ToString(){
			return "WAYPOINT{" + this.position.x + "," + this.position.y+"}";
		}
	
	}

#pragma warning restore 0659


    /// <summary>
	/// The way points.
	/// </summary>
	private List<WayPoint> wayPoints;

	/// <summary>
	/// The hotel locations.
	/// </summary>
	private List<Vector2> hotelLocations;


	/// <summary>
	/// The gun shop locations.
	/// </summary>
	private List<Vector2> gunShopLocations;

	public List<Vector2> HotelLocations { get { return this.hotelLocations; } }
	public List<Vector2> GunShopLocations { get { return this.gunShopLocations; } }


	public PathFinder(){

		this.wayPoints = new List<WayPoint> ();
		this.hotelLocations = new List<Vector2> ();
        this.gunShopLocations = new List<Vector2>();

	}


	/// <summary>
	/// Gets the path from start to end in terms of the waypoints. 
	/// </summary>
	/// <returns>The path as an array of vector2. The last element of the path
	/// will always be 'end' </returns>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public Vector2[] getPath(Vector2 start, Vector2 end){

		//if the start can see the end, don't bother doing any pathfinding
		if (canSee (start, end)) {
			return new Vector2[]{end};
		}

		/* A* pathfinding
		 * 
		 * Find the waypoints that mark the start and end of the movement. 
		 *
		 */

		//find the closest node to the start and end. 
		//TODO right now, it is assumed that start is a valid position. If start is given as an invalid position, then this method may not produce the correct waypoint
		//float firstSqrDist = float.MaxValue;
		float lastSqrDist = float.MaxValue;

		WayPoint temp = new WayPoint (start);

		//WayPoint first = null;
		WayPoint last = null;
		foreach (WayPoint waypoint in this.wayPoints) {

			if (this.canSee(waypoint.Position, temp.Position)){
				temp.addChild(waypoint, (waypoint.Position - temp.Position).magnitude);
			}

			float waypointSqrDist = (waypoint.Position - start).sqrMagnitude;
			//if ( first == null || waypointSqrDist < firstSqrDist ){
			//	firstSqrDist = waypointSqrDist;
		//		first = waypoint;
		//	}
			waypointSqrDist = (waypoint.Position - end).sqrMagnitude;
			if ( last == null || waypointSqrDist < lastSqrDist ){
				lastSqrDist = waypointSqrDist;
				last = waypoint;
			}
		}


		//run A*
		List<WayPoint> waypoints = doAStar (temp, last);

		//clean the waypoint list. There is a chance that the last waypoint has the path move past the end
		if (waypoints.Count > 1 
		    && this.canSee (end, waypoints [waypoints.Count - 1].Position)
		    && this.canSee (end, waypoints [waypoints.Count - 2].Position)) {
			waypoints.Remove(waypoints[waypoints.Count-1]);
		}

        // Remove the starting point as we are already there
        if (waypoints.Count > 0) {
            waypoints.RemoveAt(0);
        }

		//assemble information to return
		Vector2[] path = new Vector2[waypoints.Count + 1];
		for (int i = 0; i < waypoints.Count; i ++) {
			path[i] = waypoints[i].Position;
		}
		path [path.Length - 1] = end;



		//draw path
		Vector3 origin = new Vector3 (start.x, 1, start.y);
		foreach (Vector2 p in path) {
			Vector3 next = new Vector3(p.x, 1, p.y);
			Debug.DrawLine(origin, next,Color.green,2f, false);
			origin = next;
		}


		return path;
	}


	//adapted from the wikipedia on A*
	private List<WayPoint> doAStar(WayPoint start, WayPoint goal){
	
		List<WayPoint> closedSet = new List<WayPoint> ();
		List<WayPoint> openSet = new List<WayPoint> ();
		openSet.Add (start);

		Dictionary<WayPoint, WayPoint> cameFrom = new Dictionary<WayPoint, WayPoint> ();
		Dictionary<WayPoint, float> gScore = new Dictionary<WayPoint, float> ();
		Dictionary<WayPoint, float> fScore = new Dictionary<WayPoint, float> ();

		gScore [start] = 0;
		fScore [start] = gScore [start] + start.heuristicTo (goal);

		//something fishy is going on
		while (openSet.Count > 0) {
			WayPoint current = null;//get the lowest value from openset
			foreach(WayPoint currentCandidate in openSet){
				if (current == null || fScore[currentCandidate] < fScore[current]){
					current = currentCandidate;
				}
			}
	
			
			if (current == goal){
				return reconstructPath(cameFrom, goal);
			}

			openSet.Remove (current);
			closedSet.Add (current);
			foreach (WayPoint waypoint in current.getChildren()){
				if (closedSet.Contains(waypoint)){
					continue;
				}

				float gTentative = gScore[current] + current.costTo(waypoint);
				if (!openSet.Contains(waypoint) || gTentative < gScore[waypoint]){ // < gScore[waypoint] 
					cameFrom[waypoint] = current;
					gScore[waypoint]  = gTentative;
					fScore[waypoint] = gScore[waypoint] + waypoint.heuristicTo(goal);
					if (!openSet.Contains(waypoint)){
						openSet.Add (waypoint);
					}
				}
			}
		}

		//return a failure :(
		return new List<WayPoint> ();

	}

	//adapted from wikipedia on A*
	private List<WayPoint> reconstructPath(Dictionary<WayPoint, WayPoint> cameFrom, WayPoint currentNode){

		if (cameFrom.ContainsKey (currentNode)) {
			List<WayPoint> p = reconstructPath(cameFrom, cameFrom[currentNode]);
			p.Add(currentNode);
			return p;
		} else{
			List<WayPoint> wpl = new List<WayPoint> ();
			wpl.Add (currentNode);
			return wpl;
		}
	}

	/// <summary>
	/// Builds the path graph. 
	/// </summary>
	/// <param name="buildings">Buildings.</param>
	/// <param name="streets">Streets.</param>
	public void buildPathGraph(List<Building> buildings, List<Street> streets, List<Park> parks){
		//generate waypoints from streets
		//a way point should be generated on any street that IS NOT UP/DOWN or RIGHT/LEFT
		
		bool cornersOnly = true;
		foreach (Street street in streets){
			if (street.NextToPark || !cornersOnly || !street.isUpDown() && ! street.isRightLeft()){
				this.addWaypoint(street.Position);
			}
		}
		foreach (Park park in parks) {
			this.addWaypoint(park.Position);
		}

		foreach (Building building in buildings) {
			if (building.BuildingType == Building.BUILDINGTYPE.HOTEL){
				this.hotelLocations.Add (building.Position);
			} else if (building.BuildingType == Building.BUILDINGTYPE.GUNSHOP){
				this.gunShopLocations.Add (building.Position);
			}
		}


	}

	/// <summary>
	/// Gets the waypoints visible from the given position
	/// </summary>
	/// <returns>The waypoints visible from.</returns>
	/// <param name="position">Position.</param>
	public List<Vector2> getWaypointsVisibleFrom(Vector2 position){
		List<Vector2> visibleWayPoints = new List<Vector2> ();

		//TODO maybe at some point make this not a O(n) alg
		foreach (WayPoint waypoint in this.wayPoints) {
			if (this.canSee(position, waypoint.Position)){
				visibleWayPoints.Add (waypoint.Position);
			}
		}

		return visibleWayPoints;
	}

	/// <summary>
	/// Add a waypoint to the pathfinder. 
	/// each new waypoint will check what other waypoints it can see. It will 
	/// </summary>
	/// <param name="wayPoint">Way point.</param>
	private void addWaypoint(Vector2 waypointPosition){

		//create a new waypoint
		WayPoint wayPoint = new WayPoint (waypointPosition);


		Vector3 origin = new Vector3 (waypointPosition.x, 1, waypointPosition.y);
		if (DEBUG) {
			Debug.DrawLine (origin, origin +new Vector3(1,0,1), Color.blue, 10000f, false);
		}
	
		//determine what waypoints the new waypoint can see
		foreach (WayPoint otherWayPoint in this.wayPoints) {

			//math n stuff
			//TODO replace with canSee()
			Vector3 direction = (new Vector3(otherWayPoint.Position.x - origin.x, 0, otherWayPoint.Position.y - origin.z));
			float distance = direction.magnitude;
			direction.Normalize();

			bool canSee = !Physics.Raycast(origin, direction, distance, ((1 << LayerMask.NameToLayer("City"))));

			if (canSee){

				//new waypoint can see another waypoint, which means the other waypoint can see the new waypoint. Reflect changes in both waypoints
				wayPoint.addChild(otherWayPoint, distance);
				otherWayPoint.addChild (wayPoint, distance);

				if (DEBUG){
					Debug.DrawLine(origin, origin + distance * direction, Color.red, 10000f, false);
				}
			}

		}


		this.wayPoints.Add (wayPoint);
	}

	private bool canSee(Vector2 a, Vector2 b){
		//using 2, which should be right in the middle of the player
		Vector3 origin = new Vector3 (a.x, .5f, a.y);
		Vector3 direction = new Vector3 (b.x - a.x, 0, b.y - a.y);
		float distance = direction.magnitude;
		direction.Normalize ();

        return !Physics.Raycast(origin, direction, distance,
            ((1 << LayerMask.NameToLayer("City")) | (1 << LayerMask.NameToLayer("Sidewalk"))));
	}



}
