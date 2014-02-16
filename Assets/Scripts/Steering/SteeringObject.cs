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
    /// Object containing methods for steering
    /// </summary>
    SteeringBehaviors steeringBehaviors;

	void Start () { 
        // Check to see if we have a rigid body
        try {
            GetComponent<Rigidbody>();

            steeringBehaviors = gameObject.AddComponent<SteeringBehaviors>();

        } catch (MissingComponentException e) {
            Debug.Log("Object " + this.name + " does not have a Rigidbody.\n" + e.Message);
        }
        
    }
	
	void Update () {

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }

        Vector3 force = new Vector3(0, 0, 0);

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                steeringBehaviors.targetPosition = hit.point;
            }
        }

        // Add forces
        force += steeringBehaviors.GetAlignmentForce(maxVelocity);
        force += steeringBehaviors.GetCohesionForce(maxVelocity);
        force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity);
        force += steeringBehaviors.GetFearForce(maxVelocity);
        force += steeringBehaviors.GetSeekForce(maxVelocity);
        force += steeringBehaviors.GetSeparationForce(maxVelocity);
        force += steeringBehaviors.GetWallAvoidanceForce(maxVelocity);
        force += steeringBehaviors.GetWanderForce(maxVelocity);

        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force
        rigidbody.AddForce(force - rigidbody.velocity);
        
	}
}
