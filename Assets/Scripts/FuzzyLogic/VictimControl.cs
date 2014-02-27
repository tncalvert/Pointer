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

	// The VictimData head of the control. It holds the important data, and should be used carefully.
	private VictimData head;
	public VictimData Head {
		get { return this.head; }
		set { 
			this.head = value;
			this.updateFromHead();		
		}
	}

	/* VARIABLES OF THE VICTIM
	 * These variables may change due to game events.
	 */

	// how sleepy is the victim? on a scale of 0 to 1, hwere 1 is practically asleep and 0 is as chippy as a bird
	public float sleepyness;

	// how many shots can the victim shoot 
	public int ammo;

	// does the victim have a gun. Yes or no?
	public bool hasGun;


	/* Fuzzy Logic declarations 
	 */

	// The fuzzy brain of the victim
	private FuzzyBrain brain;

	// The fuzzy set containing all of the actions that a victim may way to do
	private FuzzySet actionSet;

	private FuzzySet peopleAround;
	private FuzzySet monsterInSight;
	private FuzzySet terror;


	private VictimSteering steering;
	private Material material;

	/// <summary>
	/// Updates the values in this instance from the VictimData head.
	/// </summary>
	public void updateFromHead(){
		//TODO implement
	}

	/// <summary>
	/// Updates the values in the VictimData head to reflect the values in this instance
	/// </summary>
	public void updateHeadValues(){
		//TODO implement
	}


	// Use this for initialization
	void Start () {


		if (this.head == null) {
			this.head = new VictimData ();
			this.updateHeadValues();
		}




		this.steering = this.GetComponent<VictimSteering> ();
		this.material = this.GetComponent<Renderer> ().material;


		this.brain = new FuzzyBrain ();

		this.actionSet = new FuzzySet ("actionSet");
		//this.brain.addFuzzySet (this.actionSet);
		this.brain.setRefreshFunction (this.actionSet, refreshActionSet);


		this.peopleAround = new FuzzySet ("peopleAround");
		//this.brain.addFuzzySet (this.peopleAround);
		this.brain.setRefreshFunction (this.peopleAround, refreshPeopleAroundSet);
		//this.peopleAround =  

		this.monsterInSight = new FuzzySet ("monsterInSight");
		//this.brain.addFuzzySet (this.monsterInSight);
		this.brain.setRefreshFunction (this.monsterInSight, refreshMonsterInSightSet);

		this.terror = new FuzzySet ("terror");
		//this.brain.addFuzzySet (this.terror);
		this.brain.setRefreshFunction (this.terror, refreshTerrorSet);

		//TODO register update functions for sets



		this.toughness = Random.value;
		this.sleepyness = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//this.refreshMonsterInSightSet (this.monsterInSight);
		//this.refreshTerrorSet (this.terror);
		this.updateColor ();

	}

	/// <summary>
	/// Updates the sleepyness. Victims get tired with time
	/// </summary>
	private void updateSleepyness(){
		this.sleepyness = Mathf.Clamp (this.sleepyness + .0001f, 0, 1);
	}

	/// <summary>
	/// Updates the color of the victim. Tough people are red, others are blue.
	/// </summary>
	private void updateColor(){
		float red = Mathf.Clamp (this.toughness, 0, 1);
		float blue = 1 - red;
		this.material.color = new Color (red, 0, blue);

	}


	/// <summary>
	/// Computes the current action.
	/// </summary>
	/// <returns>The current action.</returns>
	public string computeCurrentAction(){
		this.brain.refreshValues (this.peopleAround,
		                          this.monsterInSight,
		                          this.terror);

		this.brain.refreshValues (this.actionSet);
		return this.actionSet.getFuzzyMax();
	}

	/// <summary>
	/// Plans for given action. 
	/// </summary>
	/// <param name="action">Action. This string should be one of the ACTION_* strings in VictimControl</param>
	public void planForAction(string action){
		switch (action) {
		case ACTION_FLEE:
			break;
		case ACTION_SHOOT:
			break;
		case ACTION_FIND_AMMO:
			break;
		case ACTION_FIND_GROUP:
			break;
		case ACTION_FIND_GUN:
			break;
		case ACTION_FIND_ROOM:
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// returns true if the victim has the capacity to get a gun from the store. This assumes that the victim is physically next to the store
	/// </summary>
	/// <returns><c>true</c>, If the victim can get the gun, <c>false</c> otherwise.</returns>
	public bool canGetGun(){
		//TODO implement
		return false;
	}

	/// <summary>
	///  returns true if the victim has the capacity to get ammo from the store. This assumes that the victim is physically next to the store
	/// </summary>
	/// <returns><c>true</c>, If the victim can get the gun, <c>false</c> otherwise.</returns>
	public bool canGetAmmo(){
		//TODO implement
		return false;
	}

	/// <summary>
	/// returns true if the victim has the capacity to get rest from the hotel. This assumes that the victim is physically next to the hotel
	/// </summary>
	/// <returns><c>true</c>, if the victim can rest <c>false</c> otherwise.</returns>
	public bool canGetRest(){
		//TODO implement
		return false;
	}




	private void refreshActionSet(FuzzySet set){
		set.resetAll ();


		string people = this.peopleAround.getFuzzyMax ();
		string monster = this.monsterInSight.getFuzzyMax ();
		string terr = this.terror.getFuzzyMax ();

		/*THE MONSTER SHOWS UP!!!!!!!!
		*/
		//if I am alone and the monster shows up
		if (people == FuzzyBrain.NONE && monster == FuzzyBrain.MANY) {
			set[ACTION_FLEE] += (1-this.bravery)*.8f;
			set[ACTION_SHOOT] += this.toughness * .4f * (this.hasGun ? 1 : 0);
		}
		//should it be
		//set{ACTION_FLEE] += (this.peopleAround[FuzzyBrain.NONE] * this.monsterInSight[FuzzyBrain.MANY]) * (1 - this.bravery)*.8f;
		//?




		//if I am in a small group and the monster shows up
		if (people == FuzzyBrain.FEW && monster == FuzzyBrain.MANY) {
			set[ACTION_FLEE] += (1-this.bravery)*.6f;
			set[ACTION_SHOOT] += this.toughness * .6f * (this.hasGun ? 1 : 0);
		}

		//if I am in a group and the monster shows up
		if (people == FuzzyBrain.MANY && monster == FuzzyBrain.MANY) {
			set[ACTION_FLEE] += (1-this.bravery)*.4f;
			set[ACTION_SHOOT] += this.toughness * .8f * (this.hasGun ? 1 : 0);
		}


		/*I CANNOT SEE THE MONSTER
		*/
		//if I am alone and I cant see the monster, I should probably find a group or a resource
		if (people == FuzzyBrain.NONE && monster != FuzzyBrain.MANY) {
			set[ACTION_FIND_GROUP] += 1-this.independence; // I want to find a group if I have low independence
			set[ACTION_FIND_GUN] += this.independence * (this.hasGun ? 0 : 1);//I want to find a gun if I have high independence and don't have a gun
			set[ACTION_FIND_AMMO] += this.independence * (this.hasGun ? 1 : 0);//I want to find ammo if I have a gun and if I have high independence
			set[ACTION_FIND_ROOM] += this.sleepyness;//I want to find a room if I am sleepy
		}

		//if I am in a small group and I cant see the monster
		if (people == FuzzyBrain.FEW && monster != FuzzyBrain.MANY) {
			set[ACTION_FIND_GUN] += .8f*this.independence * (this.hasGun ? 0 : 1);//I want to find a gun if I have high independence and don't have a gun
			set[ACTION_FIND_AMMO] += .8f*this.independence * (this.hasGun ? 1 : 0);//I want to find ammo if I have a gun and if I have high independence
			set[ACTION_FIND_ROOM] += .8f*this.sleepyness;//I want to find a room if I am sleepy
		}

		//if I am in a group and I cant see the monster
		if (people == FuzzyBrain.FEW && monster != FuzzyBrain.MANY) {
			set[ACTION_FIND_GUN] += .5f*this.independence * (this.hasGun ? 0 : 1);//I want to find a gun if I have high independence and don't have a gun
			set[ACTION_FIND_AMMO] += .5f*this.independence * (this.hasGun ? 1 : 0);//I want to find ammo if I have a gun and if I have high independence
			set[ACTION_FIND_ROOM] += .5f*this.sleepyness;//I want to find a room if I am sleepy
		}

		set [ACTION_FLEE] /= Mathf.Max (float.Epsilon, 1-this.sleepyness);//more likely to run if you are tired
		set [ACTION_FLEE] *= 1-this.toughness;//less likely to run if you are tough

		set[ACTION_SHOOT] /= Mathf.Max (float.Epsilon, 1-this.toughness); //more likely to shoot if you are tough
		set[ACTION_SHOOT] *= 1-this.sleepyness; //less likely to shoot if you are tired


		set.normalize ();
	}

	private void refreshPeopleAroundSet(FuzzySet set){
		set.resetAll ();

		Collider[] colliders = Physics.OverlapSphere (this.steering.transform.position, 10, (1 << LayerMask.NameToLayer ("Victims")));
		RaycastHit hit = new RaycastHit ();
		int count = 0;
		foreach (Collider collider in colliders) {
			if (!Physics.Raycast(this.steering.transform.position,
			                    (collider.transform.position - this.steering.transform.position).normalized, 
			                    (collider.transform.position - this.steering.transform.position).magnitude,
			                    (1 << LayerMask.NameToLayer("City")))){
				count += 1;
			}
		}

		if (count < 2) {
			set [FuzzyBrain.NONE] += .8f;
			set [FuzzyBrain.FEW] += .2f;
		} else if (count < 6) {
			set [FuzzyBrain.NONE] += .2f;
			set [FuzzyBrain.FEW] += .5f;
			set [FuzzyBrain.MANY] += .3f;
		} else {
			set [FuzzyBrain.NONE] += .1f;
			set [FuzzyBrain.FEW] += .2f;
			set [FuzzyBrain.MANY] += .7f;
		}

		set.normalize ();
	}

	//The victims dont have to looking in the direction of the monster
	//Works if we can draw a line from the victim to the monster.
	private void refreshMonsterInSightSet(FuzzySet set){
		set.resetAll ();
		//Victim Position
		Vector3 victim = new Vector3 (this.rigidbody.position.x, this.rigidbody.position.y, this.rigidbody.position.z);

		//Player Position
		PlayerSteering monster = (PlayerSteering)GameObject.FindObjectOfType (typeof(PlayerSteering));
		Vector3 player = new Vector3 (monster.rigidbody.position.x, monster.rigidbody.position.y, monster.rigidbody.position.z);

		if (!Physics.Raycast (victim, (player - victim), (player - victim).magnitude,
		                     (1 << LayerMask.NameToLayer ("City")) | (1 << LayerMask.NameToLayer ("Sidewalk")))) {
			//Debug.Log ("MONSTER IN SIGHT!");

			//What about FuzzyBrain.FEW/NONE/SOME?
			set [FuzzyBrain.MANY] += 1f;
		}

		set.normalize ();
	}

	private void refreshTerrorSet(FuzzySet set){
		set.resetAll ();

		int radius = 50;
		//can I see blood?

		int bloodCount = 0;

		Collider[] nearbySplatter = Physics.OverlapSphere(rigidbody.position, radius, 1 << LayerMask.NameToLayer("Ground"));

		foreach (var n in nearbySplatter) {
			if (n.name.Equals ("splatFab(Clone)"))
				bloodCount++;
		}

		//Dont really know what i'm doing here
		if (bloodCount == 0)
			set [FuzzyBrain.NONE] += 1f;
		else if (bloodCount == 1) {
			set [FuzzyBrain.FEW] += 0.7f;
			set [FuzzyBrain.NONE] += 0.3f;
		} else if (bloodCount == 2) {
			set [FuzzyBrain.SOME] += 0.6f;
			set [FuzzyBrain.FEW] += 0.1f;
			set [FuzzyBrain.NONE] += 0.1f;
			set [FuzzyBrain.MANY] += 0.2f;
		} else {
			set [FuzzyBrain.MANY] += 0.8f;
			set[FuzzyBrain.SOME] += 0.2f;
		}

		//how many lights are around?
		int lightCount = 0;
		
		Collider[] nearbyLights = Physics.OverlapSphere(rigidbody.position, radius, 1 << LayerMask.NameToLayer("City"));
		
		foreach (var n in nearbyLights) {
			if (n.name.Equals ("StreetLight(Clone)"))
				lightCount++;
		}
		
		//Dont really know what i'm doing here
		if (lightCount == 0)
			set [FuzzyBrain.NONE] += 1f;
		else if (bloodCount == 1) {
			set [FuzzyBrain.FEW] += 0.7f;
			set [FuzzyBrain.NONE] += 0.3f;
		} else if (bloodCount == 2) {
			set [FuzzyBrain.SOME] += 0.6f;
			set [FuzzyBrain.FEW] += 0.1f;
			set [FuzzyBrain.NONE] += 0.1f;
			set [FuzzyBrain.MANY] += 0.2f;
		} else {
			set [FuzzyBrain.MANY] += 0.8f;
			set[FuzzyBrain.SOME] += 0.2f;
		}

		//monster in sight?

		//Victim Position
		Vector3 victim = new Vector3 (this.rigidbody.position.x, this.rigidbody.position.y, this.rigidbody.position.z);
		
		//Player Position
		PlayerSteering monster = (PlayerSteering)GameObject.FindObjectOfType (typeof(PlayerSteering));
		Vector3 player = new Vector3 (monster.rigidbody.position.x, monster.rigidbody.position.y, monster.rigidbody.position.z);
		
		if (!Physics.Raycast (victim, (player - victim), (player - victim).magnitude,
		                      (1 << LayerMask.NameToLayer ("City")) | (1 << LayerMask.NameToLayer ("Sidewalk")))) {
			//Debug.Log ("MONSTER IN SIGHT!");
			
			//What about FuzzyBrain.FEW/NONE/SOME?
			set [FuzzyBrain.MANY] += 1f;
		}

		set.normalize ();
	}

}
