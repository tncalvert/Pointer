using UnityEngine;
using System.Collections;

/// <summary>
/// Victim event planner class is responsible for exchanging events between victim components
/// </summary>
public class VictimEventPlanner : MonoBehaviour {

	/// <summary>
	/// The VictimControl. This must exist in the gameobject
	/// </summary>
	private VictimControl control;

	/// <summary>
	/// The VictimSteering. This must exist in the gameobject
	/// </summary>
	private VictimSteering steering;

	/// <summary>
	/// boolean to keep track of if this script should run
	/// </summary>
	private bool valid;


	// Use this for initialization
	void Start () {
	
		this.control = this.GetComponent<VictimControl> ();
		this.steering = this.GetComponent<VictimSteering> ();


		this.valid = this.control != null && this.steering != null;

	}
	
	// Update is called once per frame
	void Update () {
		if (!this.valid) {
			return; //exit if not valid
		}



	}
}
