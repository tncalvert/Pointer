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
		/// Normalize the elements. All values will be brought within the range of 0 and 1. 
		/// 
		/// </summary>
		public void normalize(){
			float sum = 0;
			foreach (string key in this.elements.Keys){
				sum += this.elements[key];
			}


			foreach (string key in this.elements.Keys) {
				this.elements[key] /= sum;
			}

		}

		/// <summary>
		/// Gets the max value
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
		/// Gets the element with max value, fuzzified. This weights each answer by its value and rolls a die
		/// Note, this function will not call normalize(), but values SHOULD be normalized before you try to use this function
		/// Null will be returned iff something bonkers happens
		/// </summary>
		/// <returns>The fuzzy max.</returns>
		public string getFuzzyMax(){
			float value = Random.value;
			float prev = 0;
			foreach (string key in this.elements.Keys){
				if (value >= prev && value < prev + this.elements[key]){
					return key;
				} else {
					prev += this.elements[key];
				}
			}
			return null;
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
