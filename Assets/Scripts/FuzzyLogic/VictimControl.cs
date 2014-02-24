using UnityEngine;
using System.Collections;

/// <summary>
/// The control class handles victim behaviour
/// </summary>
public class VictimControl : MonoBehaviour {

	// ActionSet elements
	public const string ACTION_FLEE = "flee";
	public const string ACTION_FIND_GUN = "findGun";
	public const string ACTION_FIND_AMMO = "findAmmo";
	public const string ACTION_FIND_ROOM = "findRoom";
	public const string ACTION_FIND_GROUP = "findGroup";
	public const string ACTION_SHOOT = "shoot";

	/* ATTRIBUTES OF THE VICTIM
	 * These attributes don't have reason to change through out the game.
	 * They are the constant factors, and should range from 0 to 1, where
	 * 1 = VERY, 0 = NOT AT ALL
	 */

	//toughness factors into aggression and endurance	
	public float toughness;

	//bravery factors into aggression and flee
	public float bravery;

	//independence factors into how social the victim is
	public float independence;

	/* VARIABLES OF THE VICTIM
	 * These variables may change due to game events.
	 */

	public float sleepyness;
	public int ammo;
	public bool hasGun;


	/* Fuzzy Logic declarations 
	 */

	// The fuzzy brain of the victim
	private FuzzyBrain brain;

	// The fuzzy set containing all of the actions that a victim may way to do
	private FuzzySet actionSet;

	private FuzzySet peopleAround;


	// Use this for initialization
	void Start () {
		this.brain = new FuzzyBrain ();

		this.actionSet = this.brain ["actionSet"];
		this.actionSet ["someAction"] = 0;



		this.peopleAround = this.brain ["peopleAround"];




		//TODO register update functions for sets

	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public string computeCurrentAction(){
		this.brain.refreshValues (this.peopleAround);
		return "";
	}
}
