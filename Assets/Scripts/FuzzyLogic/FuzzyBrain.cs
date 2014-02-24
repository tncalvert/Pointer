using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// the Fuzzy Brain is the class that will handle the fuzzy calculations.
/// </summary>
public class FuzzyBrain {


	public static float AND(float fuzzyElementA, float fuzzyElementB){
		return Mathf.Min (fuzzyElementA, fuzzyElementB);
	}
	public static float OR(float fuzzyElementA, float fuzzyElementB){
		return Mathf.Max (fuzzyElementA, fuzzyElementB);
	}
	public static float NOT(float fuzzyElement){
		return 1 - fuzzyElement;
	}


	public const string MANY = "many";
	public const string SOME = "some";
	public const string FEW = "few";
	public const string NONE = "none";





	/// <summary>
	/// The fuzzy sets that are part of this brain
	/// </summary>
	private Dictionary<string, FuzzySet> fuzzySets;

	private Dictionary<string, FuzzyRefresh> fuzzyRefresh;

	public FuzzyBrain(){
		this.fuzzySets = new Dictionary<string, FuzzySet> ();
		this.fuzzyRefresh = new Dictionary<string, FuzzyRefresh> ();
	}
	public FuzzyBrain(Dictionary<string, FuzzySet> fuzzySets){
		this.fuzzySets = fuzzySets;
		this.fuzzyRefresh = new Dictionary<string, FuzzyRefresh> ();
		if (this.fuzzySets == null) {
			this.fuzzySets = new Dictionary<string, FuzzySet>();
		}
	}




	/// <summary>
	/// Adds the fuzzy set.
	/// </summary>
	/// <param name="name">Name. Note that case does not matter</param>
	/// <param name="set">Set.</param>
	public void setFuzzySet(FuzzySet set){
		this.fuzzySets [set.getName().ToLower()] = set;
	}

	/// <summary>
	/// Gets the fuzzy set. Null will be returned if there is set with the given name
	/// </summary>
	/// <returns>The fuzzy set.</returns>
	/// <param name="name">Name. Note that case does not matter</param>
	public FuzzySet getFuzzySet(string name){
		return (this.fuzzySets.ContainsKey (name.ToLower ()) ? this.fuzzySets [name.ToLower ()] : null);
	}


	public FuzzySet this [string name]{
		get {
				if (this.fuzzySets.ContainsKey (name)) {
					return this.getFuzzySet (name);
				} else{
					this.setFuzzySet(new FuzzySet(name));
					return this.fuzzySets[name];
				}
			}
		set {
				if (this.fuzzySets.ContainsKey (name)) {
					this.setFuzzySet (value);
				} else {
					this.setFuzzySet(new FuzzySet(name));	
				}
			}
	}


	/// <summary>
	/// Sets the refresh function.
	/// The function will be called to update the memberships of the set
	/// when refreshValues or refreshAllValues is called
	/// </summary>
	/// <param name="set">Set.</param>
	/// <param name="func">Func.</param>
	public void setRefreshFunction(FuzzySet set, FuzzyRefresh func){
		this.fuzzyRefresh [set.getName ().ToLower ()] = func;
	}

	/// <summary>
	/// Refreshs the values of the set by using the FuzzyRefresh func associated with the given set. If no func is associated, nothing will happen
	/// </summary>
	/// <param name="set">Set.</param>
	public void refreshValues(params FuzzySet[] sets){
		for (int i = 0 ; i < sets.Length ; i++){
			FuzzySet set = sets[i];
			if (this.fuzzyRefresh.ContainsKey (set.getName ().ToLower ())) {
				this.fuzzyRefresh[set.getName().ToLower()].Invoke(set);
			}
		}

	}

	/// <summary>
	/// All refreshValues on all sets
	/// </summary>
	//public void refreshAllValues(){
	//	foreach (string key in this.fuzzyRefresh.Keys) {
	//		this.fuzzyRefresh[key].Invoke(this.fuzzySets[key]);
	//	}
	//}




}
public delegate void FuzzyRefresh(FuzzySet set);
