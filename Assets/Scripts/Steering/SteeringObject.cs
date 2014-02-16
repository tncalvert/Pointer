using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows the object containing this script to be moved by a series of steering forces
/// Requires that the object has a rigidbody on it
/// Class based on demonstrations and code by Prof. Kesselman
/// </summary>
public class SteeringObject : MonoBehaviour {

    /// <summary>
    /// The maximum forces that the steering behaviors can exert on the object
    /// </summary>
    public float maxForce = 1.0f;

    /// <summary>
    /// The maximum velocity that results from the application of a steering force
    /// </summary>
    public float maxVelocity = 3.0f;

    /// <summary>
    /// A list of all steering forces acting on this object
    /// </summary>
    public List<SteeringForce> steeringForces = new List<SteeringForce>();

    // Behaviors
    Seek seeker;
    CollisionAvoidance collisionAvoider;
    Fear fear;
    WallAvoidance wallAvoidance;
    Wander wander;
    Cohesion cohesion;
    Separation separation;
    Alignment alignment;

	void Start () { 
        // Check to see if we have a rigid body
        try {
            GetComponent<Rigidbody>();

            // Add always used steering forces

            seeker = new Seek(rigidbody);
            AddSteeringForce(seeker);
            //fear = new Fear(rigidbody);
            //AddSteeringForce(fear);
            //collisionAvoider = new CollisionAvoidance(rigidbody,
            //    Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y,
            //    renderer.bounds.size.z) / 2, renderer.bounds.center);
            //AddSteeringForce(collisionAvoider);
            //wander = new Wander(rigidbody);
            //AddSteeringForce(wander);
            //wallAvoidance = new WallAvoidance(rigidbody,
            //    Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y,
            //    renderer.bounds.size.z) / 2);
            //AddSteeringForce(wallAvoidance);
            cohesion = new Cohesion(rigidbody);
            separation = new Separation(rigidbody);
            alignment = new Alignment(rigidbody);
            AddSteeringForce(cohesion);
            AddSteeringForce(separation);
            AddSteeringForce(alignment);

        } catch (MissingComponentException e) {
            Debug.Log("Object " + this.name + " does not have a Rigidbody.\n" + e.Message);
        }
        
    }
	
	void Update () {

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                seeker.SetSeekPosition(hit.point);
            }
        }

        // Update necessary components
        //collisionAvoider.UpdateRendererCenter(renderer.bounds.center);

        Vector3 force = new Vector3(0, 0, 0);

        // Accumulate the effects of all steering forces
        foreach (SteeringForce sf in steeringForces) {
            force = force + sf.GetSteeringForce(maxVelocity);
        }

        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force
        GetComponent<Rigidbody>().AddForce(force);
        
	}

    /// <summary>
    /// Adds a new steering force to the object
    /// Does not check if there is already a steering force of this type on the object
    /// </summary>
    /// <param name="sf">The new steering force to add</param>
    public void AddSteeringForce(SteeringForce sf) {
        steeringForces.Add(sf);
    }

    /// <summary>
    /// Removes a steering force from the object
    /// Does not handle duplicates
    /// </summary>
    /// <param name="sf">Steering force to remove</param>
    public void RemoveSteeringForce(SteeringForce sf) {
        steeringForces.Remove(sf);
    }
}
