using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {


    //// <summary>
    /// The splatter. Squishy.
    /// </summary>
    public SplatterScript splatter;
    public GameObject splatterPrefab;
	public ParticleSystem attackEffect;

    /// <summary>
    /// Attack range for the monster
    /// </summary>
    int attackRange = 5;


	//Attack Timer
	float attackTimer = 120;
	
	//Wilhelm Scream
	AudioClip scream;
	
	//Attack Tracker
	AttackTracker attack;
	
	//Level tracker
	Inspector inspect;
	
	void Start() {
		splatter = gameObject.AddComponent<SplatterScript>();
		splatter.bloodSplat = splatterPrefab;
		
		//Keep track of victims
		GameObject attackObj = GameObject.Find ("AttackTimer");
		attack = attackObj.GetComponent<AttackTracker> ();


		attackEffect = (ParticleSystem)Instantiate (this.attackEffect);

		
		GameObject inspectObj = GameObject.Find ("MasterGameObject");
		inspect = inspectObj.GetComponent<Inspector> ();

	}
	
	void Update () {

		if(attackTimer != 120)
		{
			attackTimer++;

			if(attackTimer == 30)
				attack.setText("3");

			if(attackTimer == 60)
				attack.setText("2");

			if(attackTimer == 90)
				attack.setText("1");

			if(attackTimer == 120)
				attack.setText ("Attack Ready!");
		}

        //Kill the victims
        if (attackTimer == 120 && Input.GetKeyDown("space")) {

			attackEffect.transform.position = this.transform.position;
			attackEffect.Play();

			attackTimer = 0;
			attack.setText("4");

            Collider[] nearbyVictims = Physics.OverlapSphere(rigidbody.position, attackRange, 1 << LayerMask.NameToLayer("Victims"));

			foreach (Collider n in nearbyVictims) {
                if (this.splatter != null) {
					this.splatter.makeBigSplat(n.transform.position, Vector3.down + (n.transform.position - this.transform.position).normalized, 10, 5, (1<<LayerMask.NameToLayer("City")));
					//this.splatter.makeSplat(n.transform.position, new Vector3(0, -1, 0), 10, (1 << LayerMask.NameToLayer("City")));

				}
				
				if (nearbyVictims.Length != 0)
					audio.Play ();
			}
		}
	}
}

