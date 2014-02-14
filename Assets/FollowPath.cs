using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour {


	/// <summary>
	/// Very simple path following code. I wrote this to test the A*. I did 
	/// not write it as the final path following code.
	/// </summary>

	public PathFinder pathFinder;

	private Vector2[] path;
	private int currentPathIndex;
	public float speed = 1;

	// Use this for initialization
	void Start () {
		this.path = new Vector2[0];
		this.currentPathIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
	


		if (this.currentPathIndex < this.path.Length 
		    && this.path.Length > 0) {
			Vector2 position = new Vector2(this.transform.position.x, this.transform.position.z);

			//can I cut a corner?
			if (this.currentPathIndex + 1 < this.path.Length){
				Vector2 nextTarget = this.path[this.currentPathIndex+1];
				if (this.canSee(position, nextTarget)){
					this.currentPathIndex++;
				}
			}


			Vector2 target = this.path[this.currentPathIndex];

			Vector2 distance = (target - position);



			if (distance.sqrMagnitude < this.speed){
				currentPathIndex ++;
			} else {

				distance.Normalize();


				Vector3 movement = this.speed * new Vector3(distance.x, 0, distance.y);
				this.transform.position += movement;

			}

			//direction.Normalize();


			Debug.DrawLine(this.transform.position, new Vector3(target.x, 1, target.y), Color.blue, .001f, false);
			for (int i = currentPathIndex; i < this.path.Length-1; i ++) {
				Debug.DrawLine(new Vector3(this.path[i].x, 1, this.path[i].y), new Vector3(this.path[i+1].x, 1, this.path[i+1].y), Color.blue, .001f, false);
			}
		}


	


	}

	private bool canSee(Vector2 a, Vector2 b){
		Vector3 origin = new Vector3 (a.x, 1, a.y);
		Vector3 direction = new Vector3 (b.x - a.x, 1, b.y - a.y);
		float distance = direction.magnitude;
		direction.Normalize ();
		return !Physics.Raycast (origin, direction, distance);
	}

	public void setPath(Vector2[] path){
		this.path = path;
		this.currentPathIndex = 0;
	}
}
