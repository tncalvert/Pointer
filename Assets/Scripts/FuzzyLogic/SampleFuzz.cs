using UnityEngine;
using System.Collections;

public class SampleFuzz : MonoBehaviour {

	FuzzyBrain fBrain;

	FuzzySet actions, bravery, peopleAround;



	public SampleFuzz(){
		fBrain = new FuzzyBrain();

		actions = new FuzzySet ("actions");
		peopleAround = new FuzzySet ("people");


	}

	public void refreshAction(FuzzySet actionSet){

		actionSet ["run"] = .2f;
		actionSet ["fight"] = .2f;
		string people = peopleAround.getFuzzyMax ();
		switch (people) {
			case FuzzyBrain.MANY:
				actionSet["fight"] += .3f;
				break;
			case FuzzyBrain.SOME:
				actionSet["fight"] += .3f;
				actionSet["run"] += .5f;
				break;
			case FuzzyBrain.NONE:
				actionSet["run"] += .6f;
				break;
			default:
				break;
		}

		actionSet.normalize ();
	}

	public void refreshPeopleAround(FuzzySet peopleSet){
		peopleSet [FuzzyBrain.MANY] = .3f;
		peopleSet [FuzzyBrain.SOME] = .3f;
		peopleSet [FuzzyBrain.NONE] = .3f;

		peopleSet.normalize ();
	}

	// Use this for initialization
	void Start () {
		fBrain.addFuzzySet (peopleAround);
		fBrain.setRefreshFunction (peopleAround, refreshPeopleAround);
		fBrain.setRefreshFunction (actions, refreshAction);
		fBrain.refreshValues (peopleAround, actions);
		
		
		string action = actions.getFuzzyMax ();
		Debug.Log (action);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
