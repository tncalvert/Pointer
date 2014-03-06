using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Inspector : MonoBehaviour {


	private static bool inspecting;
	public static bool Inspecting { get { return inspecting; } }

    private bool masterPause;

	//Time Tracker
	LevelTime time;

	//Level tracker
	LevelChanger level;

	// Use this for initialization
	void Start () {
		//Keep track of time
		GameObject timeObj = GameObject.Find ("TimeCounter");
		time = timeObj.GetComponent<LevelTime> ();

		//Keep track of the level
		GameObject levelObj = GameObject.Find ("LevelChanger");
		level = levelObj.GetComponent<LevelChanger> ();
	}
	
	void OnGUI(){

        if (masterPause) { // If we're in the escape menu
            return;
        }

		//If the user is holding the left shift
		if (Input.GetKey(KeyCode.LeftShift)) {
			time.stopCounting();

			inspecting = true;
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
					
					GUI.TextArea(new Rect(x - 50,y + 5,200,(1+lines.Count) * 18),text);
					
				}
			}
			
			
		}
		//Else the user is not holding down shift
		else
		{
			//Resume counter
			inspecting = false;

			//Only continue counting if the level hasnt ended
			if(level.getLevelEnd() == 0)
				time.startCounting();
		}

	}

    /// <summary>
    /// Sets a master pause for the escape menu
    /// We don't want shift to accidentally override this
    /// so there needs to be a higher control
    /// </summary>
    /// <param name="pause"></param>
    public void setEscapePause(bool pause) {
        inspecting = pause;
        masterPause = pause;
        if (pause) {
            time.stopCounting();
        } else {
            time.startCounting();
        }
    }

	//Returns whether the user is inspecting or not
	public bool isInspecting()
	{
		return inspecting;
	}

}
