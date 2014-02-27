using UnityEngine;
using System.Collections;

/// <summary>
/// Victim data that holds all of the crucial information dictating how victims behave. 
/// The ideal use of this class is to serve as a field of other classes that pull values out of this class.
/// </summary>
public class VictimData {


	/**************************************
	 ****** VICTIM CONTROL VARIABLES ******
	 *************************************/

	// the bravery attribute of the victim
	public float AttribBravery { get; set; }

	// the toughness attribute of the victim
	public float AttribToughness{ get; set; }

	// the independence of the victim
	public float AttribIndependence{ get; set;}


	/***************************************
	 ****** VICTIM STEERING VARIABLES ******
	 **************************************/

	// the max velocity of the victim
	public float MaxVelocity{ get; set; }

	// the max force that may be applied to the victim
	public float MaxForce{ get; set; }

	// the square of the minimum arrival distance 
	public float MinimumArrivalRadiusSqrd{ get; set; }

	// the square of the mimimum complete arrival distance
	public float MinimumCompleteArrivalRadiusSqrd{get;set;}

	//the probability of getting a unique path
	public float UniquePathProbability{ get; set; }

	//the distance to check nearby paths
	public float PathCheckRadius{get; set;}

	/***************************************
	 *************STEERING WEIGHTS *********
	 **************************************/

	//the alignment weight
	public float SteeringAlignment{ get; set; }

	//the cohesion weight
	public float SteeringCohesion{get; set;}

	//the collision avoidance weight
	public float SteeringCollisionAvoidance{ get; set; }

	//the fear weight
	public float SteeringFear{get; set;}

	//the seek weight
	public float SteeringSeek{ get; set; }

	//the separation weight
	public float SteeringSeparation{get;set;}

	//the wallavoidance weight
	public float SteeringWallAvoidance{get; set;}

	//the wander weight
	public float SteeringWander{ get; set;}

	//the side walk love weight
	public float SteeringSideWalkLove{ get; set; }



	public VictimData(){
	}


}
