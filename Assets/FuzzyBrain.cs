using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// the Fuzzy Brain is the class that will handle the fuzzy calculations.
/// </summary>
public class FuzzyBrain {


	/// <summary>
	/// A Fuzzy Set has elements
	/// </summary>
	public class FuzzySet
	{

		/// <summary>
		/// The fuzzy elements
		/// </summary>
		private Dictionary<string, float> elements;

		public FuzzySet(){
			this.elements = new Dictionary<string, float>();
		}
		public FuzzySet(Dictionary<string, float> elements){
			this.elements = elements;
		}

		/// <summary>
		/// Sets the element. This will overwrite any existing value for this element
		/// </summary>
		/// <param name="name">Name. Note that case does not matter</param>
		/// <param name="value">Value.</param>
		public void setElement(string name, float value){
			this.elements [name.ToLower ()] = value;
		}

		/// <summary>
		/// Gets the element. NaN will be returned if no value for this element has been set
		/// </summary>
		/// <returns>The element.</returns>
		/// <param name="name">Name. Note that case does not matter</param>
		public float getElement(string name){
			return (this.elements.ContainsKey (name.ToLower ()) ? this.elements [name.ToLower ()] : float.NaN);
		}

		/// <summary>
		/// Normalize the elements. All values will be brought within the range of -1 and 1. 
		/// Examples...
		/// [A: 1, B: .5] -> [A: 1, B: .5]
		/// [A: 2, B: .5] -> [A: 1, B: .25]
		/// [A: -2, B: .5] -> [A: -1, B: .5]
		/// 
		/// TODO think about ratio change problem with negative + positive numbers
		/// </summary>
		public void normalize(){
			float max = float.NegativeInfinity;
			float min = float.PositiveInfinity;
			foreach (string key in this.elements.Keys){
				max = Mathf.Max (this.elements[key], max);
				min = Mathf.Min (this.elements[key], min);
			}
			max = Mathf.Max (1.0f, max);
			min = Mathf.Abs (Mathf.Min (-1.0f, min));

			foreach (string key in this.elements.Keys) {
				this.elements[key] /= ( (this.elements[key] > 0 )? max : min );
			}

		}

		/// <summary>
		/// Gets the element with the max value
		/// </summary>
		/// <returns>The max element.</returns>
		public float getMax(){
			float max = float.NegativeInfinity;
			foreach (string key in this.elements.Keys) {
				max = Mathf.Max (max, this.elements [key]);
			}
			return max;
		}

		/// <summary>
		/// perform FuzzySetOperation on this instance
		/// </summary>
		/// <returns>The operation.</returns>
		/// <param name="oper">Oper.</param>
		public float doOperation(FuzzySetOperation oper){
			return oper.Invoke (this);
		}

		public delegate float FuzzySetOperation(FuzzySet set);


	} // end of fuzzySet class def





	/// <summary>
	/// The fuzzy sets that are part of this brain
	/// </summary>
	private Dictionary<string, FuzzySet> fuzzySets;

	public FuzzyBrain(){
		this.fuzzySets = new Dictionary<string, FuzzySet> ();
	}
	public FuzzyBrain(Dictionary<string, FuzzySet> fuzzySets){
		this.fuzzySets = fuzzySets;
		if (this.fuzzySets == null) {
			this.fuzzySets = new Dictionary<string, FuzzySet>();
		}
	}

	/// <summary>
	/// Adds the fuzzy set.
	/// </summary>
	/// <param name="name">Name. Note that case does not matter</param>
	/// <param name="set">Set.</param>
	public void addFuzzySet(string name, FuzzySet set){
		this.fuzzySets [name.ToLower ()] = set;
	}

	/// <summary>
	/// Gets the fuzzy set. Null will be returned if there is set with the given name
	/// </summary>
	/// <returns>The fuzzy set.</returns>
	/// <param name="name">Name. Note that case does not matter</param>
	public FuzzySet getFuzzySet(string name){
		return (this.fuzzySets.ContainsKey (name.ToLower ()) ? this.fuzzySets [name.ToLower ()] : null);
	}




}
