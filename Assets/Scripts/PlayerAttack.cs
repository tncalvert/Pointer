using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

    //// <summary>
    /// The splatter. Squishy.
    /// </summary>
    public SplatterScript splatter;
    public GameObject splatterPrefab;

    /// <summary>
    /// Attack range for the monster
    /// </summary>
    int attackRange = 5;

	//Attack Timer
	float attackTimer = 120;

	//Wilhelm Scream
	AudioClip scream;

	//Score tracker
	ScoreDisplay score;

    void Start() {
        splatter = gameObject.AddComponent<SplatterScript>();
        splatter.bloodSplat = splatterPrefab;

		//Keep track of score
		GameObject timer = GameObject.Find ("ScoreCounter");
		score = timer.GetComponent<ScoreDisplay> ();
	}

	void Update () {
		if(attackTimer != 120)
		{
			attackTimer++;
			if(attackTimer == 120)
				Debug.Log ("Attack Ready");
		}

        //Kill the victims
        if (attackTimer == 120 && Input.GetKeyDown("space")) {
            Collider[] nearbyVictims = Physics.OverlapSphere(rigidbody.position, attackRange, 1 << LayerMask.NameToLayer("Victims"));

			foreach (Collider n in nearbyVictims) {
                if (this.splatter != null) {
					this.splatter.makeBigSplat(n.transform.position, Vector3.down + (n.transform.position - this.transform.position).normalized, 10, 5, (1<<LayerMask.NameToLayer("City")));
					//this.splatter.makeSplat(n.transform.position, new Vector3(0, -1, 0), 10, (1 << LayerMask.NameToLayer("City")));
				}

                Destroy(n.gameObject);
				score.incrementScore();
            }

			if(nearbyVictims.Length != 0)
				audio.Play ();

			attackTimer = 0;
        }
	}
}
