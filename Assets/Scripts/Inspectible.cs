using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Inspectible : MonoBehaviour {


	//public string text;
	public GetInspectorLines getTextFunc;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public delegate List<string> GetInspectorLines();

}
