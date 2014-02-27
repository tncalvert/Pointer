using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Inspector : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	



	}

	void OnGUI(){

		if (Input.GetKey (KeyCode.LeftShift)) {

			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(cameraRay, out hit)) {
				
				GameObject obj = hit.collider.gameObject;
				
				Inspectible insp = obj.GetComponent<Inspectible>();
				if (insp != null && insp.getTextFunc != null){
					//Debug.Log (insp.text);

					float y = Screen.height - Input.mousePosition.y;
					float x = Input.mousePosition.x;
					List<string> lines = insp.getTextFunc();
					string text = "";
					foreach(string line in lines){
						text += line + "\n";
					}
					
					GUI.TextArea(new Rect(x - 50,y + 5,100,(1+lines.Count) * 12),text);
					
				}
			}
			
			
		}

	}

}
