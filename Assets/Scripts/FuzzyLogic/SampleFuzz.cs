using UnityEngine;
using System.Collections;

public class SampleFuzz : MonoBehaviour {

	FuzzyBrain fBrain;

	FuzzyBrain.FuzzySet actionSet;

	public float task(FuzzyBrain.FuzzySet set){
		return 0;
	}

	public SampleFuzz(){
		fBrain = new FuzzyBrain();

		actionSet = new FuzzyBrain.FuzzySet ();

		actionSet.doOperation (task);

		//
		actionSet.setElement ("afraid", .4f);
		actionSet.setElement ("brave", .2f);



		actionSet.normalize ();
		if (actionSet.getFuzzyMax ().Equals ("afraid")) {
			//RUN!!!
		} else {
			//FIGHT!!!!!!!!
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
