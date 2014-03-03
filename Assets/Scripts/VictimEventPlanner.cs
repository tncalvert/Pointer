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


	private string runningAction;
	private bool monsterWasInSight;
	private int initialAmmo;
	private float initialSleepy;

	private float timePassed;

	/// <summary>
	/// The time until new fuzzy call. Some amount of time after an event, it will recalculate, just to prevent things from getting stuck
	/// </summary>
	public float timeUntilNewFuzzyCall = 7;


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

		/* Do a new fuzzy computation 
		 * IF
		 * 	monster is newly visible
		 * OR
		 *  currentAction is complete
		 * OR 
		 * 	x time has passed (to avoid AIs getting stuck for too long)
		 * OR
		 *  ?
		 */

		if (this.isMonsterNewlyVisible () 
		    || this.completedCurrentAction () 
		    || this.hasEnoughTimePassed ()) {
			this.timePassed = 0;
			this.runningAction = this.control.computeCurrentAction();
			this.control.planForAction(this.runningAction);
			this.initialAmmo = this.control.ammo;
			this.initialSleepy = this.control.sleepyness;
		}


	}

	/// <summary>
	/// Determines if the monster just came into view
	/// </summary>
	/// <returns><c>true</c>, if monster newly visible was ised, <c>false</c> otherwise.</returns>
	private bool isMonsterNewlyVisible(){

		if (this.monsterWasInSight) {
			return false; //the victim already knows about the monster
		}

		GameObject fearedObject = GameObject.FindWithTag("feared");
		bool monsterCurrentlyInSight = !Physics.Raycast (this.transform.position,
		                                               (fearedObject.transform.position - this.transform.position).normalized,
		                                               (fearedObject.transform.position - this.transform.position).magnitude,
		                                               ~(1 << LayerMask.NameToLayer ("city")));

		if (monsterCurrentlyInSight) {
			this.monsterWasInSight = true;
			return true; //we can see the monster now. Eep.
		} else {
			this.monsterWasInSight = false;
			return false; //we cannot see the monster. Phew.
		}

	}

	/// <summary>
	/// Deteremines if the victim Completed the current action.
	/// </summary>
	/// <returns><c>true</c>, if current action was completeded, <c>false</c> otherwise.</returns>
	private bool completedCurrentAction(){

		//depending on the action, we rank its completeness differently. 
		//with more time, all of this probably should been refactored into a better design. Maybe where an Action class provided a name, a planning method, and a completeness method
		switch (this.runningAction) {
		case VictimControl.ACTION_FIND_AMMO: //If I am searching for ammo, this task is done when my ammo goes up. Weather I keep on deciding to get more ammo is not part of this alg
			return this.control.ammo > this.initialAmmo;
			break;
		case VictimControl.ACTION_FIND_GROUP: //If I am searching for a group, this task is done when the number of people goes up. 
			break;
		case VictimControl.ACTION_FIND_GUN: //If I am searching for a gun, this is task is done when I have one
			return this.control.HasGun;
			break;
		case VictimControl.ACTION_FIND_ROOM: //If I am searching for rest, this task is done when my sleepness goes down
			return this.initialSleepy > this.control.sleepyness;
			break;
		case VictimControl.ACTION_FLEE:
			break;
		case VictimControl.ACTION_SHOOT:
			break;
		default:
			break;
		}


		return false;
	}

	/// <summary>
	/// Determines if enough time passed.
	/// </summary>
	/// <returns><c>true</c>, if enough time passed was hased, <c>false</c> otherwise.</returns>
	private bool hasEnoughTimePassed(){

		this.timePassed += Time.fixedDeltaTime;
		if (this.timePassed >= this.timeUntilNewFuzzyCall) {
			this.timePassed = 0;
			return true;
		} else {
			return false;
		}
	}
}
