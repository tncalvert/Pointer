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

    /// <summary>
    /// Player score
    /// </summary>
    private int playerScore = 0;

	//Attack Timer
	float attackTimer = 120;

	//Wilhelm Scream
	AudioClip scream;

    void Start() {
        splatter = gameObject.AddComponent<SplatterScript>();
        splatter.bloodSplat = splatterPrefab;
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

            foreach (var n in nearbyVictims) {
                if (this.splatter != null) {
                    this.splatter.makeSplat(n.transform.position, new Vector3(0, -1, 0), 10, (1 << LayerMask.NameToLayer("City")));
                }

                Destroy(n.gameObject);
                playerScore++;

                Debug.Log("Score: " + playerScore);
            }

			if(nearbyVictims.Length != 0)
				audio.Play ();

			attackTimer = 0;
        }
	}

	//Return the player score to display
	//public int getScore()
	//{
	//	return playerScore;
	//}
}
