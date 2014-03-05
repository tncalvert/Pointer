using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// A Fuzzy Set has elements
/// </summary>
public class FuzzySet
{
	
	/// <summary>
	/// The fuzzy elements
	/// </summary>
	private Dictionary<string, float> elements;

	/// <summary>
	/// The name of the set
	/// </summary>
	private string name;

	public FuzzySet(string name){
		this.name = name;
		this.elements = new Dictionary<string, float>();
	}
	public FuzzySet(string name, Dictionary<string, float> elements){
		this.name = name;
		this.elements = elements;
	}

	/// <summary>
	/// Gets the name of the set
	/// </summary>
	/// <returns>The name.</returns>
	public string getName(){
		return this.name;
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
		return (this.elements.ContainsKey (name.ToLower ()) ? this.elements [name.ToLower ()] : 0);
	}

	/// <summary>
	/// Gets or sets the membership of the specified element
	/// </summary>
	/// <param name="key">element name. Note, case does not matter</param>
	public float this [string key] {
		get{ return this.getElement (key);}
		set{ this.setElement (key, value);}
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
		
		Dictionary<string, float> buffer = new Dictionary<string, float> ();
		foreach (string key in this.elements.Keys) {
			buffer[key] = this.elements[key] / sum;
		}

		foreach (string key in buffer.Keys) {
			this.elements[key] = buffer[key];
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

	public string getMaxKey(){
		float max = float.NegativeInfinity;
		string k = "";
		foreach (string key in this.elements.Keys) {
			max = Mathf.Max (max, this.elements [key]);
			k = key;
		}
		return k;
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

		return this.getMaxKey ();
	}

	/// <summary>
	/// Resets all elements to zero
	/// </summary>
	public void resetAll(){
		List<string> a = new List<string> ();
		foreach (string key in this.elements.Keys) {
			a.Add(key);
		}
		foreach (string key in a) {
			this.elements[key] = 0;
		}
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

