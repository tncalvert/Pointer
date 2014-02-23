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
    int playerScore = 0;

    void Start() {
        splatter = gameObject.AddComponent<SplatterScript>();
        splatter.bloodSplat = splatterPrefab;
    }

	void Update () {
        //Kill the victims
        if (Input.GetKeyDown("space")) {
            Collider[] nearbyVictims = Physics.OverlapSphere(rigidbody.position, attackRange, 1 << LayerMask.NameToLayer("Victims"));

            foreach (var n in nearbyVictims) {
                if (this.splatter != null) {
                    this.splatter.makeSplat(n.transform.position, new Vector3(0, -1, 0), 10, (1 << LayerMask.NameToLayer("City")));
                }

                Destroy(n.gameObject);
                playerScore++;

                Debug.Log("Score: " + playerScore);
            }

        }
	}
}
