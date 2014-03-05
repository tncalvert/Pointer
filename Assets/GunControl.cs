using UnityEngine;
using System.Collections;

/// <summary>
/// Gun control. Provide functionality to fire a gun. Guns shoot instant bullets. There is no hope of dodging. 
/// </summary>
public class GunControl : MonoBehaviour {

	/// <summary>
	/// The fire cool down time. The gun must wait this amount of time before firing again
	/// </summary>
	public float fireCoolDownTime = .5f;


	private float fireTimeLeft = 0;


	private bool justFired;
	public bool JustFired{ get { return this.justFired; } }

	//which victim is holding this gun
	public VictimControl victim;

    public VictimMonitor victimMonitor;

    public GameObject player;



	private ParticleSystem muzzleFlash;
	public ParticleSystem MuzzleFlash {get{return this.muzzleFlash;}}
	private LineRenderer lineRend;

	private Color lineRendStartColor = new Color(1f,0,0);
	private Color lineRendEndColor = new Color(1f,1,0);
	private float lineRendAlphaDecay = 2f;
	private float lineRendAlpha = 1;

	// Use this for initialization
	void Start () {
        victimMonitor = victim.gameObject.GetComponent<VictimMonitor>();
		 muzzleFlash = this.GetComponentInChildren<ParticleSystem> ();
		 lineRend = this.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		this.lineRendStartColor.a = this.lineRendAlpha;
		this.lineRendEndColor.a = this.lineRendAlpha;
		this.lineRend.SetColors(this.lineRendStartColor, this.lineRendEndColor);
		this.lineRendAlpha = Mathf.Max (0, this.lineRendAlpha - (this.lineRendAlphaDecay * Time.deltaTime));


		fireTimeLeft -= Time.deltaTime;
		if (fireTimeLeft <= this.fireCoolDownTime * .75f) {
			this.justFired = false;
		}
	}

	/// <summary>
	/// Fire this gun
	/// </summary>
	public void fire(){

		if (fireTimeLeft <= 0) {
			this.justFired = true;
			fireTimeLeft = fireCoolDownTime;

			if (muzzleFlash != null){
				muzzleFlash.Play();
			}

			RaycastHit hit = new RaycastHit();
			//Debug.DrawLine(muzzleFlash.transform.position,muzzleFlash.transform.position +  muzzleFlash.transform.forward * 100, Color.cyan,1);
			Vector3 direction = new Vector3(muzzleFlash.transform.forward.x, 0, muzzleFlash.transform.forward.z);
			Vector2 n = new Vector2(-direction.z, direction.x).normalized;
			bool playerHit = false;
			for (int i = 0 ; i < 60 ; i ++){
				Vector3 randomness = .05f*(Random.value-.5f)*new Vector3(n.x, 0, n.y);
				playerHit |= (Physics.Raycast(muzzleFlash.transform.position, (direction+randomness).normalized ,out hit,9999,~(1<<LayerMask.NameToLayer("Victims")))
				              &&hit.collider.gameObject == player);
				this.lineRend.SetPosition(0, this.transform.position);
				this.lineRend.SetPosition(1, hit.collider.transform.position);
				this.lineRend.SetWidth(.2f,.15f);

				lineRendAlpha = 1;
				Debug.DrawRay(this.transform.position, direction, Color.red, 1);
			}


			if (playerHit){
				Debug.DrawLine(this.transform.position, hit.collider.transform.position,Color.cyan, 1);

				// Inform the monitor of a hit
                if (hit.collider.gameObject == player) {
					Debug.Log ("THE PLAYER GOT HIT!");
                    victimMonitor.DealtDamage(1f);
                }
			}
			
		}

	}

}
