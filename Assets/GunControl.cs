﻿using UnityEngine;
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

	// Use this for initialization
	void Start () {
        victimMonitor = victim.gameObject.GetComponent<VictimMonitor>();
		 muzzleFlash = this.GetComponentInChildren<ParticleSystem> ();

	}
	
	// Update is called once per frame
	void Update () {

        
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
			if (Physics.Raycast(this.transform.position,direction,out hit,9999,~(1<<LayerMask.NameToLayer("Victims")))){
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
